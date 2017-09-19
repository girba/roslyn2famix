namespace Fame.Internal
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using Common;

	public class MethodAccess : Access
	{
		private readonly MethodInfo _getter;
		private readonly MethodInfo _setter;

		public MethodAccess(MethodInfo m) : base(m.ReturnType)
		{
			_getter = m;
			//this.getter.Accessible = true;  // TODO: In C#?
			_setter = SetterMethod();
		}

		public MethodAccess(MethodInfo getter, MethodInfo setter) : base(getter.ReturnType)
		{
			_getter = getter;
			//this.getter.Accessible = true;  // TODO: In C#?
			_setter = setter;
			//this.setter.setAccessible(true);  // TODO: In C#?
		}

		public override FamePropertyAttribute Annotation
		{
			get
			{
				Debug.Assert(_getter.GetParameters().Length == 0, _getter.Name);

				return (FamePropertyAttribute)_getter.GetCustomAttribute(typeof(FamePropertyAttribute));

			}
		}

		public override string Name => _getter.Name;

		public override object Read(object element)
		{
			return _getter.Invoke(element, null);
		}

		private MethodInfo SetterMethod()
		{
			if (Annotation.Derived)
			{
				return null;
			}

			string setterName = SetterName();

			try
			{
				MethodInfo m = _getter.DeclaringType?.GetMethod(setterName, new[] {_getter.ReturnType});
				//m.Accessible = true;  // TODO: In C#?

				return m;
			}
			catch (AmbiguousMatchException ex)
			{
				throw new AssertionError(ex);
			}
		}

		private string SetterName()
		{
			// TODO: Verify
			string name = _getter.Name;

			if (name.StartsWith("is", StringComparison.Ordinal))
			{
				name = char.ToLower(name[2]) + name.Substring(3);
			}

			if (name.StartsWith("get", StringComparison.Ordinal))
			{
				name = char.ToLower(name[3]) + name.Substring(4);
			}

			return "set" + char.ToUpper(name[0]) + name.Substring(1);
		}

		public override void Write(object element, object value)
		{
			try
			{
				_setter.Invoke(element, new []{ value } );
			}
			catch (ArgumentException ex)
			{
				throw new AssertionError(ex);
			}
			catch (MethodAccessException ex)
			{
				throw new AssertionError(ex);
			}
			catch (TargetInvocationException ex)
			{
				throw new AssertionError(ex);
			}
		}
	}
}