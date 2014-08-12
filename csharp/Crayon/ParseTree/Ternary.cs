﻿namespace Crayon.ParseTree
{
	internal class Ternary : Expression
	{
		public Expression Condition { get; private set; }
		public Expression TrueValue { get; private set; }
		public Expression FalseValue { get; private set; }

		public Ternary(Expression condition, Expression trueValue, Expression falseValue)
			: base(condition.FirstToken)
		{
			this.Condition = condition;
			this.TrueValue = trueValue;
			this.FalseValue = falseValue;
		}

		public override Expression Resolve(Parser parser)
		{
			this.Condition = this.Condition.Resolve(parser);
			this.TrueValue = this.TrueValue.Resolve(parser);
			this.FalseValue = this.FalseValue.Resolve(parser);

			BooleanConstant bc = this.Condition as BooleanConstant;
			if (bc != null)
			{
				return bc.Value ? this.TrueValue : this.FalseValue;
			}

			return this;
		}
	}
}
