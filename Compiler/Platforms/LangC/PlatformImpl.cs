﻿using Common;
using Pastel.Nodes;
using Pastel.Transpilers;
using Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace LangC
{
    public class PlatformImpl : AbstractPlatform
    {
        public override string Name { get { return "lang-c"; } }
        public override string InheritsFrom { get { return null; } }
        public override string NL { get { return "\n"; } }

        public PlatformImpl() : base()
        {
            this.ContextFreePlatformImpl = new ContextFreeLangCPlatform();
        }

        public override IDictionary<string, object> GetConstantFlags()
        {
            return new Dictionary<string, object>()
                {
                    { "IS_ASYNC", false },
                    { "PLATFORM_SUPPORTS_LIST_CLEAR", true },
                    { "STRONGLY_TYPED", true },
                    { "IS_ARRAY_SAME_AS_LIST", false },
                    { "IS_PYTHON", false },
                    { "IS_CHAR_A_NUMBER", true },
                    { "INT_IS_FLOOR", true },
                    { "IS_THREAD_BLOCKING_ALLOWED", true },
                    { "HAS_INCREMENT", true },
                    { "IS_C", true },
                };
        }

        public override void ExportStandaloneVm(Dictionary<string, FileOutput> output, Pastel.PastelCompiler compiler, IList<LibraryForExport> everyLibrary, ILibraryNativeInvocationTranslatorProvider libraryNativeInvocationTranslatorProviderForPlatform)
        {
            throw new NotImplementedException();
        }

        public override void ExportProject(Dictionary<string, FileOutput> output, Pastel.PastelCompiler compiler, IList<LibraryForExport> libraries, ResourceDatabase resourceDatabase, Options options, ILibraryNativeInvocationTranslatorProvider libraryNativeInvocationTranslatorProviderForPlatform)
        {
            throw new NotImplementedException();
        }

        public override void GenerateCodeForStruct(TranspilerContext sb, AbstractTranslator translator, StructDefinition structDef)
        {
            if (structDef.NameToken.Value == "Value")
            {
                // I need to do fancy stuff with unions, so special case this one.
                string valueStruct = this.LoadTextResource("Resources/ValueStruct.txt", new Dictionary<string, string>());
                sb.Append(valueStruct);
                return;
            }

            sb.Append("struct ");
            sb.Append(structDef.NameToken.Value);
            sb.Append(" {\n");
            for (int i = 0; i < structDef.ArgNames.Length; ++i)
            {
                string fieldName = structDef.ArgNames[i].Value;
                PType fieldType = structDef.ArgTypes[i];
                sb.Append('\t');
                sb.Append(translator.TranslateType(fieldType));
                sb.Append(' ');
                sb.Append(fieldName);
                sb.Append(";\n");
            }

            sb.Append("};\n\n");
        }

        public override void GenerateCodeForFunction(TranspilerContext sb, AbstractTranslator translator, FunctionDefinition funcDef)
        {
            sb.Append(this.Translator.TranslateType(funcDef.ReturnType));
            sb.Append(" v_");
            sb.Append(funcDef.NameToken.Value);
            sb.Append('(');
            for (int i = 0; i < funcDef.ArgNames.Length; ++i)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(this.Translator.TranslateType(funcDef.ArgTypes[i]));
                sb.Append(" v_");
                sb.Append(funcDef.ArgNames[i].Value);
            }
            sb.Append(")\n{\n");
            translator.TabDepth = 1;
            translator.TranslateExecutables(sb, funcDef.Code);
            translator.TabDepth = 0;
            sb.Append("}\n");
        }

        public override void GenerateCodeForGlobalsDefinitions(TranspilerContext sb, AbstractTranslator translator, IList<VariableDeclaration> globals)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, string> GenerateReplacementDictionary(Options options, ResourceDatabase resDb)
        {
            throw new NotImplementedException();
        }

        public static void BuildStringTable(StringBuilder sb, StringTableBuilder stringTable, string newline)
        {
            List<string> names = stringTable.Names;
            List<string> values = stringTable.Values;
            int total = names.Count;
            for (int i = 0; i < total; ++i)
            {
                sb.Append("int* ");
                sb.Append(names[i]);
                sb.Append(';');
                sb.Append(newline);
            }
            sb.Append("void populate_string_table_for_");
            sb.Append(stringTable.Prefix);
            sb.Append("()");
            sb.Append(newline);
            sb.Append('{');
            sb.Append(newline);
            for (int i = 0; i < total; ++i)
            {
                sb.Append('\t');
                sb.Append(names[i]);
                sb.Append(" = String_from_utf8(");
                sb.Append(Common.Util.ConvertStringValueToCode(values[i]).Replace("%", "%%"));
                sb.Append(");");
                sb.Append(newline);
            }
            sb.Append('}');
            sb.Append(newline);
            sb.Append(newline);
        }

        public override void GenerateCodeForFunctionDeclaration(TranspilerContext sb, AbstractTranslator translator, FunctionDefinition funcDef)
        {
            sb.Append(this.Translator.TranslateType(funcDef.ReturnType));
            sb.Append(" v_");
            sb.Append(funcDef.NameToken.Value);
            sb.Append('(');
            for (int i = 0; i < funcDef.ArgNames.Length; ++i)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(this.Translator.TranslateType(funcDef.ArgTypes[i]));
                sb.Append(" v_");
                sb.Append(funcDef.ArgNames[i].Value);
            }
            sb.Append(");");
        }
    }
}
