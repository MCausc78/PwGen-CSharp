using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwGen
{
	public static class Flag
	{
		public const byte
			Digits = 0b00000100,
			Lower = 0b00000010,
			None = 0b00000000,
			Upper = 0b00000001,
			Special = 0b00001000,
			All = Digits | Lower | Upper | Special,
			Default = Digits | Lower | Upper;
	}
}
