using System;

namespace Core.Const
{
	[Flags]
	public enum Locks : uint
	{
		None = 0x00,
		Walk = 0x08,
		Run = 0x10,

		// ------------------------------------------------------------------
		Move = Run | Walk,
		Default = 0xEFFFFFFE,
		All = 0xFFFFFFFF,
	}
}
