namespace Radical.Windows.Markup
{
    using Radical.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Markup;

    /// <summary>
    /// A markup extension that returns a type, supporting generic types: the type arguments
    /// are used to close the generic type definition.
    /// </summary>
    [ContentProperty("TypeArguments")]
    public class TypeExtension : MarkupExtension
    {
        private Type _closedType;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeExtension"/> class.
        /// </summary>
        public TypeExtension()
        {
            _typeArguments = new List<Type>();

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeExtension"/> class.
        /// </summary>
        /// <param name="typeName">The name of the type.</param>
        public TypeExtension(string typeName)
            : this()
        {
            Ensure.That(typeName).Named("typeName").IsNotNull();

            _typeName = typeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeExtension"/> class.
        /// </summary>
        /// <param name="typeName">The name of the generic type.</param>
        /// <param name="typeArgument1">The first type argument.</param>
        public TypeExtension(string typeName, Type typeArgument1)
            : this(typeName)
        {
            Ensure.That(typeArgument1).Named("typeArgument1").IsNotNull();

            TypeArgument1 = typeArgument1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeExtension"/> class.
        /// </summary>
        /// <param name="typeName">The name of the generic type.</param>
        /// <param name="typeArgument1">The first type argument.</param>
        /// <param name="typeArgument2">The second type argument.</param>
        public TypeExtension(string typeName, Type typeArgument1, Type typeArgument2)
            : this(typeName, typeArgument1)
        {
            Ensure.That(typeArgument2).Named("typeArgument2").IsNotNull();

            TypeArgument2 = typeArgument2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeExtension"/> class.
        /// </summary>
        /// <param name="typeName">The name of the generic type.</param>
        /// <param name="typeArgument1">The first type argument.</param>
        /// <param name="typeArgument2">The second type argument.</param>
        /// <param name="typeArgument3">The third type argument.</param>
        public TypeExtension(string typeName, Type typeArgument1, Type typeArgument2, Type typeArgument3)
            : this(typeName, typeArgument1, typeArgument2)
        {
            Ensure.That(typeArgument3).Named("typeArgument3").IsNotNull();

            TypeArgument3 = typeArgument3;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeExtension"/> class.
        /// </summary>
        /// <param name="typeName">The name of the generic type.</param>
        /// <param name="typeArgument1">The first type argument.</param>
        /// <param name="typeArgument2">The second type argument.</param>
        /// <param name="typeArgument3">The third type argument.</param>
        /// <param name="typeArgument4">The fourth type argument.</param>
        public TypeExtension(string typeName, Type typeArgument1, Type typeArgument2, Type typeArgument3, Type typeArgument4)
            : this(typeName, typeArgument1, typeArgument2, typeArgument3)
        {
            Ensure.That(typeArgument4).Named("typeArgument4").IsNotNull();

            TypeArgument4 = typeArgument4;
        }

        private string _typeName;
        /// <summary>
        /// Gets or sets the name of the type, resolved using the XAML type resolver. Setting it clears <see cref="Type"/>.
        /// </summary>
        [ConstructorArgument("typeName")]
        public string TypeName
        {
            get { return _typeName; }
            set
            {
                Ensure.That(value).Named("value").IsNotNull();

                _typeName = value;
                _type = null;
            }
        }

        private Type _type;
        /// <summary>
        /// Gets or sets the type. Setting it clears <see cref="TypeName"/>.
        /// </summary>
        public Type Type
        {
            get { return _type; }
            set
            {
                Ensure.That(value).Named("value").IsNotNull();

                _type = value;
                _typeName = null;
            }
        }

        private readonly List<Type> _typeArguments;
        /// <summary>
        /// Gets the type arguments used to close a generic type definition.
        /// </summary>
        public IList<Type> TypeArguments
        {
            get { return _typeArguments; }
        }

        /// <summary>
        /// Gets or sets the first type argument.
        /// </summary>
        [ConstructorArgument("typeArgument1")]
        public Type TypeArgument1
        {
            get { return GetTypeArgument(0); }
            set { SetTypeArgument(0, value); }
        }

        /// <summary>
        /// Gets or sets the second type argument.
        /// </summary>
        [ConstructorArgument("typeArgument2")]
        public Type TypeArgument2
        {
            get { return GetTypeArgument(1); }
            set { SetTypeArgument(1, value); }
        }

        /// <summary>
        /// Gets or sets the third type argument.
        /// </summary>
        [ConstructorArgument("typeArgument3")]
        public Type TypeArgument3
        {
            get { return GetTypeArgument(2); }
            set { SetTypeArgument(2, value); }
        }

        /// <summary>
        /// Gets or sets the fourth type argument.
        /// </summary>
        [ConstructorArgument("typeArgument4")]
        public Type TypeArgument4
        {
            get { return GetTypeArgument(3); }
            set { SetTypeArgument(3, value); }
        }

        private Type GetTypeArgument(int index)
        {
            return index < _typeArguments.Count ? _typeArguments[index] : null;
        }

        private void SetTypeArgument(int index, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException();
            }

            if (index > _typeArguments.Count)
            {
                throw new ArgumentOutOfRangeException("Type Arguments need to be specified in the right order.");
            }

            if (index == _typeArguments.Count)
            {
                _typeArguments.Add(type);
            }
            else
            {
                _typeArguments[index] = type;
            }
        }

        /// <summary>
        /// Returns the type, closed with the type arguments if it is a generic type definition.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>The resolved type.</returns>
        /// <exception cref="InvalidOperationException">Neither <see cref="Type"/> nor <see cref="TypeName"/> is set, or the type cannot be resolved.</exception>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_typeName == null && _type == null)
            {
                throw new InvalidOperationException("Must specify Type or TypeName.");
            }

            Type type = _type;
            Type[] typeArguments = _typeArguments.TakeWhile(t => t != null).ToArray();

            if (_closedType == null)
            {
                if (type == null)
                {
                    // resolve using type name
                    if (!(serviceProvider.GetService(typeof(IXamlTypeResolver)) is IXamlTypeResolver typeResolver))
                    {
                        throw new InvalidOperationException("Cannot retrieve IXamlTypeResolver.");
                    }

                    // check that the number of generic arguments match
                    string typeName = _typeName;
                    if (typeArguments.Length > 0)
                    {
                        int genericsMarkerIndex = typeName.LastIndexOf('`');
                        if (genericsMarkerIndex < 0)
                        {
                            typeName = string.Format("{0}`{1}", typeName, typeArguments.Length);
                        }
                        else
                        {
                            bool validArgumentCount = false;
                            if (genericsMarkerIndex < typeName.Length)
                            {
                                if (int.TryParse(typeName.Substring(genericsMarkerIndex + 1), out int typeArgumentCount))
                                {
                                    validArgumentCount = true;
                                }
                            }

                            if (!validArgumentCount)
                            {
                                throw new InvalidOperationException("Invalid type argument count in type name.");
                            }
                        }
                    }

                    type = typeResolver.Resolve(typeName);
                    if (type == null)
                    {
                        throw new InvalidOperationException("Invalid type name.");
                    }
                }
                else if (type.IsGenericTypeDefinition && type.GetGenericArguments().Length != typeArguments.Length)
                {
                    throw new InvalidOperationException("Invalid type argument count in type.");
                }

                // build closed type
                if (typeArguments.Length > 0 && type.IsGenericTypeDefinition)
                {
                    _closedType = type.MakeGenericType(typeArguments);
                }
                else
                {
                    _closedType = type;
                }
            }

            return _closedType;
        }
    }
}
