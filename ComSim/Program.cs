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
	///                       http://trustanchorgroup.com/Schema/ComSim.xsd namespace.
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
	/// -e                    If encryption is used by the database. Default=no encryption.
	/// -bs BLOCK_SIZE        Block size, in bytes. Default=8192.
	/// -bbs BLOB_BLOCK_SIZE  BLOB block size, in bytes. Default=8192.
	/// -enc ENCODING         Text encoding. Default=UTF-8
	/// -?                    Displays command-line help.
	/// </summary>
	class Program
	{
		static int Main(string[] args)
		{
			try
			{
				XmlDocument Model = null;
				Encoding Encoding = Encoding.UTF8;
				string ProgramDataFolder = null;
				string SnifferFolder = null;
				string LogFileName = null;
				string LogTransformFileName = null;
				string SnifferTransformFileName = null;
				int i = 0;
				int c = args.Length;
				int BlockSize = 8192;
				int BlobBlockSize = 8192;
				bool Encryption = false;
				string s;
				bool Help = args.Length == 0;
				bool LogConsole = false;

				while (i < c)
				{
					s = args[i++].ToLower();

					if (s.StartsWith("/"))
						s = "-" + s.Substring(1);

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
					Console.Out.WriteLine("                      http://trustanchorgroup.com/Schema/ComSim.xsd namespace.");
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
					Console.Out.WriteLine("-e                    If encryption is used by the database. Default=no encryption.");
					Console.Out.WriteLine("-bs BLOCK_SIZE        Block size, in bytes. Default=8192.");
					Console.Out.WriteLine("-bbs BLOB_BLOCK_SIZE  BLOB block size, in bytes. Default=8192.");
					Console.Out.WriteLine("-enc ENCODING         Text encoding. Default=UTF-8");
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
						Dictionary<string, Assembly> Loaded = new Dictionary<string, Assembly>();

						foreach (Assembly A in AppDomain.CurrentDomain.GetAssemblies())
							Loaded[A.GetName().Name] = A;

						foreach (XmlNode N2 in E.ChildNodes)
						{
							if (N2 is XmlElement E2 && E2.LocalName == "Assembly")
							{
								string FileName = XML.Attribute(E2, "fileName");

								if (!File.Exists(FileName))
									throw new Exception("File not found: " + FileName);

								LinkedList<string> ToLoad = new LinkedList<string>();
								ToLoad.AddLast(FileName);

								while (!string.IsNullOrEmpty(FileName = ToLoad.First?.Value))
								{
									ToLoad.RemoveFirst();

									Console.Out.WriteLine("Loading " + FileName);

									byte[] Bin = File.ReadAllBytes(FileName);
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

				Dictionary<string, XmlSchema> Schemas = new Dictionary<string, XmlSchema>();
				LinkedList<XmlElement> ToProcess = new LinkedList<XmlElement>();
				XmlElement Loop;
				string Last = null;

				ToProcess.AddLast(Model.DocumentElement);

				while (!((Loop = ToProcess.First?.Value) is null))
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
							if (!Factory.TryGetSchemaResource(s, out KeyValuePair<string, Assembly> P))
								throw new Exception("Namespace " + s + " not defined in a schema in any of the loaded modules.");

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

				TaskCompletionSource<bool> Done = new TaskCompletionSource<bool>(false);

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

				bool Result;

				using (FilesProvider FilesProvider = new FilesProvider(ProgramDataFolder, "Default", BlockSize, 10000, BlobBlockSize, Encoding, 3600000, Encryption, false))
				{
					Result = Run(Model, FilesProvider, Done, SnifferFolder, SnifferTransformFileName).Result;
				}

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

		private static async Task<bool> Run(XmlDocument ModelXml, FilesProvider DB, TaskCompletionSource<bool> Done,
			string SnifferFolder, string SnifferTransformFileName)
		{
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
				Model Model = (Model)await Factory.Create(ModelXml.DocumentElement, null);

				Model.SnifferFolder = SnifferFolder;
				Model.SnifferTransformFileName = SnifferTransformFileName;
				Model.OnGetKey += Model_OnGetKey;

				return await Model.Run(Done);
			}
			finally
			{
				Console.Out.WriteLine("Stopping modules...");
				await Types.StopAllModules();
				await DB.Flush();
				Log.Terminate();
			}
		}

		private static void Model_OnGetKey(object Sender, KeyEventArgs e)
		{
			Console.Out.Write("Input value for key " + e.Name + ": ");
			e.Value = Console.In.ReadLine();
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
