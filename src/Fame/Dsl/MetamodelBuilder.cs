namespace Fame.Dsl
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Fm3;
	using Parser;

	public class MetamodelBuilder
	{
		public interface IDocument
		{
			IPackage BeginPackage(string name);
			void EndDocument();
		}

		public interface IPackage
		{
			IClass BeginClass(string name);
			IDocument EndPackage();
			void EndDocument();
			IPackage BeginPackage(string @string);
		}

		public interface IClass
		{
			IClass With(string name, string type, string opposite);
			IClass With(string name, string type);
			IClass WithMany(string name, string type, string opposite);
			IClass WithMany(string name, string type);
			IPackage EndClass();
			IClass BeginClass(string @string);
			void EndDocument();
			IPackage BeginPackage(string @string);
		}

		private sealed class DocumentImpl : IDocument
		{
			private readonly IParseClient _client;
			private readonly Func<string, int> _toFunc;

			public DocumentImpl(IParseClient client, Func<string, int> toFunc)
			{
				_client = client;
				_toFunc = toFunc;
				client.BeginDocument();
			}

			public IPackage BeginPackage(string name)
			{
				return new PackageImpl(name, _client, this);
			}

			public void EndDocument()
			{
				_client.EndDocument();
			}

			private sealed class PackageImpl : IPackage
			{
				private readonly string _packageName;
				private readonly IParseClient _client;
				private readonly DocumentImpl _document;

				public PackageImpl(string name, IParseClient client, DocumentImpl document)
				{
					Debug.Assert(name != null);

					_client = client;
					_document = document;

					client.BeginElement("FM3.Package");
					client.BeginAttribute("name");
					client.Primitive(_packageName = name);
					client.EndAttribute("name");
					client.BeginAttribute("classes");
				}

				public IClass BeginClass(string name)
				{
					return new ClassImpl(name, _client, this);
				}

				public IDocument EndPackage()
				{
					_client.EndAttribute("classes");
					_client.EndElement("FM3.Package");

					return _document;
				}

				private sealed class ClassImpl : IClass
				{
					private readonly string _className;
					private readonly IParseClient _client;
					private readonly PackageImpl _package;

					public ClassImpl(string name, IParseClient client, PackageImpl package)
					{
						Debug.Assert(name != null);

						_client = client;
						_package = package;

						client.BeginElement("FM3.Class");
						client.Serial(package._document._toFunc(package._packageName + "." + name));
						client.BeginAttribute("name");
						client.Primitive(_className = name);
						client.EndAttribute("name");
						client.BeginAttribute("attributes");
					}

					private IClass With(string name, string type, string opposite, bool multivalued)
					{
						_client.BeginElement("FM3.Property");
						_client.Serial(_package._document._toFunc(_package._packageName + "." + _className + "." + name));
						_client.BeginAttribute("name");
						_client.Primitive(name);
						_client.EndAttribute("name");
						_client.BeginAttribute("type");
						TypeOfProperty(type);
						_client.EndAttribute("type");

						if (opposite != null)
						{
							_client.BeginAttribute("opposite");
							_client.Reference(_package._document._toFunc(type + "." + opposite));
							_client.EndAttribute("opposite");
						}

						if (multivalued)
						{
							_client.BeginAttribute("multivalued");
							_client.Primitive(true);
							_client.EndAttribute("multivalued");
						}

						_client.EndElement("FM3.Property");

						return this;
					}

					private void TypeOfProperty(string type)
					{
						if (MetaDescription.HasPrimitiveNamed(type))
						{
							_client.Reference(type);
						}
						else
						{
							_client.Reference(_package._document._toFunc(type));
						}
					}

					public IClass With(string name, string type)
					{
						return With(name, type, null, false);
					}

					public IClass WithMany(string name, string type, string opposite)
					{
						return With(name, type, opposite, true);
					}

					public IClass WithMany(string name, string type)
					{
						return With(name, type, null, true);
					}

					public IClass With(string name, string type, string opposite)
					{
						return With(name, type, opposite, false);
					}

					public IPackage EndClass()
					{
						_client.EndAttribute("attributes");
						_client.EndElement("FM3.Class");

						return _package;
					}

					public IClass BeginClass(string name)
					{
						return EndClass().BeginClass(name);
					}

					public void EndDocument()
					{
						EndClass().EndDocument();
					}

					public IPackage BeginPackage(string name)
					{
						return EndClass().EndPackage().BeginPackage(name);
					}
				}

				public void EndDocument()
				{
					EndPackage().EndDocument();
				}

				public IPackage BeginPackage(string name)
				{
					return EndPackage().BeginPackage(name);
				}
			}
		}

		private readonly IParseClient _client;
		private readonly Dictionary<string, int> _indexDict;

		public MetamodelBuilder(IParseClient client)
		{
			_client = client;
			_indexDict = new Dictionary<string, int>();
		}

		public virtual IDocument BeginDocument()
		{
			return new DocumentImpl(_client, To);
		}

		private int To(string name)
		{
			if (_indexDict.ContainsKey(name))
				return _indexDict[name];

			var key = _indexDict.Count;
			_indexDict.Add(name, key);

			return key;
		}

		public static void Main(string[] args)
		{
			IParseClient pc = new DebugClient();
			new MetamodelBuilder(pc)
				.BeginDocument()
					.BeginPackage("RPG")
						.BeginClass("Dragon")
							.WithMany("hoard", "RPG.Treasure", "keeper")
							.WithMany("killedBy", "RPG.Hero", "kills")
						.BeginClass("Treasure")
							.With("keeper", "RPG.Dragon", "hoard")
							.With("owner", "RPG.Hero", "talisman")
						.BeginClass("Hero")
							.With("twin", "RPG.Hero", "twin")
							.With("talisman", "RPG.Treasure", "owner")
							.WithMany("kills", "RPG.Dragon", "killedBy")
				.EndDocument();

			//        m = new MetamodelBuilder(pc)
			//
			//        m.beginDocument(
			//            m.beginPackage("RPG",
			//                m.beginClass("Dragon",
			//                    m.withMany("hoard", "RPG.Treasure", "keeper"),
			//                    m.withMany("killedBy", "RPG.Hero", "kills")),
			//                m.beginClass("Treasure",
			//                    m.with("keeper", "RPG.Dragon", "hoard"),
			//                    m.with("owner", "RPG.Hero", "talisman")),
			//                m.beginClass("Hero",
			//                    m.with("twin", "RPG.Hero", "twin"),
			//                    m.with("talisman", "RPG.Treasure", "owner"),
			//                    m.withMany("kills", "RPG.Dragon", "killedBy"))));

			Console.WriteLine(pc);
		}
	}
}