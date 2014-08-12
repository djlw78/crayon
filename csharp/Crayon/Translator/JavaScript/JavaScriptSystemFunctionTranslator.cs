﻿using System;
using System.Collections.Generic;

namespace Crayon.Translator.JavaScript
{
	internal class JavaScriptSystemFunctionTranslator : AbstractSystemFunctionTranslator
	{
		public JavaScriptSystemFunctionTranslator(AbstractPlatformImplementation platform)
			: base(platform)
		{ }

		protected override void TranslateUnsafeFloatDivision(List<string> output, ParseTree.Expression numerator, ParseTree.Expression denominator)
		{
			this.Translator.TranslateExpression(output, numerator);
			output.Add(" / ");
			this.Translator.TranslateExpression(output, denominator);
		}

		protected override void TranslateStringCharAt(List<string> output, ParseTree.Expression stringValue, ParseTree.Expression index)
		{
			this.Translator.TranslateExpression(output, stringValue);
			output.Add(".charAt(");
			this.Translator.TranslateExpression(output, index);
			output.Add(")");
		}

		protected override void TranslateListSplit(List<string> output, ParseTree.Expression originalString, ParseTree.Expression sep)
		{
			this.Translator.TranslateExpression(output, originalString);
			output.Add(".split(");
			this.Translator.TranslateExpression(output, sep);
			output.Add(")");
		}

		protected override void TranslateListSet(List<string> output, ParseTree.Expression list, ParseTree.Expression index, ParseTree.Expression value)
		{
			this.Translator.TranslateExpression(output, list);
			output.Add("[");
			this.Translator.TranslateExpression(output, index);
			output.Add("] = ");
			this.Translator.TranslateExpression(output, value);
		}

		protected override void TranslateUnsafeIntegerDivision(List<string> output, ParseTree.Expression numerator, ParseTree.Expression denominator)
		{
			output.Add("Math.floor(");
			this.Translator.TranslateExpression(output, numerator);
			output.Add(" / ");
			this.Translator.TranslateExpression(output, denominator);
			output.Add(")");
		}

		protected override void TranslateListGet(List<string> output, ParseTree.Expression list, ParseTree.Expression index)
		{
			this.Translator.TranslateExpression(output, list);
			output.Add("[");
			this.Translator.TranslateExpression(output, index);
			output.Add("]");
		}

		protected override void TranslateListLength(List<string> output, ParseTree.Expression list)
		{
			output.Add("(");
			this.Translator.TranslateExpression(output, list);
			output.Add(").length");
		}

		protected override void TranslateComment(List<string> output, ParseTree.Expression commentValue)
		{
#if DEBUG
			output.Add("// " + ((ParseTree.StringConstant)commentValue).Value);
#endif
		}

		protected override void TranslateDictionaryGet(List<string> output, ParseTree.Expression dictionary, ParseTree.Expression key, ParseTree.Expression defaultValue)
		{
			output.Add("slow_dictionary_get(");
			this.Translator.TranslateExpression(output, dictionary);
			output.Add(", ");
			this.Translator.TranslateExpression(output, key);
			output.Add(", ");
			this.Translator.TranslateExpression(output, defaultValue);
			output.Add(")");
		}

		protected override void TranslateListReverse(List<string> output, ParseTree.Expression listVar)
		{
			this.Translator.TranslateExpression(output, listVar);
			output.Add(".reverse()");
		}

		protected override void TranslatePrint(List<string> output, ParseTree.Expression message)
		{
			output.Add("window.alert(");
			this.Translator.TranslateExpression(output, message);
			output.Add(")");
		}

		protected override void TranslateDictionarySet(List<string> output, ParseTree.Expression dict, ParseTree.Expression key, ParseTree.Expression value)
		{
			output.Add("slow_dictionary_set(");
			this.Translator.TranslateExpression(output, dict);
			output.Add(", ");
			this.Translator.TranslateExpression(output, key);
			output.Add(", ");
			this.Translator.TranslateExpression(output, value);
			output.Add(")");
		}

		protected override void TranslateStringCast(List<string> output, ParseTree.Expression thing, bool strongCast)
		{
			if (strongCast)
			{
				output.Add("('' + ");
				this.Translator.TranslateExpression(output, thing);
				output.Add(")");
			}
			else
			{
				this.Translator.TranslateExpression(output, thing);
			}
		}

		protected override void TranslateInsertFrameworkCode(string tab, List<string> output, string id)
		{
			switch (id)
			{
				case "ff_clock_tick":
					//output.Add("_global_vars['clock'].tick(_global_vars['fps'])");
					//break;
					output.Add("AHHHHHHHHHHHHHHHHHHHHHHHHHH\r\nTICK TOCK TICK TOCK TICK TOCK");
					break;

				case "ff_current_time":
					//output.Add("v_output = [" + (int)Types.FLOAT + ", time.time()]");
					//break;
					output.Add("v_output = (Date.now ? Date.now() : new Date().getTime()) / 1000;");
					break;

				case "ff_draw_rectangle":
					//output.Add("_PDR(v_scr[1][1], (v_red[1], v_green[1], v_blue[1]), _PR(v_x[1], v_y[1], v_width[1], v_height[1]))");
					//break;
					output.Add("TODO('draw a rectangle.');");
					break;

				case "ff_fill_screen":
					output.Add("TODO('fill the screen');");
					break;

				case "ff_invalidate_display":
					output.Add("TODO('invalidate display');");
					break;

				case "ff_floor":
					output.Add("v_output = Math.floor(TODO('what do I put here?'));");
					break;

				case "ff_get_events":
					output.Add("TODO('get events');");
					break;

				case "ff_initialize_game":
					output.Add("TODO('init game.');");
					break;

				case "ff_initialize_screen":
					output.Add("TODO('initialize screen');");
					break;

				case "ff_print":
					output.Add("TODO('print!');");
					break;

				case "ff_random":
					output.Add("v_output = [" + (int)Types.FLOAT + ", Math.random()];");
					break;

				case "ff_set_title":
					output.Add("TODO('set title...is this possible?');");
					break;

				default:
					throw new NotImplementedException();
			}
		}

		protected override void TranslateListPush(List<string> output, ParseTree.Expression list, ParseTree.Expression value)
		{
			this.Translator.TranslateExpression(output, list);
			output.Add(".push(");
			this.Translator.TranslateExpression(output, value);
			output.Add(")");
		}

		protected override void TranslateListRemoveAt(List<string> output, ParseTree.Expression list, ParseTree.Expression index)
		{
			this.Translator.TranslateExpression(output, list);
			output.Add(".splice(");
			this.Translator.TranslateExpression(output, index);
			output.Add(", 1)");
		}

		protected override void TranslateStringLength(List<string> output, ParseTree.Expression stringValue)
		{
			this.Translator.TranslateExpression(output, stringValue);
			output.Add(".length");
		}

		protected override void TranslateKillExecution(List<string> output, ParseTree.Expression exceptionMessage)
		{
			throw new NotImplementedException();
		}

		protected override void TranslateListPop(List<string> output, ParseTree.Expression list)
		{
			this.Translator.TranslateExpression(output, list);
			output.Add(".pop()");
		}
	}
}
