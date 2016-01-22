﻿namespace Crayon.ParseTree
{
	internal class NegativeSign : Expression
	{
		public Expression Root { get; private set; }

		public NegativeSign(Token sign, Expression root)
			: base(sign)
		{
			this.Root = root;
		}

		internal override Expression Resolve(Parser parser)
		{
			this.Root = this.Root.Resolve(parser);
			if (this.Root is IntegerConstant)
			{
				return new IntegerConstant(this.FirstToken, ((IntegerConstant)this.Root).Value * -1);
			}

			if (this.Root is FloatConstant)
			{
				return new FloatConstant(this.FirstToken, ((FloatConstant)this.Root).Value * -1);
			}

			return this;
		}

		internal override void VariableUsagePass(Parser parser)
		{
			this.Root.VariableUsagePass(parser);
		}

		internal override void VariableIdAssignmentPass(Parser parser)
		{
			this.Root.VariableIdAssignmentPass(parser);
		}
	}
}
