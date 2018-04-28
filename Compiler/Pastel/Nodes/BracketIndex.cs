﻿using System;

namespace Pastel.Nodes
{
    internal class BracketIndex : Expression
    {
        public Expression Root { get; set; }
        public Token BracketToken { get; set; }
        public Expression Index { get; set; }

        public BracketIndex(Expression root, Token bracketToken, Expression index) : base(root.FirstToken)
        {
            this.Root = root;
            this.BracketToken = bracketToken;
            this.Index = index;
        }

        public override Expression ResolveNamesAndCullUnusedCode(PastelCompiler compiler)
        {
            this.Root = this.Root.ResolveNamesAndCullUnusedCode(compiler);
            this.Index = this.Index.ResolveNamesAndCullUnusedCode(compiler);
            return this;
        }

        internal override Expression ResolveType(VariableScope varScope, PastelCompiler compiler)
        {
            this.Root = this.Root.ResolveType(varScope, compiler);
            this.Index = this.Index.ResolveType(varScope, compiler);

            PType rootType = this.Root.ResolvedType;
            PType indexType = this.Index.ResolvedType;

            bool badIndex = false;
            if (rootType.RootValue == "List" || rootType.RootValue == "Array")
            {
                badIndex = !indexType.IsIdentical(PType.INT);
                this.ResolvedType = rootType.Generics[0];
            }
            else if (rootType.RootValue == "Dictionary")
            {
                badIndex = !indexType.IsIdentical(rootType.Generics[0]);
                this.ResolvedType = rootType.Generics[1];
            }
            else if (rootType.RootValue == "string")
            {
                badIndex = !indexType.IsIdentical(PType.INT);
                this.ResolvedType = PType.CHAR;
                if (this.Root is InlineConstant && this.Index is InlineConstant)
                {
                    string c = ((string)((InlineConstant)this.Root).Value)[(int)((InlineConstant)this.Index).Value].ToString();
                    InlineConstant newValue = new InlineConstant(PType.CHAR, this.FirstToken, c);
                    newValue.ResolveType(varScope, compiler);
                    return newValue;
                }
            }
            else
            {
                badIndex = true;
            }

            if (badIndex)
            {
                throw new ParserException(this.BracketToken, "Cannot index into a " + rootType + " with a " + indexType + ".");
            }

            return this;
        }

        internal override Expression ResolveWithTypeContext(PastelCompiler compiler)
        {
            this.Root = this.Root.ResolveWithTypeContext(compiler);
            this.Index = this.Index.ResolveWithTypeContext(compiler);

            Expression[] args = new Expression[] { this.Root, this.Index };
            NativeFunction nf;
            switch (this.Root.ResolvedType.RootValue)
            {
                case "string": nf = NativeFunction.STRING_CHAR_AT; break;
                case "List": nf = NativeFunction.LIST_GET; break;
                case "Dictionary": nf = NativeFunction.DICTIONARY_GET; break;
                case "Array": nf = NativeFunction.ARRAY_GET; break;
                default: throw new InvalidOperationException(); // this should have been caught earlier in ResolveType()
            }
            return new NativeFunctionInvocation(this.FirstToken, nf, args) { ResolvedType = this.ResolvedType };
        }
    }
}
