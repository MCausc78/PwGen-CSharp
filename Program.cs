using System;
using System.Collections.Generic;

namespace PwGen
{
	class Program
	{
		private static void ShowUsage()
		{
			Console.Error.WriteLine(@"Usage: csgen <options> <password-lengths>

where OPTION's is:
	-a, --all		Same as -duls
	-c, --custom		Allows to specify zero flags
	-D, --default		Same as -dul
	-d, --digits		Include digits in passwords (0-9)
	-e, --exclude		Exclude custom characters
	-?, -h, --help, --usage	Shows this menu
	-i, --include		Includes custom characters
	-u, --upper		Include uppercase ASCII in passwords (A-Z)
	-l, --lower		Include lowercase ASCII in passwords (a-z)
	-r, --remove-lvowels	Excludes lowercase vowels (excludes aeiou)
	-R, --remove-uvowels	Excludes uppercase vowels (excludes AEIOU)
	-s, --special		Include special symbols in passwords (!@#$%^&*())
");
		}
		static int Main(string[] argv)
		{
			byte flags = Flag.None;
			bool isCustom = false;
			string exclude = "", include = "";
			if (argv.Length != 0)
			{
				List<int> passwords = new();
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
							case "help":
							case "include":
								include = args[1];
								args.RemoveAt(1);
								break;
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
						foreach (char ch in arg)
						{
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
									exclude = args[1];
									args.RemoveAt(1);
									break;
								case '?':
								case 'h':
									ShowUsage();
									return 1;
								case 'i':
									include = args[1];
									args.RemoveAt(1);
									break;
								case 'u':
									flags ^= Flag.Upper;
									break;
								case 'l':
									flags ^= Flag.Lower;
									break;
								case 'R':
									exclude += "AEIOU";
									break;
								case 'r':
									exclude += "aeiou";
									break;
								case 's':
									flags ^= Flag.Special;
									break;
								default:
									Console.Error.WriteLine("Error: Unknown option '{0}'", ch);
									return 5;
							}
						}
					}
					else
					{
						if (int.TryParse(arg, out int length))
						{
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
