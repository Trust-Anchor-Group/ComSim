using System;
using System.IO;
using System.Xml;

namespace ComSim
{
	/// <summary>
	/// The TAG Network Communication Simulator (or TAG ComSim) is a white-label console
	/// utility application written in C# provided by Trust Anchor Group (TAG for short).
	/// It can be used to simulate network communication traffic in large-scale networks.
	/// 
	/// Command-line arguments:
	/// 
	/// -i FILENAME        Specifies the filename of the model to use during simulation.
	/// -input FILENAME    The file must be an XML file that conforms to the
	/// -m FILENAME        http://trustanchorgroup.com/Schema/ComSim.xsd namespace.
	/// -model FILENAME    Schema is available at Schema/ComSim.xsd in repository.
	/// </summary>
	class Program
	{
		static int Main(string[] args)
		{
			try
			{
				XmlDocument Model = null;
				int i = 0;
				int c = args.Length;
				string s;
				bool Help = args.Length == 0;

				while (i < c)
				{
					s = args[i++].ToLower();

					if (s.StartsWith("/"))
						s = "-" + s.Substring(1);

					switch (s)
					{
						case "-i":
						case "-input":
						case "-m":
						case "-model":
							if (i >= c)
								throw new Exception("Expected model filename.");

							s = args[i++];
							if (!File.Exists(s))
								throw new Exception("File not found: " + s);

							Model = new XmlDocument();
							Model.Load(s);
							break;

						case "-?":
						case "-h":
						case "-help":
							Help = true;
							break;

						default:
							throw new Exception("Unrecognized switch: " + s);
					}
				}

				if (Help)
				{
					Console.Out.WriteLine("The TAG Network Communication Simulator (or TAG ComSim) is a white-label console");
					Console.Out.WriteLine("utility application written in C# provided by Trust Anchor Group (TAG for short).");
					Console.Out.WriteLine("It can be used to simulate network communication traffic in large-scale networks.");
					Console.Out.WriteLine();
					Console.Out.WriteLine("Command-line arguments:");
					Console.Out.WriteLine();
					Console.Out.WriteLine("-i FILENAME        Specifies the filename of the model to use during simulation.");
					Console.Out.WriteLine("-input FILENAME    The file must be an XML file that conforms to the");
					Console.Out.WriteLine("-m FILENAME        http://trustanchorgroup.com/Schema/ComSim.xsd namespace.");
					Console.Out.WriteLine("-model FILENAME    Schema is available at Schema/ComSim.xsd in the repository.");
					Console.Out.WriteLine();

					if (args.Length <= 1)
						return 1;
				}

				if (Model is null)
					throw new Exception("No simulation model specified.");
			}
			catch (Exception ex)
			{
				WriteLine(ex.Message, ConsoleColor.White, ConsoleColor.Red);
				return 1;
			}

			return 0;
		}

		private static void WriteLine(string Row, ConsoleColor ForegroundColor, ConsoleColor BackgrounColor)
		{
			ConsoleColor ForegroundColorBak = Console.ForegroundColor;
			ConsoleColor BackgroundColorBak = Console.BackgroundColor;

			Console.ForegroundColor = ForegroundColor;
			Console.BackgroundColor = BackgrounColor;

			Console.Out.WriteLine(Row);

			Console.ForegroundColor = ForegroundColorBak;
			Console.BackgroundColor = BackgroundColorBak;
		}

	}
}
