﻿using System;
using System.Collections.Generic;

namespace Parser.ParseTree
{
    public class ClassReferenceLiteral : Expression
    {
        public ClassDefinition ClassDefinition { get; set; }

        public ClassReferenceLiteral(Token firstToken, ClassDefinition cd, TopLevelConstruct owner)
            : base(firstToken, owner)
        {
            this.ClassDefinition = cd;
        }

        public override bool CanAssignTo { get { return false; } }

        internal override void PerformLocalIdAllocation(ParserContext parser, VariableScope varIds, VariableIdAllocPhase phase) { }

        internal override Expression Resolve(ParserContext parser) { return this; }

        internal override Expression ResolveEntityNames(ParserContext parser)
        {
            // ClassReferenceLiteral is created in the Resolve pass, so this is never called.
            throw new InvalidOperationException();
        }
    }
}
