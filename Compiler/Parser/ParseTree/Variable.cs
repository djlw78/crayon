﻿namespace Parser.ParseTree
{
    public class Variable : Expression
    {
        public override bool IsInlineCandidate { get { return true; } }

        public override bool CanAssignTo { get { return true; } }

        public string Name { get; private set; }

        public int LocalScopeId { get; set; }

        public Variable(Token token, string name, TopLevelConstruct owner)
            : base(token, owner)
        {
            this.Name = name;
        }

        internal override Expression Resolve(ParserContext parser)
        {
            if (this.Name == "$var")
            {
                return new CompileTimeDictionary(this.FirstToken, "var", this.Owner);
            }

            if (parser.IsReservedKeyword(this.Name))
            {
                throw new ParserException(this.FirstToken, "'" + this.Name + "' is a reserved keyword and cannot be used like this.");
            }

            return this;
        }

        internal override Expression ResolveEntityNames(ParserContext parser)
        {
            if (this.Name == "$$$")
            {
                throw new ParserException(this.FirstToken, "Core function invocations cannot stand alone and must be immediately invoked.");
            }

            if (this.Name.StartsWith("$$"))
            {
                return new LibraryFunctionReference(this.FirstToken, this.Name.Substring(2), this.Owner);
            }

            if (this.Name == "this" || this.Name == "base")
            {
                TopLevelConstruct container = parser.CurrentCodeContainer;

                if (container is FunctionDefinition)
                {
                    FunctionDefinition funcDef = (FunctionDefinition)this.Owner;
                    if (funcDef.IsStaticMethod)
                    {
                        throw new ParserException(this.FirstToken, "Cannot use '" + this.Name + "' in a static method");
                    }

                    if (funcDef.Owner == null)
                    {
                        throw new ParserException(this.FirstToken, "Cannot use '" + this.Name + "' in a function that isn't a class method.");
                    }
                }

                if (container is FieldDeclaration)
                {
                    if (((FieldDeclaration)container).IsStaticField)
                    {
                        throw new ParserException(this.FirstToken, "Cannot use '" + this.Name + "' in a static field value.");
                    }
                }

                if (container is ConstructorDefinition)
                {
                    ConstructorDefinition constructor = (ConstructorDefinition)container;
                    if (constructor == ((ClassDefinition)constructor.Owner).StaticConstructor) // TODO: This check is silly. Add an IsStatic field to ConstructorDefinition.
                    {
                        throw new ParserException(this.FirstToken, "Cannot use '" + this.Name + "' in a static constructor.");
                    }
                }

                if (this.Name == "this")
                {
                    return new ThisKeyword(this.FirstToken, this.Owner);
                }
                return new BaseKeyword(this.FirstToken, this.Owner);
            }

            NamespaceReferenceTemplate nrt = this.Owner.FileScope.FileScopeEntityLookup.DoNamespaceLookup(this.Name, this.Owner);
            if (nrt != null)
            {
                return new NamespaceReference(this.FirstToken, this.Owner, nrt);
            }

            TopLevelConstruct exec = this.Owner.FileScope.FileScopeEntityLookup.DoEntityLookup(this.Name, this.Owner);

            if (exec != null)
            {
                return Resolver.ResolverPipeline.ConvertStaticReferenceToExpression(exec, this.FirstToken, this.Owner);
            }

            return this;
        }

        internal override void PerformLocalIdAllocation(ParserContext parser, VariableIdAllocator varIds, VariableIdAllocPhase phase)
        {
            if ((phase & VariableIdAllocPhase.ALLOC) != 0)
            {
                this.LocalScopeId = varIds.GetVarId(this.FirstToken);
                if (this.LocalScopeId == -1)
                {
                    string name = this.FirstToken.Value;

                    if (parser.LibraryManager.IsValidLibraryNameFromLocale(this.Locale, name))
                    {
                        throw new ParserException(this.FirstToken, "'" + name + "' is referenced but not imported in this file.");
                    }

                    TopLevelConstruct owner = this.Owner;
                    while (owner != null && !(owner is ClassDefinition))
                    {
                        owner = owner.Owner;
                    }

                    if (owner != null)
                    {
                        ClassDefinition cd = (ClassDefinition)owner;
                        foreach (FieldDeclaration fd in cd.Fields)
                        {
                            if (fd.NameToken.Value == name)
                            {
                                string message = "'" + name + "' is used like a local variable but it is " + (fd.IsStaticField ? "a static" : "an instance") + " field.";
                                message += " Did you mean '" + (fd.IsStaticField ? cd.NameToken.Value : "this") + "." + name + "' instead of '" + name + "'?";
                                throw new ParserException(this.FirstToken, message);
                            }
                        }
                    }

                    // TODO: But if it's being called like a function then...
                    // - give a better error message "function 'foo' is not defined"
                    // - give an even better error message when there's a class or instance function with the same name
                    //   e.g. "'foo' is a static function and must be invoked with the class name: FooClass.foo(...)
                    // - if there's a method, suggest using "this."
                    // - if the variable name matches a library that is available, suggest it as a missing import.
                    throw new ParserException(this.FirstToken, "The variable '" + name + "' is used but is never assigned to.");
                }
            }
        }

        public override string ToString()
        {
            return "<Variable> " + this.Name;
        }
    }
}
