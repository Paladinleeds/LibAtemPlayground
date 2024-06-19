using System.Collections.Generic;
using LibAtem.Common;

namespace ShortcutsAtemKeyboard
{
	public class Config
	{
		public Dictionary<MixEffectBlockId, MixEffectConfig> MixEffect { get; set; }

		public Dictionary<SuperSourceBoxId, Dictionary<char, VideoSource>> SuperSource { get; set; }

		public class MixEffectConfig
		{
			public Dictionary<char, VideoSource> Program { get; set; }
			public Dictionary<char, VideoSource> Preview { get; set; }

			public char Cut { get; set; }
			public char Auto { get; set; }
		}
	}
}
