using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using TAG.Simulator;
using TAG.Simulator.Events;
using Waher.Content;
using Waher.Content.Xml;
using Waher.Content.Xsl;
using Waher.Events;
using Waher.Events.Console;
using Waher.Events.Files;
using Waher.Events.Persistence;
using Waher.Persistence;
using Waher.Persistence.Files;
using Waher.Runtime.Inventory;
using Waher.Runtime.Inventory.Loader;

namespace ComSim
{
	/// <summary>
	/// The TAG Network Communication Simulator (or TAG ComSim) is a white-label console
	/// utility application written in C# provided by Trust Anchor Group (TAG for short).
	/// It can be used to simulate network communication traffic in large-scale networks.
	/// 
	/// Command-line arguments:
	/// 
	/// -i FILENAME           Specifies the filename of the model to use during simulation.
	///                       The file must be an XML file that conforms to the
	///                       http://lab.tagroot.io/Schema/ComSim.xsd namespace.
	///                       Schema is available at Schema/ComSim.xsd in repository.
	/// -l LOG_FILENAME       Redirects logged events to a log file.
	/// -lt LOG_TRANSFORM     File name of optional XSLT transform for use with log file.
	/// -lc                   Log events to the console.
	/// -s SNIFFER_FOLDER     Optional folder for storing network sniff files.
	/// -st SNIFFER_TRANSFORM File name of optional XSLT transform for use with sniffers.
	/// -d APP_DATA_FOLDER    Points to the application data folder. Required if storage
	///                       of data in a local database is necessary for the 
	///                       simulation. (Storage can include generated user credentials
	///                       so that the same user identities can be used across
	///                       simulations.)
	/// -af FOLDER            Adds an assembly folder. Assemblies can be loaded from this
	///                       folder.
	/// -e                    If encryption is used by the database. Default=no encryption.
	/// -bs BLOCK_SIZE        Block size, in bytes. Default=8192.
	/// -bbs BLOB_BLOCK_SIZE  BLOB block size, in bytes. Default=8192.
	/// -enc ENCODING         Text encoding. Default=UTF-8
	/// -mr FILENAME          Generates a Markdown Report file after simulation.
	/// -xr FILENAME          Generates an XML report file after simulation.
	/// -master RELNAME       Adds a Master file declaration to the top of markdown
	///                       reports. The reference must be relative to the generated
	///                       report file.
	/// -css RELNAME          Adds a CSS file declaration to the top of markdown
	///                       reports. The reference must be relative to the generated
	///                       report file.
	/// -ik KEYNAME FILENAME  Import keys from a CSV file. The CSV file must consist of
	///                       two columns. The first, contains lookup values, the second,
	///                       the key values corresponding to the lookup values. The
	///                       KEYNAME argument defines the key name to which the keys
	///                       are associated. FILENAME must point to a CSV file.
	/// -?                    Displays command-line help.
	/// </summary>
	class Program
	{
		private static readonly Dictionary<string, Dictionary<string, string>> importedKeys = [];

		static int Main(string[] args)
		{
			try
			{
				StringBuilder CommandLine = new("ComSim.exe");
				LinkedList<string> Master = [];
				LinkedList<string> Css = [];
				LinkedList<string> AssemblyFolders = [];
				Encoding Encoding = Encoding.UTF8;
				XmlDocument Model = null;
				string ProgramDataFolder = null;
				string SnifferFolder = null;
				string LogFileName = null;
				string LogTransformFileName = null;
				string SnifferTransformFileName = null;
				string XmlOutputFileName = null;
				string MarkdownOutputFileName = null;
				int i;
				int c = args.Length;
				int BlockSize = 8192;
				int BlobBlockSize = 8192;
				bool Encryption = false;
				string s;
				bool Help = args.Length == 0;
				bool LogConsole = false;
				bool Quote;

				for (i = 0; i < c; i++)
				{
					CommandLine.Append(' ');

					s = args[i];
					Quote = s.Contains(' ');
					if (Quote)
						CommandLine.Append('"');

					CommandLine.Append(s);

					if (Quote)
						CommandLine.Append('"');
				}

				i = 0;
				while (i < c)
				{
					s = args[i++].ToLower();
					s = s.ToLower();

					if (s.StartsWith('/'))
						s = "-" + s[1..];

					switch (s)
					{
						case "-i":
							if (i >= c)
								throw new Exception("Expected model filename.");

							s = args[i++];
							if (!File.Exists(s))
								throw new Exception("File not found: " + s);

							Model = new XmlDocument();
							Model.Load(s);
							break;

						case "-l":
							if (i >= c)
								throw new Exception("Missing log file name.");

							if (string.IsNullOrEmpty(LogFileName))
								LogFileName = args[i++];
							else
								throw new Exception("Only one log file name allowed.");
							break;

						case "-lt":
							if (i >= c)
								throw new Exception("Missing log transform file name.");

							if (string.IsNullOrEmpty(LogTransformFileName))
								LogTransformFileName = args[i++];
							else
								throw new Exception("Only one log transform file name allowed.");
							break;

						case "-lc":
							LogConsole = true;
							break;

						case "-s":
							if (i >= c)
								throw new Exception("Missing sniffer folder.");

							if (string.IsNullOrEmpty(SnifferFolder))
								SnifferFolder = args[i++];
							else
								throw new Exception("Only one sniffer folder allowed.");
							break;

						case "-st":
							if (i >= c)
								throw new Exception("Missing sniffer transform file name.");

							if (string.IsNullOrEmpty(SnifferTransformFileName))
								SnifferTransformFileName = args[i++];
							else
								throw new Exception("Only one sniffer transform file name allowed.");
							break;

						case "-d":
							if (i >= c)
								throw new Exception("Missing program data folder.");

							if (string.IsNullOrEmpty(ProgramDataFolder))
								ProgramDataFolder = args[i++];
							else
								throw new Exception("Only one program data folder allowed.");
							break;

						case "-bs":
							if (i >= c)
								throw new Exception("Block size missing.");

							if (!int.TryParse(args[i++], out BlockSize))
								throw new Exception("Invalid block size");

							break;

						case "-bbs":
							if (i >= c)
								throw new Exception("Blob Block size missing.");

							if (!int.TryParse(args[i++], out BlobBlockSize))
								throw new Exception("Invalid blob block size");

							break;

						case "-enc":
							if (i >= c)
								throw new Exception("Text encoding missing.");

							Encoding = Encoding.GetEncoding(args[i++]);
							break;

						case "-e":
							Encryption = true;
							break;

						case "-mr":
							if (i >= c)
								throw new Exception("Missing markdown report file name.");

							if (string.IsNullOrEmpty(MarkdownOutputFileName))
								MarkdownOutputFileName = args[i++];
							else
								throw new Exception("Only one markdown report file name allowed.");
							break;

						case "-xr":
							if (i >= c)
								throw new Exception("Missing XML report file name.");

							if (string.IsNullOrEmpty(XmlOutputFileName))
								XmlOutputFileName = args[i++];
							else
								throw new Exception("Only one XML report file name allowed.");
							break;

						case "-master":
							if (i >= c)
								throw new Exception("Missing master file name.");

							Master.AddLast(args[i++]);
							break;

						case "-css":
							if (i >= c)
								throw new Exception("Missing CSS file name.");

							Css.AddLast(args[i++]);
							break;

						case "-ik":
							if (i >= c)
								throw new Exception("Missing key name.");

							string KeyName = args[i++];

							if (i >= c)
								throw new Exception("Missing CSV filename.");

							string CsvFileName = args[i++];
							string Csv = File.ReadAllText(CsvFileName);
							Dictionary<string, string> Lookup;

							if (!importedKeys.TryGetValue(KeyName, out Lookup))
							{
								Lookup = [];
								importedKeys[KeyName] = Lookup;
							}

							string[][] Records = CSV.Parse(Csv);

							foreach (string[] Record in Records)
							{
								if (Record.Length != 2)
									throw new Exception("CSV file must contain records with two columns: Lookup, and value.");

								Lookup[Record[0]] = Record[1];
							}

							break;

						case "-af":
							if (i >= c)
								throw new Exception("Missing Assembly folder.");

							AssemblyFolders.AddLast(args[i++]);
							break;

						case "-?":
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
					Console.Out.WriteLine("-i FILENAME           Specifies the filename of the model to use during simulation.");
					Console.Out.WriteLine("                      The file must be an XML file that conforms to the");
					Console.Out.WriteLine("                      http://lab.tagroot.io/Schema/ComSim.xsd namespace.");
					Console.Out.WriteLine("                      Schema is available at Schema/ComSim.xsd in the repository.");
					Console.Out.WriteLine("-l LOG_FILENAME       Redirects logged events to a log file.");
					Console.Out.WriteLine("-lt LOG_TRANSFORM     File name of optional XSLT transform for use with log file.");
					Console.Out.WriteLine("-lc                   Log events to the console.");
					Console.Out.WriteLine("-s SNIFFER_FOLDER     Optional folder for storing network sniff files.");
					Console.Out.WriteLine("-st SNIFFER_TRANSFORM File name of optional XSLT transform for use with sniffers.");
					Console.Out.WriteLine("-d APP_DATA_FOLDER    Points to the application data folder. Required if storage");
					Console.Out.WriteLine("                      of data in a local database is necessary for the");
					Console.Out.WriteLine("                      simulation. (Storage can include generated user credentials");
					Console.Out.WriteLine("                      so that the same user identities can be used across");
					Console.Out.WriteLine("                      simulations.)");
					Console.Out.WriteLine("-af FOLDER            Adds an assembly folder. Assemblies can be loaded from this");
					Console.Out.WriteLine("                      folder.");
					Console.Out.WriteLine("-e                    If encryption is used by the database. Default=no encryption.");
					Console.Out.WriteLine("-bs BLOCK_SIZE        Block size, in bytes. Default=8192.");
					Console.Out.WriteLine("-bbs BLOB_BLOCK_SIZE  BLOB block size, in bytes. Default=8192.");
					Console.Out.WriteLine("-enc ENCODING         Text encoding. Default=UTF-8");
					Console.Out.WriteLine("-mr FILENAME          Generates a Markdown Report file after simulation.");
					Console.Out.WriteLine("-xr FILENAME          Generates an XML report file after simulation.");
					Console.Out.WriteLine("-master RELNAME       Adds a Master file declaration to the top of markdown");
					Console.Out.WriteLine("                      reports. The reference must be relative to the generated");
					Console.Out.WriteLine("                      report file.");
					Console.Out.WriteLine("-css RELNAME          Adds a CSS file declaration to the top of markdown");
					Console.Out.WriteLine("                      reports. The reference must be relative to the generated");
					Console.Out.WriteLine("                      report file.");
					Console.Out.WriteLine("-ik KEYNAME FILENAME  Import keys from a CSV file. The CSV file must consist of");
					Console.Out.WriteLine("                      two columns. The first, contains lookup values, the second,");
					Console.Out.WriteLine("                      the key values corresponding to the lookup values. The");
					Console.Out.WriteLine("                      KEYNAME argument defines the key name to which the keys");
					Console.Out.WriteLine("                      are associated. FILENAME must point to a CSV file.");
					Console.Out.WriteLine("-?                    Displays command-line help.");
					Console.Out.WriteLine();

					if (args.Length <= 1)
						return 1;
				}

				if (Model is null)
					throw new Exception("No simulation model specified.");

				if (string.IsNullOrEmpty(ProgramDataFolder))
					throw new Exception("No program data folder set");

				Console.Out.WriteLine("Loading modules.");

				foreach (XmlNode N in Model.DocumentElement.ChildNodes)
				{
					if (N is XmlElement E && E.LocalName == "Assemblies")
					{
						Dictionary<string, Assembly> Loaded = [];

						foreach (Assembly A in AppDomain.CurrentDomain.GetAssemblies())
							Loaded[A.GetName().Name] = A;

						foreach (XmlNode N2 in E.ChildNodes)
						{
							if (N2 is XmlElement E2 && E2.LocalName == "Assembly")
							{
								string FileName = XML.Attribute(E2, "fileName");

								if (!File.Exists(FileName))
								{
									string FileName2 = null;

									foreach (string Folder in AssemblyFolders)
									{
										FileName2 = Path.Combine(Folder, FileName);
										if (File.Exists(FileName2))
											break;
										else
											FileName2 = null;
									}

									if (FileName2 is null)
										throw new Exception("File not found: " + FileName);
									else
										FileName = FileName2;
								}

								LinkedList<string> ToLoad = [];
								ToLoad.AddLast(FileName);

								while (!string.IsNullOrEmpty(FileName = ToLoad.First?.Value))
								{
									ToLoad.RemoveFirst();

									Console.Out.WriteLine("Loading " + FileName);

									byte[] Bin = File.ReadAllBytes(FileName);
									try
									{
										Assembly A = AppDomain.CurrentDomain.Load(Bin);
										Loaded[A.GetName().Name] = A;

										AssemblyName[] Referenced = A.GetReferencedAssemblies();

										foreach (AssemblyName AN in Referenced)
										{
											if (Loaded.ContainsKey(AN.Name))
												continue;

											string RefFileName = Path.Combine(Path.GetDirectoryName(FileName), AN.Name + ".dll");

											if (!File.Exists(RefFileName))
												continue;

											if (!ToLoad.Contains(RefFileName))
												ToLoad.AddLast(RefFileName);
										}
									}
									catch (BadImageFormatException)
									{
										// Ignore
									}
								}
							}
						}

					}
				}

				if (!string.IsNullOrEmpty(SnifferFolder))
				{
					if (!Directory.Exists(SnifferFolder))
						Directory.CreateDirectory(SnifferFolder);

					foreach (string FileName in Directory.GetFiles(SnifferFolder, "*.xml", SearchOption.TopDirectoryOnly))
						File.Delete(FileName);
				}

				if (!Directory.Exists(ProgramDataFolder))
					Directory.CreateDirectory(ProgramDataFolder);

				Console.Out.WriteLine("Initializing runtime inventory.");
				TypesLoader.Initialize();
				Factory.Initialize();

				Console.Out.WriteLine("Validating model.");

				Dictionary<string, XmlSchema> Schemas = [];
				LinkedList<XmlElement> ToProcess = [];
				XmlElement Loop;
				string Last = null;

				ToProcess.AddLast(Model.DocumentElement);

				while ((Loop = ToProcess.First?.Value) is not null)
				{
					ToProcess.RemoveFirst();

					foreach (XmlNode N in Loop.ChildNodes)
					{
						if (N is XmlElement E)
							ToProcess.AddLast(E);
					}

					s = Loop.NamespaceURI;
					if (s != Last)
					{
						Last = s;
						if (!Schemas.ContainsKey(s))
						{
							if (Factory.TryGetSchemaResource(s, out KeyValuePair<string, Assembly> P))
								Schemas[s] = XSL.LoadSchema(P.Key, P.Value);
						}
					}
				}

				XmlSchema[] Schemas2 = new XmlSchema[Schemas.Count];
				Schemas.Values.CopyTo(Schemas2, 0);

				XSL.Validate("Model", Model, "Model", TAG.Simulator.Model.ComSimNamespace, Schemas2);

				if (!string.IsNullOrEmpty(LogFileName))
				{
					if (File.Exists(LogFileName))
						File.Delete(LogFileName);

					Log.Register(new XmlFileEventSink("XmlEventSink", LogFileName, LogTransformFileName, int.MaxValue));
				}

				if (LogConsole)
					Log.Register(new ConsoleEventSink(false));

				TaskCompletionSource<bool> Done = new(false);

				try
				{
					SetConsoleCtrlHandler((ControlType) =>
					{
						switch (ControlType)
						{
							case CtrlTypes.CTRL_BREAK_EVENT:
							case CtrlTypes.CTRL_CLOSE_EVENT:
							case CtrlTypes.CTRL_C_EVENT:
							case CtrlTypes.CTRL_SHUTDOWN_EVENT:
								Done.TrySetResult(false);
								break;

							case CtrlTypes.CTRL_LOGOFF_EVENT:
								break;
						}

						return true;
					}, true);
				}
				catch (Exception)
				{
					Log.Error("Unable to register CTRL-C control handler.");
				}

				Console.Out.WriteLine("Initializing database.");

				bool Result = Run(Model, Done, SnifferFolder, SnifferTransformFileName, MarkdownOutputFileName,
						XmlOutputFileName, CommandLine.ToString(), Master, Css, !LogConsole,
						ProgramDataFolder, BlockSize, BlobBlockSize, Encoding, Encryption).Result;

				if (Result)
				{
					Console.Out.WriteLine("Simulation completed.");
					return 0;
				}
				else
				{
					WriteLine("Simulation aborted.", ConsoleColor.White, ConsoleColor.Red);
					return 1;
				}
			}
			catch (AggregateException ex)
			{
				foreach (Exception ex2 in ex.InnerExceptions)
					WriteLine(ex2.Message, ConsoleColor.White, ConsoleColor.Red);

				return 1;
			}
			catch (Exception ex)
			{
				WriteLine(ex.Message, ConsoleColor.White, ConsoleColor.Red);
				return 1;
			}
		}

		private static async Task<bool> Run(XmlDocument ModelXml, TaskCompletionSource<bool> Done,
			string SnifferFolder, string SnifferTransformFileName, string MarkdownOutputFileName, string XmlOutputFileName,
			string CommandLine, IEnumerable<string> Master, IEnumerable<string> Css, bool EmitDots,
			string ProgramDataFolder, int BlockSize, int BlobBlockSize, Encoding Encoding, bool Encryption)
		{
			using FilesProvider DB = await FilesProvider.CreateAsync(ProgramDataFolder, "Default", BlockSize, 10000, BlobBlockSize, Encoding, 3600000, Encryption, false);
			
			try
			{
				Console.Out.WriteLine("Starting database...");
				Database.Register(DB);
				await DB.RepairIfInproperShutdown(null);

				await Database.Clear("EventLog");
				Log.Register(new PersistedEventLog(int.MaxValue));

				Console.Out.WriteLine("Starting modules...");
				await Types.StartAllModules(60000);

				Console.Out.WriteLine("Running simulation...");
				Model Model = (Model)await Factory.Create(ModelXml.DocumentElement, null, null);

				Model.CommandLine = CommandLine;
				Model.SnifferFolder = SnifferFolder;
				Model.SnifferTransformFileName = SnifferTransformFileName;
				Model.OnGetKey += Model_OnGetKey;
				Model.OnGetThreadCount += Model_OnGetThreadCount;

				bool Result = await Model.Run(Done, EmitDots);

				if (!string.IsNullOrEmpty(MarkdownOutputFileName))
				{
					Console.Out.WriteLine("Generating Markdown report: " + MarkdownOutputFileName);

					string Folder = Path.GetDirectoryName(MarkdownOutputFileName);
					if (!string.IsNullOrEmpty(Folder) && !Directory.Exists(Folder))
						Directory.CreateDirectory(Folder);

					using StreamWriter Output = File.CreateText(MarkdownOutputFileName);
					
					foreach (string s in Master)
					{
						Output.Write("Master: ");
						Output.WriteLine(s);
					}

					foreach (string s in Css)
					{
						Output.Write("CSS: ");
						Output.WriteLine(s);
					}

					await Model.ExportMarkdown(Output);
				}

				if (!string.IsNullOrEmpty(XmlOutputFileName))
				{
					Console.Out.WriteLine("Generating XML report: " + XmlOutputFileName);

					XmlWriterSettings Settings = new()
					{
						Encoding = Encoding.UTF8,
						Indent = true,
						IndentChars = "\t",
						NewLineChars = "\r\n",
						NewLineOnAttributes = false,
						WriteEndDocumentOnClose = true
					};

					string Folder = Path.GetDirectoryName(XmlOutputFileName);
					if (!string.IsNullOrEmpty(Folder) && !Directory.Exists(Folder))
						Directory.CreateDirectory(Folder);

					using XmlWriter Output = XmlWriter.Create(XmlOutputFileName, Settings);
					
					Output.WriteStartDocument();
					Output.WriteStartElement("Report", "http://lab.tagroot.io/Schema/ComSimReport.xsd");

					await Model.ExportXml(Output);

					Output.WriteEndElement();
				}

				return Result;
			}
			finally
			{
				Console.Out.WriteLine("Stopping modules...");
				await Types.StopAllModules();
				await DB.Flush();
				await Log.TerminateAsync();
			}
		}

		private static void Model_OnGetThreadCount(object Sender, ThreadCountEventArgs e)
		{
			e.Count = System.Diagnostics.Process.GetCurrentProcess().Threads.Count;
		}

		private static void Model_OnGetKey(object Sender, KeyEventArgs e)
		{
			if (importedKeys.TryGetValue(e.Name, out Dictionary<string, string> Keys) &&
				Keys.TryGetValue(e.LookupValue, out string Key))
			{
				e.Value = Key;

				Console.Out.Write("Key for ");
				Console.Out.Write(e.Name);
				Console.Out.Write(".");
				Console.Out.Write(e.LookupValue);
				Console.Out.WriteLine(" imported.");
			}
			else
			{
				Console.Out.Write("Input value for key ");
				Console.Out.Write(e.Name);

				if (!string.IsNullOrEmpty(e.LookupValue))
				{
					Console.Out.Write('.');
					Console.Out.Write(e.LookupValue);
				}

				Console.Out.Write(": ");

				e.Value = Console.In.ReadLine();
			}
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

		#region unmanaged

		// https://msdn.microsoft.com/en-us/library/windows/desktop/ms686016(v=vs.85).aspx
		// https://msdn.microsoft.com/en-us/library/windows/desktop/ms683242(v=vs.85).aspx

		[DllImport("Kernel32")]
		public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
		public delegate bool HandlerRoutine(CtrlTypes CtrlType);

		public enum CtrlTypes
		{
			CTRL_C_EVENT = 0,
			CTRL_BREAK_EVENT = 1,
			CTRL_CLOSE_EVENT = 2,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT = 6
		}

		#endregion

	}
}
