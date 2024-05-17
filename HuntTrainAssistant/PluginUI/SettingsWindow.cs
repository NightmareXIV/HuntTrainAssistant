using ECommons.Funding;
using ECommons.SimpleGui;
using NightmareUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.PluginUI;
public unsafe class SettingsWindow : ConfigWindow
{
		public TabSettings TabSettings = new();
		public TabDebug TabDebug = new();

		private SettingsWindow() : base()
		{
				this.WindowName = "HuntTrainAssistant Configuration";
				EzConfigGui.WindowSystem.AddWindow(this);
		}

		public override void Draw()
		{
				PatreonBanner.DrawRight();
				ImGuiEx.EzTabBar("Bar", PatreonBanner.Text,
						("Settings", TabSettings.Draw, null, true),
						("Debug", TabDebug.Draw, ImGuiColors.DalamudGrey3, true),
						("Log", InternalLog.PrintImgui, ImGuiColors.DalamudGrey3, false)
						);
		}
}
