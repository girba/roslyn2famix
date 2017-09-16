using System;
using System.Reflection;

namespace Fame.Internal
{
	// TODO
	public class MethodAccess : Access
	{
		public MethodAccess(Type type) : base(type)
		{
		}

		public MethodAccess(MethodInfo m) : base(m.ReturnType)
		{
		}

		public override FamePropertyAttribute Annotation { get; }
		public override string Name { get; }
		public override object Read(object element)
		{
			throw new NotImplementedException();
		}

		public override void Write(object element, object value)
		{
			throw new NotImplementedException();
		}
	}
}
