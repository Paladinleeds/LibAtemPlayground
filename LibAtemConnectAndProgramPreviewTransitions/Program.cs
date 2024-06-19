﻿/* -LICENSE-START-
** Copyright (c) 2018 Blackmagic Design
**
** Permission is hereby granted, free of charge, to any person or organization
** obtaining a copy of the software and accompanying documentation covered by
** this license (the "Software") to use, reproduce, display, distribute,
** execute, and transmit the Software, and to prepare derivative works of the
** Software, and to permit third-parties to whom the Software is furnished to
** do so, all subject to the following:
** 
** The copyright notices in the Software and this entire statement, including
** the above license grant, this restriction and the following disclaimer,
** must be included in all copies of the Software, in whole or in part, and
** all derivative works of the Software, unless such copies or derivative
** works are solely in the form of machine-executable object code generated by
** a source language processor.
** 
** THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
** IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
** FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT
** SHALL THE COPYRIGHT HOLDERS OR ANYONE DISTRIBUTING THE SOFTWARE BE LIABLE
** FOR ANY DAMAGES OR OTHER LIABILITY, WHETHER IN CONTRACT, TORT OR OTHERWISE,
** ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
** DEALINGS IN THE SOFTWARE.
** -LICENSE-END-
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

// using BMDSwitcherAPI;
using LibAtem;
using LibAtem.Commands;
using LibAtem.Commands.Audio;
using LibAtem.Commands.Audio.Fairlight;
using LibAtem.Commands.AudioRouting;
using LibAtem.Commands.CameraControl;
using LibAtem.Commands.DataTransfer;
using LibAtem.Commands.DeviceProfile;
using LibAtem.Commands.DownstreamKey;
using LibAtem.Commands.Macro;
using LibAtem.Commands.Media;
using LibAtem.Commands.MixEffects;
using LibAtem.Commands.MixEffects.Key;
using LibAtem.Commands.MixEffects.Transition;
using LibAtem.Commands.Recording;
using LibAtem.Commands.Settings;
using LibAtem.Commands.Settings.HyperDeck;
using LibAtem.Commands.Settings.Multiview;
using LibAtem.Commands.Streaming;
using LibAtem.Commands.SuperSource;
using LibAtem.Commands.Talkback;
using LibAtem.Common;
using LibAtem.MacroOperations;
using LibAtem.MacroOperations.Audio;
using LibAtem.MacroOperations.DownStreamKey;
using LibAtem.MacroOperations.HyperDeck;
using LibAtem.MacroOperations.Media;
using LibAtem.MacroOperations.MixEffects;
using LibAtem.MacroOperations.MixEffects.Key;
using LibAtem.MacroOperations.MixEffects.Key.Chroma;
using LibAtem.MacroOperations.MixEffects.Key.DVE;
using LibAtem.MacroOperations.MixEffects.Key.Luma;
using LibAtem.MacroOperations.MixEffects.Key.Pattern;
using LibAtem.MacroOperations.MixEffects.Transition;
using LibAtem.MacroOperations.MixEffects.Transition.Dip;
using LibAtem.MacroOperations.MixEffects.Transition.DVE;
using LibAtem.MacroOperations.MixEffects.Transition.Stinger;
using LibAtem.MacroOperations.MixEffects.Transition.Wipe;
using LibAtem.MacroOperations.Settings;
using LibAtem.MacroOperations.SuperSource;
using LibAtem.Net;
using LibAtem.Net.DataTransfer;
using LibAtem.Serialization;
using LibAtem.Util;
using LibAtem.Util.Media;
using System.Runtime.Versioning;
using Newtonsoft.Json;
using ShortcutsAtemKeyboard;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;


namespace LibAtemConnectAndProgramPreviewTransitions
{
	public override void 
	{
		S_OK = 0,
		E_FAIL = unchecked((int)0x80004005),
		E_POINTER = unchecked((int)0x80004003)
	}
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Please input your switcher IP");
			string switcherIP = Console.ReadLine();

			Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
			// Connect to the switcher

			// I need to figure this out somehow...
			var client = new AtemClient(switcherIP, false);
			bool didConnect = client.Connect(); // Need a way to check the response from this... so that I can make sure there's an actual switcher connected before allowing transitions to work...

			// Console.WriteLine($"Connection to {switcherIP} failed. Press ENTER to exit.");
			// Console.ReadLine();
			// Environment.Exit(0);

			ConsoleKeyInfo key;
			Console.WriteLine("Press escape to exit");
			while ((key = Console.ReadKey()).Key != ConsoleKey.Escape)
			{
				if (config.MixEffect != null)
				{
					foreach (KeyValuePair<MixEffectBlockId, Config.MixEffectConfig> me in config.MixEffect)
					{
						if (me.Value.Program != null && me.Value.Program.TryGetValue(key.KeyChar, out VideoSource src))
						{
							client.SendCommand(new ProgramInputSetCommand { Index = me.Key, Source = src });
							Console.WriteLine("");
							Console.WriteLine($"Program Value Got: {src}");
						}

						if (me.Value.Preview != null && me.Value.Preview.TryGetValue(key.KeyChar, out src))
						{
							client.SendCommand(new PreviewInputSetCommand { Index = me.Key, Source = src });
							Console.WriteLine("");
							Console.WriteLine($"Preview Value Got: {src}");
						}

						if (me.Value.Cut == key.KeyChar)
						{
							client.SendCommand(new MixEffectCutCommand { Index = me.Key });
							Console.WriteLine("");
							Console.WriteLine("Cutting Between Inputs");
						}

						if (me.Value.Auto == key.KeyChar)
						{
							client.SendCommand(new MixEffectAutoCommand { Index = me.Key });
							Console.WriteLine("");
							Console.WriteLine("Transitioning Between Inputs");
						}
					}
				}

				/*
				try
				{
					connection.Connect();
				}
				catch (AtemConnectionException e)
				{
					Console.WriteLine($"Connection to ATEM switcher failed: {e.Message}");
					Console.WriteLine("Please press ENTER key to close the window.");
					Console.ReadLine();
					Environment.Exit(0);
				}
				*/
			}
		}
	}

}
