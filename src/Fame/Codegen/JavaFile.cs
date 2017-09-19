using System;
using System.IO;

namespace Fame.Codegen
{
	using System.Collections.Generic;
	using System.Text;

	public class JavaFile  // TODO: CsharpFile
	{
		// TODO: change names to match C# semantics (e.g. import = using)
		private readonly HashSet<string> _imports;
		private readonly string _myPackage;
		private readonly string _name;
		private string _superName;
		private string _modelPackagename;
		private string _modelClassname;

		public JavaFile(string myPackage, string name)
		{
			_myPackage = myPackage;
			_name = name;
			ContentStream = new StringBuilder();
			_imports = new HashSet<string>();
		}

		// TODO: Verify
		public void AddImport(Type tee)
		{
			AddImport(tee.Assembly.GetName().Name, tee.Name);

			//public <T> void addImport(Class<T> tee) {
			//	this.addImport(tee.getPackage().getName(), tee.getSimpleName());
			//}
		}

		public void AddImport(string aPackage, string className)
		{
			if (aPackage.Equals(_myPackage))
			{
				return;
			}

			if (aPackage.Equals("System"))  // TODO: C# - System.* ?
			{
				return;
			}

			_imports.Add(aPackage + "." + className);  // TODO: C# ?
		}

		public void AddSuperclass(string aPackage, string className)
		{
			if (className.Equals("object") && aPackage.Equals("System"))
			{
				return;
			}

			AddImport(aPackage, className);
			_superName = className;
		}

		public void GenerateCode(StreamWriter stream)
		{
			Template template = Template.Get("Class");

			template.Set("PACKAGE", _myPackage);

			template.Set("AUTOGENCODE", "Automagically generated code");

			template.Set("THISTYPE", _name);

			template.Set("THISPACKAGE", _modelPackagename);

			template.Set("THISNAME", _modelClassname);

			template.Set("EXTENDS", _superName == null ? "" : "extends " + _superName);

			template.Set("IMPORTS", Imports);

			template.Set("FIELDS", "");

			template.Set("METHODS", ContentStream.ToString());

			stream.Write(template.Apply());
		}

		public StringBuilder ContentStream { get; }

		public string Imports
		{
			get
			{
				StringBuilder stream = new StringBuilder();

				foreach (string each in _imports)
				{
					stream.Append("import ").Append(each).Append(";\n");
				}

				return stream.ToString();
			}
		}

		public string ModelClassname
		{
			get => _modelClassname;
			set => _modelClassname = value;
		}

		public string ModelPackagename
		{
			get => _modelPackagename;
			set => _modelPackagename = value;
		}
	}
}