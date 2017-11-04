﻿using Common;
using Parser.ParseTree;
using System.Collections.Generic;

namespace Parser
{
    public class AnnotationParser
    {
        private ParserContext parser;
        public AnnotationParser(ParserContext parser)
        {
            this.parser = parser;
        }

        public Annotation ParseAnnotation(TokenStream tokens)
        {
            Token annotationToken = tokens.PopExpected("@");
            Token typeToken = tokens.Pop();

            // TODO: refactor this. All built-in annotations should be exempt from the VerifyIdentifier check in an extensible way.
            if (typeToken.Value != this.parser.Keywords.PRIVATE)
            {
                parser.VerifyIdentifier(typeToken);
            }

            List<Expression> args = new List<Expression>();
            if (tokens.PopIfPresent("("))
            {
                while (!tokens.PopIfPresent(")"))
                {
                    if (args.Count > 0)
                    {
                        tokens.PopExpected(",");
                    }

                    args.Add(this.parser.ExpressionParser.Parse(tokens, null));
                }
            }
            return new Annotation(annotationToken, typeToken, args);
        }

        public Multimap<string, Annotation> ParseAnnotations(TokenStream tokens)
        {
            Multimap<string, Annotation> annotations = new Multimap<string, Annotation>();
            while (tokens.IsNext("@"))
            {
                Annotation annotation = this.ParseAnnotation(tokens);
                annotations.Add(annotation.Type, annotation);
            }
            return annotations;
        }
    }
}