﻿using Parser.Resolver;

namespace Parser.ParseTree
{
    public class IsComparison : Expression
    {
        public Expression Expression { get; set; }
        public Token IsToken { get; set; }
        public Token ClassToken { get; set; }
        public string ClassName { get; set; }
        public ClassDefinition ClassDefinition { get; set; }

        public override bool CanAssignTo { get { return false; } }

        public IsComparison(
            Expression root,
            Token isToken,
            Token firstClassToken,
            string classNameWithNamespace,
            Node owner)
            : base(root.FirstToken, owner)
        {
            this.Expression = root;
            this.IsToken = isToken;
            this.ClassToken = firstClassToken;
            this.ClassName = classNameWithNamespace;
        }

        internal override Expression Resolve(ParserContext parser)
        {
            this.Expression.Resolve(parser);
            return this;
        }

        internal override Expression ResolveEntityNames(ParserContext parser)
        {
            this.Expression.ResolveEntityNames(parser);
            this.ClassDefinition = this.FileScope.DoClassLookup(this, this.ClassToken, this.ClassName);
            return this;
        }

        internal override Expression ResolveTypes(ParserContext parser, TypeResolver typeResolver)
        {
            // TODO: it'd be nice to just inline the result here, if possible.
            this.Expression = this.Expression.ResolveTypes(parser, typeResolver);
            this.ResolvedType = ResolvedType.BOOLEAN;
            return this;
        }

        internal override void PerformLocalIdAllocation(ParserContext parser, VariableScope varIds, VariableIdAllocPhase phase)
        {
            this.Expression.PerformLocalIdAllocation(parser, varIds, phase);
        }
    }
}
