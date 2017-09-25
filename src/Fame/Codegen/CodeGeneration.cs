namespace Fame.Codegen
{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using Common;
	using Fm3;
	using Internal;

	public class CodeGeneration
	{
		public static readonly ICollection<string> Keywords = new [] { "abstract", "continue", "for", "new", "switch", "Assert", "default", 
			"goto", "namespace", "synchronized", "bool", "do", "if", "private", "this", "break", "double", "protected",
			"throw", "byte", "else", "using", "public", "case", "enum", "is", "return", "catch",
			"int", "short", "try", "char", "readonly", "interface", "static", "void", "class", "finally", "long",
			"volatile", "const", "float", "base", "while" };

		public static string AsJavaSafeName(string name)
		{
			if (Keywords.Contains(name))
			{
				return "my" + name.ToUpperFirstChar();
			}

			return name;
		}

		private readonly string _destinationPackage = "com.example";
		private readonly string _outputDirectory;
		private readonly string _classNamePrefix = "";
		private JavaFile _code;
		private string _folder;

		public CodeGeneration(string outputDirectory)
		{
			_outputDirectory = outputDirectory;
		}

		public CodeGeneration(string destinationPackage, string outputDirectory, string classNamePrefix)
		{
			_classNamePrefix = classNamePrefix;
			_destinationPackage = destinationPackage;
			_outputDirectory = outputDirectory;
		}

		public void Accept(MetaRepository metamodel)
		{
			foreach (PackageDescription each in metamodel.AllPackageDescriptions())
			{
				AcceptPackage(each);
			}
		}

		public void Accept(Repository repo)
		{

			foreach (PackageDescription each in repo.All<PackageDescription>())
			{
				AcceptPackage(each);
			}
		}

		private void AcceptAccessorProperty(PropertyDescription m)
		{
			_code.AddImport(typeof(FamePropertyAttribute));

			string typeName = "object";

			if (m.Type != null)
			{ // TODO should not have null type
				typeName = ClassName(m.Type);
				//code.AddImport(this.packageName(m.Type.Assembly), typeName);
				// TODO: namespace!?
			}

			if (m.IsMultivalued)
			{
				_code.AddImport("java.util", "*"); // TODO
			}

			string myName = AsJavaSafeName(m.Name);
			string @base = m.IsMultivalued ? "Many" : "One";

			Template field = Template.Get(@base + ".Field");

			if (m.Opposite != null)
			{
				@base = @base + (m.Opposite.IsMultivalued ? "Many" : "One");

				if (@base.Equals("ManyOne") || @base.Equals("ManyMany"))
				{
					_code.AddImport(typeof(MultivalueSet<>));
				}
			}

			Template getter = Template.Get(@base + ".Getter");
			Template setter = Template.Get(@base + ".Setter");

			field.Set("TYPE", typeName);
			field.Set("THISTYPE", AsJavaSafeName(ClassName(m.OwningMetaDescription)));
			field.Set("FIELD", myName);
			field.Set("NAME", m.Name);
			field.Set("GETTER", "get" + char.ToUpper(myName[0]) + myName.Substring(1));
			field.Set("SETTER", "set" + char.ToUpper(myName[0]) + myName.Substring(1));

			if (m.Opposite != null)
			{
				string oppositeName = m.Opposite.Name;

				field.Set("OPPOSITENAME", oppositeName);

				oppositeName = AsJavaSafeName(oppositeName);

				field.Set("OPPOSITESETTER", "set" + char.ToUpper(oppositeName[0]) + oppositeName.Substring(1));
				field.Set("OPPOSITEGETTER", "get" + char.ToUpper(oppositeName[0]) + oppositeName.Substring(1));

			}

			getter.All = field;
			setter.All = field;

			string props = "";

			if (m.IsDerived)
			{
				props += ", derived = true";
			}

			if (m.IsContainer)
			{
				props += ", container = true";
			}

			getter.Set("PROPS", props);

			StringBuilder stream = _code.ContentStream;
			stream.Append(field.Apply());
			stream.Append(getter.Apply());
			stream.Append(setter.Apply());

			// adder for multivalued properties
			if (!m.IsMultivalued)
			{
				return;
			}

			Template adder = Template.Get("Many.Sugar");

			adder.Set("TYPE", typeName);
			adder.Set("FIELD", myName);
			adder.Set("GETTER", "get" + char.ToUpper(myName[0]) + myName.Substring(1));
			adder.Set("ADDER", "add" + char.ToUpper(myName[0]) + myName.Substring(1));
			adder.Set("NUMOF", "numberOf" + char.ToUpper(myName[0]) + myName.Substring(1));
			adder.Set("HAS", "has" + char.ToUpper(myName[0]) + myName.Substring(1));

			stream.Append(adder.Apply());
		}

		//private void AcceptDerivedProperty(PropertyDescription m)
		//{
			//        assert m.isDerived() && !m.hasOpposite();
			//        code.addImport(FameProperty.class);
			//        String typeName = "Object";
			//        if (m.getType() != null) { // TODO should not have null type
			//            typeName = className(m.getType());
			//            code.addImport(this.packageName(m.getType().getPackage()), typeName);
			//        }
			//        if (m.isMultivalued()) {
			//            code.addImport("java.util", "*");
			//        }
			//        String myName = CodeGeneration.asJavaSafeName(m.getName());
			//
			//        String base = m.isMultivalued() ? "Many" : "One";
			//        Template getter = Template.get(base + ".Derived.Getter");
			//
			//        getter.set("TYPE", typeName);
			//        getter.set("NAME", m.getName());
			//        getter.set("GETTER", "get" + Character.toUpperCase(myName.charAt(0)) + myName.substring(1));
			//
			//        String props = "";
			//        if (m.isDerived()) {
			//            props += ", derived = true";
			//        }
			//        if (m.isContainer()) {
			//            props += ", container = true";
			//        }
			//        getter.set("PROPS", props);
			//
			//        StringBuilder stream = code.getContentStream();
			//        stream.append(getter.apply());

		//	return;
		//}

		private void AcceptClass(MetaDescription m)
		{
			if (m.IsPrimitive())
			{
				return;
			}

			// TODO: Check
			//code = new JavaFile(this.packageName(m.Assembly), className(m));
			//code.ModelPackagename = m.Assembly.Fullname;			
			_code = new JavaFile(PackageName(m.Package), ClassName(m))
			{
				ModelPackagename = m.Package.Fullname,
				ModelClassname = m.Name
			};

			_code.AddImport(typeof(FameDescriptionAttribute));
			_code.AddImport(typeof(FamePackageAttribute));

			if (m.BaseClass != null)
			{
				// TODO: Check
				//code.AddSuperclass(this.packageName(m.BaseType.Assembly), className(m.BaseClass));
				_code.AddSuperclass(PackageName(m.Package), ClassName(m.SuperClass));
			}

			foreach (PropertyDescription property in m.Attributes)
			{
				AcceptProperty(property);
			}

			using (var stream2 = File.Create(Path.Combine(_folder, ClassName(m) + ".cs")))
			{
				using (var writer = new StreamWriter(stream2))
				{
					_code.GenerateCode(writer);
				}
			}
		}

		private void AcceptPackage(PackageDescription m)
		{
			_folder = Path.Combine(_outputDirectory, PackageName(m).Replace('.', '\\'));
			if (!Directory.Exists(_folder))
			{
				Directory.CreateDirectory(_folder);
			}

			foreach (MetaDescription meta in m.Elements)
			{
				AcceptClass(meta);
			}

			string name = m.Name.ToUpperFirstChar() + "Model";

			Template template = Template.Get("Package");

			string packageName = PackageName(m);
			template.Set("PACKAGE", packageName);
			template.Set("MODEL", name);
			template.Set("AUTOGENCODE", "Automagically generated code");

			StringBuilder builder = new StringBuilder();

			foreach (MetaDescription meta in m.Elements)
			{
				builder.Append("\t\tmetamodel.with(");
				builder.Append(packageName);
				builder.Append('.');
				builder.Append(ClassName(meta));
				builder.Append(".class);\n");
			}

			template.Set("ADDCLASSES", builder.ToString());

			var file =  Path.Combine(_folder, name + ".cs");

			using (var stream = new StreamWriter(file))
			{
				stream.Write(template.Apply());
			}

			_folder = null;
		}

		private void AcceptProperty(PropertyDescription m)
		{
			if (m.IsDerived && !m.HasOpposite())
			{
				//AcceptDerivedProperty(m);
				return;
			}

			AcceptAccessorProperty(m);
		}

		private string ClassName(MetaDescription meta)
		{
			if (meta.IsPrimitive() || meta.IsRoot())
			{
				return meta.Name;
			}

			return MapClassName(meta.Name);
		}

		public string DestinationPackage()
		{
			return _destinationPackage;
		}

		public string MapClassName(string name)
		{
			return _classNamePrefix + name;
		}

		public string MapPackageName(string name)
		{
			return name.ToLower();
		}

		public string OutputDirectory()
		{
			return _outputDirectory;
		}

		private string PackageName(PackageDescription m)
		{
			if (m == null)
			{
				return "System";
			}

			return DestinationPackage() + "." + MapPackageName(m.Name);
		}
	}
}