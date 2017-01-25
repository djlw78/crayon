﻿using System;
using System.Collections.Generic;
using Crayon.ParseTree;

namespace Crayon.Translator.JavaScript
{
    internal class JavaScriptSystemFunctionTranslator : AbstractSystemFunctionTranslator
    {
        protected override void TranslateResourceGetManifest(List<string> output)
        {
            output.Add("C$resourceManifest");
        }

        protected override void TranslateCast(List<string> output, StringConstant typeValue, Expression expression)
        {
            this.Translator.TranslateExpression(output, expression);
        }

        protected override void TranslateCastToList(List<string> output, StringConstant typeValue, Expression enumerableThing)
        {
            this.Translator.TranslateExpression(output, enumerableThing);
        }

        protected override void TranslateChr(List<string> output, Expression asciiValue)
        {
            output.Add("String.fromCharCode(");
            this.Translator.TranslateExpression(output, asciiValue);
            output.Add(")");
        }

        protected override void TranslateCommandLineArgs(List<string> output)
        {
            output.Add("[]");
        }

        protected override void TranslateComment(List<string> output, StringConstant commentValue)
        {
#if DEBUG
            output.Add("// " + commentValue.Value);
#endif
        }

        protected override void TranslateConvertListToArray(List<string> output, StringConstant type, Expression list)
        {
            this.Translator.TranslateExpression(output, list);
        }

        protected override void TranslateDictionaryGetGuaranteed(List<string> output, Expression dictionary, Expression key)
        {
            this.Translator.TranslateExpression(output, dictionary);
            output.Add("[");
            this.Translator.TranslateExpression(output, key);
            output.Add("]");
        }

        protected override void TranslateDictionaryGetKeys(List<string> output, string keyType, Expression dictionary)
        {
            output.Add("C$common$dictionaryKeys(");
            this.Translator.TranslateExpression(output, dictionary);
            output.Add(")");
        }

        protected override void TranslateDictionaryGetValues(List<string> output, Expression dictionary)
        {
            output.Add("C$common$dictionaryValues(");
            this.Translator.TranslateExpression(output, dictionary);
            output.Add(")");
        }

        protected override void TranslateDictionaryRemove(List<string> output, Expression dictionary, Expression key)
        {
            output.Add("delete ");
            this.Translator.TranslateExpression(output, dictionary);
            output.Add("[");
            this.Translator.TranslateExpression(output, key);
            output.Add("]");
        }

        protected override void TranslateDictionarySet(List<string> output, Expression dictionary, Expression key, Expression value)
        {
            this.Translator.TranslateExpression(output, dictionary);
            output.Add("[");
            this.Translator.TranslateExpression(output, key);
            output.Add("] = ");
            this.Translator.TranslateExpression(output, value);
        }

        protected override void TranslateDictionarySize(List<string> output, Expression dictionary)
        {
            output.Add("Object.keys(");
            this.Translator.TranslateExpression(output, dictionary);
            output.Add(").length");
        }

        protected override void TranslateEnqueueVmResume(List<string> output, Expression seconds, Expression executionContextId)
        {
            output.Add("C$common$enqueueVmResume(");
            this.Translator.TranslateExpression(output, seconds);
            output.Add(", ");
            this.Translator.TranslateExpression(output, executionContextId);
            output.Add(")");
        }

        protected override void TranslateInt(List<string> output, Expression value)
        {
            output.Add("Math.floor(");
            this.Translator.TranslateExpression(output, value);
            output.Add(")");
        }

        protected override void TranslateIsWindowsProgram(List<string> output)
        {
            // TODO: make platforms that have a constant value for this also have a
            // %%% platform parameter that can be used to short circuit interpreter code.
            output.Add("false");
        }

        protected override void TranslateNewArray(List<string> output, StringConstant type, Expression size)
        {
            this.TranslateNewListOfSize(output, type, size);
        }

        protected override void TranslateNewDictionary(List<string> output, StringConstant keyType, StringConstant valueType)
        {
            output.Add("{}");
        }

        protected override void TranslateNewList(List<string> output, StringConstant type)
        {
            output.Add("[]");
        }

        protected override void TranslateNewListOfSize(List<string> output, StringConstant type, Expression length)
        {
            if (length is IntegerConstant)
            {
                int literalLength = ((IntegerConstant)length).Value;
                switch (literalLength)
                {
                    case 0: output.Add("[]"); return;
                    case 1: output.Add("[null]"); return;
                    case 2: output.Add("[null, null]"); return;
                    case 3: output.Add("[null, null, null]"); return;
                    default: break;
                }
            }
            output.Add("C$common$createNewArray(");
            this.Translator.TranslateExpression(output, length);
            output.Add(")");
        }

        protected override void TranslateOrd(List<string> output, Expression character)
        {
            this.Translator.TranslateExpression(output, character);
            output.Add(".charCodeAt(0)");
        }

        protected override void TranslateParseFloat(List<string> output, Expression outParam, Expression rawString)
        {
            output.Add("C$common$floatParseHelper(");
            this.Translator.TranslateExpression(output, outParam);
            output.Add(", ");
            this.Translator.TranslateExpression(output, rawString);
            output.Add(")");
        }

        protected override void TranslateParseInt(List<string> output, Expression rawString)
        {
            output.Add("parseInt(");
            this.Translator.TranslateExpression(output, rawString);
            output.Add(")");
        }

        protected override void TranslateSortedCopyOfIntArray(List<string> output, Expression list)
        {
            output.Add("C$common$sortedCopyOfArray(");
            this.Translator.TranslateExpression(output, list);
            output.Add(")");
        }

        protected override void TranslateSortedCopyOfStringArray(List<string> output, Expression list)
        {
            output.Add("C$common$sortedCopyOfArray(");
            this.Translator.TranslateExpression(output, list);
            output.Add(")");
        }

        protected override void TranslateStringAppend(List<string> output, Expression target, Expression valueToAppend)
        {
            this.Translator.TranslateExpression(output, target);
            output.Add(" += ");
            this.Translator.TranslateExpression(output, valueToAppend);
        }

        protected override void TranslateStringAsChar(List<string> output, StringConstant stringConstant)
        {
            this.Translator.TranslateExpression(output, stringConstant);
        }

        protected override void TranslateStringCast(List<string> output, Expression thing, bool strongCast)
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

        protected override void TranslateStringCharAt(List<string> output, Expression stringValue, Expression index)
        {
            this.Translator.TranslateExpression(output, stringValue);
            output.Add(".charAt(");
            this.Translator.TranslateExpression(output, index);
            output.Add(")");
        }

        protected override void TranslateStringCharCodeAt(List<string> output, Expression stringValue, Expression index)
        {
            this.Translator.TranslateExpression(output, stringValue);
            output.Add(".charCodeAt(");
            this.Translator.TranslateExpression(output, index);
            output.Add(")");
        }

        protected override void TranslateStringCompareIsReverse(List<string> output, Expression a, Expression b)
        {
            output.Add("(");
            this.Translator.TranslateExpression(output, a);
            output.Add(".localeCompare(");
            this.Translator.TranslateExpression(output, b);
            output.Add(") > 0)");
        }

        protected override void TranslateStringConcat(List<string> output, Expression[] values)
        {
            for (int i = 0; i < values.Length; ++i)
            {
                if (i > 0) output.Add(" + ");
                this.Translator.TranslateExpression(output, values[i]);
            }
        }

        protected override void TranslateStringEquals(List<string> output, Expression aNonNull, Expression b)
        {
            this.Translator.TranslateExpression(output, aNonNull);
            output.Add(" == ");
            this.Translator.TranslateExpression(output, b);
        }

        protected override void TranslateStringFromCode(List<string> output, Expression characterCode)
        {
            output.Add("String.fromCharCode(");
            this.Translator.TranslateExpression(output, characterCode);
            output.Add(")");
        }

        protected override void TranslateStringIndexOf(List<string> output, Expression haystack, Expression needle, Expression optionalStartFrom)
        {
            this.Translator.TranslateExpression(output, haystack);
            output.Add(".indexOf(");
            this.Translator.TranslateExpression(output, needle);
            if (optionalStartFrom != null)
            {
                output.Add(", ");
                this.Translator.TranslateExpression(output, optionalStartFrom);
            }
            output.Add(")");
        }

        protected override void TranslateStringParseFloat(List<string> output, Expression stringValue)
        {
            output.Add("parseFloat(");
            this.Translator.TranslateExpression(output, stringValue);
            output.Add(")");
        }

        protected override void TranslateStringParseInt(List<string> output, Expression value)
        {
            output.Add("parseInt(");
            this.Translator.TranslateExpression(output, value);
            output.Add(")");
        }

        protected override void TranslateStringSubstring(List<string> output, Expression stringExpr, Expression startIndex, Expression optionalLength)
        {
            output.Add("C$common$substring(");
            this.Translator.TranslateExpression(output, stringExpr);
            output.Add(", ");
            this.Translator.TranslateExpression(output, startIndex);
            output.Add(", ");
            if (optionalLength != null)
            {
                this.Translator.TranslateExpression(output, optionalLength);
            }
            else
            {
                output.Add("null");
            }
            output.Add(")");
        }

        protected override void TranslateStringSubstringExistsAt(List<string> output, Expression stringExpr, Expression lookFor, Expression index)
        {
            output.Add("C$common$checksubstring(");
            this.Translator.TranslateExpression(output, stringExpr);
            output.Add(", ");
            this.Translator.TranslateExpression(output, index);
            output.Add(", ");
            this.Translator.TranslateExpression(output, lookFor);
            output.Add(")");
        }

        protected override void TranslateThreadSleep(List<string> output, Expression timeDelaySeconds)
        {
            throw new InvalidOperationException(); // Optimized out.
        }
    }
}
