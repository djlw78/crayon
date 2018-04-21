﻿using System;
using System.Collections.Generic;
using System.Linq;
using Pastel.Nodes;
using Common;

namespace Pastel
{
    public class PastelCompiler
    {
        internal PastelCompiler[] IncludedScopes { get; private set; }

        internal IDictionary<string, ExtensibleFunction> ExtensibleFunctions { get; private set; }

        public PastelCompiler(
            IList<PastelCompiler> includedScopes,
            IDictionary<string, object> constants,
            IInlineImportCodeLoader inlineImportCodeLoader,
            ICollection<ExtensibleFunction> extensibleFunctions)
        {
            this.IncludedScopes = includedScopes.ToArray();
            this.ExtensibleFunctions = extensibleFunctions == null
                ? new Dictionary<string, ExtensibleFunction>()
                : extensibleFunctions.ToDictionary(ef => ef.Name);
            this.StructDefinitions = new Dictionary<string, StructDefinition>();
            this.EnumDefinitions = new Dictionary<string, EnumDefinition>();
            this.Globals = new Dictionary<string, VariableDeclaration>();
            this.ConstantDefinitions = new Dictionary<string, VariableDeclaration>();
            this.FunctionDefinitions = new Dictionary<string, FunctionDefinition>();
            this.interpreterParser = new PastelParser(constants, inlineImportCodeLoader);
        }

        private PastelParser interpreterParser;

        public Dictionary<string, StructDefinition> StructDefinitions { get; set; }
        internal Dictionary<string, EnumDefinition> EnumDefinitions { get; set; }
        public Dictionary<string, VariableDeclaration> Globals { get; set; }
        internal Dictionary<string, VariableDeclaration> ConstantDefinitions { get; set; }
        public Dictionary<string, FunctionDefinition> FunctionDefinitions { get; set; }

        internal HashSet<string> ResolvedFunctions { get; set; }
        internal Queue<string> ResolutionQueue { get; set; }

        internal InlineConstant GetConstantDefinition(string name)
        {
            if (this.ConstantDefinitions.ContainsKey(name))
            {
                return (InlineConstant)this.ConstantDefinitions[name].Value;
            }

            foreach (PastelCompiler includedScope in this.IncludedScopes)
            {
                if (includedScope.ConstantDefinitions.ContainsKey(name))
                {
                    return (InlineConstant)includedScope.ConstantDefinitions[name].Value;
                }
            }

            return null;
        }

        internal EnumDefinition GetEnumDefinition(string name)
        {
            if (this.EnumDefinitions.ContainsKey(name))
            {
                return this.EnumDefinitions[name];
            }

            foreach (PastelCompiler includedScope in this.IncludedScopes)
            {
                if (includedScope.EnumDefinitions.ContainsKey(name))
                {
                    return includedScope.EnumDefinitions[name];
                }
            }

            return null;
        }

        internal StructDefinition GetStructDefinition(string name)
        {
            if (this.StructDefinitions.ContainsKey(name))
            {
                return this.StructDefinitions[name];
            }

            foreach (PastelCompiler includedScope in this.IncludedScopes)
            {
                if (includedScope.StructDefinitions.ContainsKey(name))
                {
                    return includedScope.StructDefinitions[name];
                }
            }

            return null;
        }

        internal FunctionDefinition GetFunctionDefinitionAndMaybeQueueForResolution(string name)
        {
            if (this.FunctionDefinitions.ContainsKey(name))
            {
                return this.FunctionDefinitions[name];
            }

            foreach (PastelCompiler includedScope in this.IncludedScopes)
            {
                if (includedScope.FunctionDefinitions.ContainsKey(name))
                {
                    return includedScope.FunctionDefinitions[name];
                }
            }

            return null;
        }

        public void CompileBlobOfCode(string name, string code)
        {
            ICompilationEntity[] entities = this.interpreterParser.ParseText(name, code);
            foreach (ICompilationEntity entity in entities)
            {
                switch (entity.EntityType)
                {
                    case CompilationEntityType.FUNCTION:
                        FunctionDefinition fnDef = (FunctionDefinition)entity;
                        string functionName = fnDef.NameToken.Value;
                        if (this.FunctionDefinitions.ContainsKey(functionName))
                        {
                            throw new ParserException(fnDef.FirstToken, "Multiple definitions of function: '" + functionName + "'");
                        }
                        this.FunctionDefinitions[functionName] = fnDef;
                        break;

                    case CompilationEntityType.STRUCT:
                        StructDefinition structDef = (StructDefinition)entity;
                        string structName = structDef.NameToken.Value;
                        if (this.StructDefinitions.ContainsKey(structName))
                        {
                            throw new ParserException(structDef.FirstToken, "Multiple definitions of function: '" + structName + "'");
                        }
                        this.StructDefinitions[structName] = structDef;
                        break;

                    case CompilationEntityType.ENUM:
                        EnumDefinition enumDef = (EnumDefinition)entity;
                        string enumName = enumDef.NameToken.Value;
                        if (this.EnumDefinitions.ContainsKey(enumName))
                        {
                            throw new ParserException(enumDef.FirstToken, "Multiple definitions of function: '" + enumName + "'");
                        }
                        this.EnumDefinitions[enumName] = enumDef;
                        break;

                    case CompilationEntityType.CONSTANT:
                    case CompilationEntityType.GLOBAL:
                        VariableDeclaration assignment = (VariableDeclaration)entity;
                        string targetName = assignment.VariableNameToken.Value;
                        Dictionary<string, VariableDeclaration> lookup = entity.EntityType == CompilationEntityType.CONSTANT
                            ? this.ConstantDefinitions
                            : this.Globals;
                        if (lookup.ContainsKey(targetName))
                        {
                            throw new ParserException(
                                assignment.FirstToken,
                                "Multiple definitions of : '" + targetName + "'");
                        }
                        lookup[targetName] = assignment;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public void Resolve()
        {
            using (new PerformanceSection("PastelCompiler.Resolve"))
            {
                this.ResolveConstants();
                this.ResolveNamesAndCullUnusedCode();
                this.ResolveTypes();
                this.ResolveWithTypeContext();
            }
        }

        private void ResolveConstants()
        {
            using (new PerformanceSection("PastelCompiler.ResolveConstants"))
            {
                HashSet<string> cycleDetection = new HashSet<string>();
                foreach (EnumDefinition enumDef in this.EnumDefinitions.Values)
                {
                    if (enumDef.UnresolvedValues.Count > 0)
                    {
                        enumDef.DoConstantResolutions(cycleDetection, this);
                    }
                }

                foreach (VariableDeclaration constDef in this.ConstantDefinitions.Values)
                {
                    if (!(constDef.Value is InlineConstant))
                    {
                        string name = constDef.VariableNameToken.Value;
                        cycleDetection.Add(name);
                        constDef.DoConstantResolutions(cycleDetection, this);
                        cycleDetection.Remove(name);
                    }
                }
            }
        }

        private void ResolveNamesAndCullUnusedCode()
        {
            using (new PerformanceSection("PastelCompiler.ResolveNamesAndCullUnusedCode"))
            {
                string[] globalNames = this.Globals.Keys.ToArray();
                for (int i = 0; i < globalNames.Length; ++i)
                {
                    string name = globalNames[i];
                    this.Globals[name] = (VariableDeclaration)this.Globals[name].ResolveNamesAndCullUnusedCode(this);
                }

                this.ResolvedFunctions = new HashSet<string>();
                this.ResolutionQueue = new Queue<string>();

                foreach (string functionName in this.FunctionDefinitions.Keys)
                {
                    this.ResolutionQueue.Enqueue(functionName);
                }

                while (this.ResolutionQueue.Count > 0)
                {
                    string functionName = this.ResolutionQueue.Dequeue();
                    if (!this.ResolvedFunctions.Contains(functionName)) // multiple invocations in a function will put it in the queue multiple times.
                    {
                        this.ResolvedFunctions.Add(functionName);
                        this.FunctionDefinitions[functionName].ResolveNamesAndCullUnusedCode(this);
                    }
                }

                List<string> unusedFunctions = new List<string>();
                foreach (string functionName in this.FunctionDefinitions.Keys)
                {
                    if (!this.ResolvedFunctions.Contains(functionName))
                    {
                        unusedFunctions.Add(functionName);
                    }
                }

                Dictionary<string, FunctionDefinition> finalFunctions = new Dictionary<string, FunctionDefinition>();
                foreach (string usedFunctionName in this.ResolvedFunctions)
                {
                    finalFunctions[usedFunctionName] = this.FunctionDefinitions[usedFunctionName];
                }
                this.FunctionDefinitions = finalFunctions;
            }
        }

        private void ResolveTypes()
        {
            using (new PerformanceSection("PastelCompiler.ResolveTypes"))
            {
                foreach (VariableDeclaration global in this.Globals.Values)
                {
                    VariableScope vs = new VariableScope();
                    global.ResolveTypes(vs, this);
                }

                string[] functionNames = this.FunctionDefinitions.Keys.OrderBy(s => s).ToArray();
                foreach (string functionName in functionNames)
                {
                    FunctionDefinition functionDefinition = this.FunctionDefinitions[functionName];
                    functionDefinition.ResolveTypes(this);
                }
            }
        }

        private void ResolveWithTypeContext()
        {
            using (new PerformanceSection("PastelCompiler.ResolveWithTypeContext"))
            {
                string[] globalNames = this.Globals.Keys.OrderBy(s => s).ToArray();
                foreach (string globalName in globalNames)
                {
                    this.Globals[globalName] = (VariableDeclaration)this.Globals[globalName].ResolveWithTypeContext(this);
                }

                string[] functionNames = this.FunctionDefinitions.Keys.OrderBy<string, string>(s => s).ToArray();
                foreach (string functionName in functionNames)
                {
                    FunctionDefinition functionDefinition = this.FunctionDefinitions[functionName];
                    functionDefinition.ResolveWithTypeContext(this);
                }
            }
        }
    }
}
