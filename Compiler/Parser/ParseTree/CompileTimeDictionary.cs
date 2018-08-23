﻿using System.Collections.Generic;

namespace Parser.ParseTree
{
    public class CompileTimeDictionary : Expression
    {
        public override bool CanAssignTo { get { return false; } }

        public string Type { get; private set; }

        public CompileTimeDictionary(Token firstToken, string type, TopLevelConstruct owner)
            : base(firstToken, owner)
        {
            this.Type = type;
        }

        internal override Expression Resolve(ParserContext parser)
        {
            return this;
        }

        internal override Expression ResolveEntityNames(ParserContext parser)
        {
            return this;
        }

        internal override void GetAllVariablesReferenced(HashSet<Variable> vars) { }
        internal override void PerformLocalIdAllocation(ParserContext parser, VariableIdAllocator varIds, VariableIdAllocPhase phase) { }
    }
}
