﻿namespace Crayon.ParseTree
{
	internal class NullConstant : Expression
	{
		public NullConstant(Token token)
			: base(token)
		{ }

		public override bool IsLiteral { get { return true; } }

		public override Expression Resolve(Parser parser)
		{
			return this;
		}
	}
}
