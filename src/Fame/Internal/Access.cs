namespace Fame.Internal
{
	using System;
	using System.Diagnostics;
	using System.Collections;
	using System.Collections.Generic;
	using Common;

	public abstract class Access
	{
		private readonly Type _containerType;

		public abstract FamePropertyAttribute Annotation { get; }
		public abstract string Name { get; }
		public abstract object Read(object element);
		public abstract void Write(object element, object value);

		public Type ElementType { get; }
		public bool AnnotationPresent => Annotation != null;
		public bool Multivalued => _containerType != null;
		public bool Array => _containerType != null && _containerType.IsArray;
		public bool Iterable => _containerType != null && _containerType.IsSubclassOf(typeof(IEnumerable));
		public bool Collection => _containerType != null && _containerType.IsSubclassOf(typeof(ICollection));

		public Access(Type type)
		{
			if (type.IsClass || type.IsValueType) // Remark: In C# boolean is not a class --> IsValueType
			{
				if (type.IsSubclassOf(typeof(ICollection)))
				{
					ElementType = typeof(object);
					_containerType = type;
				}
				else if (type.IsArray)
				{
					ElementType = type.GetElementType();
					_containerType = type;
				}
				else
				{
					ElementType = type;
					_containerType = null;
				}
			}
			else if (type.IsGenericType)
			{
				Type paraType = type;
				Type rawClass = paraType.GetGenericTypeDefinition();

				Debug.Assert(rawClass != null);
				Debug.Assert(rawClass.IsAssignableFrom(typeof(IEnumerable<>))); // TODO: Assert crashes because of ISet<>
				Debug.Assert(paraType.GetGenericArguments().Length == 1);

				_containerType = rawClass;

				Type typeArgument = paraType.GetGenericArguments()[0];

				if (typeArgument.IsClass)
				{
					ElementType = typeArgument;
				}
				// No equivalence in C# for the generic type parameter WildcardType
				//else if (typeArgument instanceof WildcardType)
				//{
				//	WildcardType wildcard = (WildcardType)typeArgument;
				//	assert wildcard.getLowerBounds().length == 0;
				//	assert wildcard.getUpperBounds().length == 1;
				//	assert wildcard.getUpperBounds()[0] instanceof Class;
				//	elementType = (Class <?>) wildcard.getUpperBounds()[0];
				//}
				else
				{
					throw new AssertionError();
				}
			}
			else
			{
				throw new AssertionError();
			}
		}
	}
}