using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwGen
{
	class PwGen
	{
		public Random RNG
		{
			get;
		}
		public PwGen()
		{
			RNG = new Random();
		}
		public string Generate(byte flags, int length, string exclude, string include)
		{
			string alphabet = "";
			StringBuilder sb = new();
			if ((flags & Flag.Digits) != 0)
				alphabet += ("0123456789");
			if ((flags & Flag.Lower) != 0)
				alphabet += "abcdefghijklmnopqrstuvwxyz";
			if ((flags & Flag.Upper) != 0)
				alphabet += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			if ((flags & Flag.Special) != 0)
				alphabet += "!@#$%^&*()";
			foreach (char ch in include)
			{
				if (alphabet.IndexOf(ch) != -1)
				{
					continue;
				}
				alphabet += ch;
			}
			int pos;
			foreach (char ch in exclude)
			{
				while ((pos = alphabet.IndexOf(ch)) != -1)
				{
					alphabet = alphabet.Remove(pos, 1);
				}
			}
			foreach (char ch in alphabet)
			{
				while (alphabet.IndexOf(ch) != (pos = alphabet.LastIndexOf(ch)))
				{
					alphabet = alphabet.Remove(pos, 1);
				}
			}
			int alphabetLength = alphabet.Length;
			if (alphabetLength < 1)
			{
				return this.Generate(Flag.None, length, "", "");
			}
			for (int i = 0; i < length; ++i)
			{
				sb.Append(alphabet[RNG.Next(alphabetLength)]);
			}
			return sb.ToString();
		}
	}
}
