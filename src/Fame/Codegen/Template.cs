namespace Fame.Codegen
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Text;

	public class Template
	{
		public static Template Get(string key)
		{
			Template template = AllTemplates[key];

			Debug.Assert(template != null, key);

			return template;
		}

		public static IDictionary<string, Template> AllTemplates
		{
			get
			{
				IDictionary<string, Template> map = new Dictionary<string, Template>();

				using (var input = File.OpenRead("template.txt"))
				{
					using (var reader = new StreamReader(input))
					{

						string line = reader.ReadLine();
						while (true)
						{
							if (line == null)
							{
								break;
							}

							Debug.Assert(line.StartsWith("%%%", StringComparison.Ordinal));

							StringBuilder template = new StringBuilder();
							string name = line.Substring(4).Trim();

							while (true)
							{
								line = reader.ReadLine();
								if (line == null || line.StartsWith("%%%", StringComparison.Ordinal))
								{
									break;
								}

								template.Append(line);
								template.Append('\n');
							}

							map[name] = new Template(name, template.ToString());
						}
					}
				}

				return map;
			}
		}

		public static void Main(params string[] argh)
		{
			Console.Write(AllTemplates);
		}

		private readonly string _template;
		private readonly IDictionary<string, string> _values = new Dictionary<string, string>();

		public Template(string name, string template)
		{
			_template = template;
		}

		public string Apply()
		{
			string result = _template;

			foreach (string key in _values.Keys)
			{
				result = result.Replace("--" + key + "--", _values[key]);
			}

			return result;
		}

		public void Set(string key, string value)
		{
			_values[key] = value;
		}

		public Template All
		{
			set
			{
				foreach (var kv in value._values)
				{
					if (_values.ContainsKey(kv.Key))
					{
						_values[kv.Key] = kv.Value;
					}
					else
					{
						_values.Add(kv.Key, kv.Value);
					}
				}
			}
		}
	}
}