using System;
using System.Linq;
using System.Collections.Generic;

namespace PwGen
{
	class Program
	{
		public static byte EncodeVersion(byte major, byte minor)
		{
			if ((major > (byte)16) || (minor > (byte)16))
			{
				throw new ArgumentException("major/minor is 4-bit");
			}
			return (byte) ((major << 4) | (byte)minor);
		}
		public static (byte, byte) DecodeVersion(byte version) =>
			((byte)(version >> 0x04), (byte)(version & 0x0F));
		public static readonly byte ProgramVersion = EncodeVersion((byte)1, (byte)2);
		private static int TryParseCount(in string arg, out int count) =>
			int.TryParse(arg, out count)
				? count < 1
					? 2
					: 0
				: 1;
			/*if (int.TryParse(arg, out count))
			{
				if (count < 1)
				{
					return 2;
				}
			}
			else
			{
				return 1;
			}
			return 0;*/
		private static void ShowUsage()
		{
			Console.Error.WriteLine(@"Usage: pwgen <options> <password-lengths>

where OPTION's is:
	-?, -h, --help, --usage	Shows this menu
	-a, --all		Same as -duls
	-c, --custom		Allows to specify zero flags
	-D, --default		Same as -dul
	-d, --digits		Include digits in passwords (0-9)
	-e, --exclude STR	Exclude custom characters
	-i, --include STR	Includes custom characters
	-l, --lower		Include lowercase ASCII in passwords (a-z)
	-s, --special		Include special symbols in passwords (!@#$%^&*())
	-r, --repeat N		Same as ""csgen <flags> <length N times>""
				Warning: ""repeat"" will use first length for all passwords!
	-u, --upper		Include uppercase ASCII in passwords (A-Z)
	-w, --remove-lvowels	Excludes lowercase vowels (excludes aeiou)
	-W, --remove-uvowels	Excludes uppercase vowels (excludes AEIOU)
	-v, --version			Show the version

example:
	`pwgen -a 16`:		generate password of length 16 with all characters
	`pwgen -i aeiou 10`:	generate password with only lowercase vowels
	`pwgen -Dr5 10`:	generate 5 passwords of length 10 with default characters
	`pwgen -DW 32`:		generate password of length 32 without uppercase vowels
");
		}
		public static void ShowVersion()
		{
			(byte major, byte minor) = DecodeVersion(ProgramVersion);
			Console.WriteLine(@"Version: {0}.{1}
OS: ""{2}""
CLR version: ""{3}""", major, minor, Environment.OSVersion.ToString(), Environment.Version.ToString());
		}
		static int Main(string[] argv)
		{
			byte flags = Flag.None;
			bool isCustom = false;
			string exclude = "", include = "";
			if (argv.Length != 0)
			{
				List<int> passwords = new();
				int count = -1;
				List<string> args = new(argv);
				while (args.Count != 0)
				{
					string arg = args[0];
					if (arg.StartsWith("--"))
					{
						arg = arg.Substring(2);
						switch (arg.ToLower())
						{
							case "":
								Console.Error.WriteLine("Error: option is empty");
								return 3;
							case "all":
								flags ^= Flag.All;
								break;
							case "custom":
								isCustom ^= true;
								break;
							case "default":
								flags ^= Flag.Default;
								break;
							case "digits":
								flags ^= Flag.Digits;
								break;
							case "exclude":
								exclude = args[1];
								args.RemoveAt(1);
								break;
							case "include":
								include = args[1];
								args.RemoveAt(1);
								break;
							case "help":
							case "usage":
								ShowUsage();
								return 1;
							case "upper":
								flags ^= Flag.Upper;
								break;
							case "lower":
								flags ^= Flag.Lower;
								break;
							case "remove-uvowels":
								exclude += "AEIOU";
								break;
							case "remove-lvowels":
								exclude += "aeiou";
								break;
							case "special":
								flags ^= Flag.Special;
								break;
							case "version":
								ShowVersion();
								return 0;
							case "repeat":
								switch (TryParseCount(args[1], out count))
								{
									case 1:
										Console.Error.WriteLine("Error: invalid number: \"{0}\"", args[1]);
										return 8;
									case 2:
										Console.Error.WriteLine("Error: count cannot be zero or negative");
										return 10;
								}
								args.RemoveAt(1);
								break;
							default:
								Console.Error.WriteLine("Error: Unknown option \"{0}\"", arg);
								return 4;
						}
					}
					else if (arg.StartsWith("-"))
					{
						arg = arg.Substring(1);
						if (string.IsNullOrEmpty(arg))
						{
							continue;
						}
						bool exitLoop = false;
						int i = 0;
						foreach (char ch in arg)
						{
							if (exitLoop)
								break;
							switch (ch)
							{
								case 'a':
									flags ^= Flag.All;
									break;
								case 'c':
									isCustom ^= true;
									break;
								case 'D':
									flags ^= Flag.Default;
									break;
								case 'd':
									flags ^= Flag.Digits;
									break;
								case 'e':
									if ((i + 1) == arg.Length)
									{
										exclude = args[1];
										args.RemoveAt(1);
									}
									else
									{
										exclude = arg.Substring(i);
										exitLoop = true;
									}
									break;
								case '?':
								case 'h':
									ShowUsage();
									return 1;
								case 'i':
									if ((i + 1) == arg.Length)
									{
										include = args[1];
										args.RemoveAt(1);
									}
									else
									{
										include = arg.Substring(i);
										exitLoop = true;
									}
									break;
								case 'u':
									flags ^= Flag.Upper;
									break;
								case 'l':
									flags ^= Flag.Lower;
									break;
								case 's':
									flags ^= Flag.Special;
									break;
								case 'r':
									if ((i + 1) == arg.Length)
									{
										arg = args[1];
										args.RemoveAt(1);
									}
									else
									{
										arg = arg.Substring(i + 1);
										exitLoop = true;
									}
									switch (TryParseCount(arg, out count))
									{
										case 1:
											Console.Error.WriteLine("Error: invalid number: \"{0}\"", arg);
											return 11;
										case 2:
											Console.Error.WriteLine("Error: count cannot be zero or negative");
											return 12;
									}
									break;
								case 'v':
									ShowVersion();
									return 0;
								case 'w':
									exclude += "aeiou";
									break;
								case 'W':
									exclude += "AEIOU";
									break;
								default:
									Console.Error.WriteLine("Error: Unknown option '{0}'", ch);
									return 5;
							}
							++i;
						}
						
					}
					else
					{
						if (int.TryParse(arg, out int length))
						{
							if (length < 1)
							{
								Console.Error.WriteLine("Error: length cannot be zero or negative");
								return 9;
							}
							passwords.Add(length);
						}
						else
						{
							Console.Error.WriteLine("Error: invalid number: \"{0}\"", arg);
							return 2;
						}
					}
					args.RemoveAt(0);
				}
				if ((flags == 0 && string.IsNullOrWhiteSpace(include)) && !isCustom)
				{
					Console.Error.WriteLine("Error: flags not specified");
					ShowUsage();
					return 6;
				}
				if (passwords.Count == 0)
				{
					Console.Error.WriteLine("Error: no lengths specifed");
					return 7;
				}
				if (count != -1)
				{
					int length = passwords[0];
					passwords.Clear();
					passwords.AddRange(Enumerable.Repeat(length, count));
				}
				PwGen pwg = new();
				foreach (int length in passwords)
				{
					Console.WriteLine(pwg.Generate(flags, length, exclude, include));
				}
			}
			else
			{
				ShowUsage();
				return 1;
			}
			return 0;
		}
	}
}
