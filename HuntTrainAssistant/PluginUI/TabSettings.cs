using Dalamud.Memory.Exceptions;
using NightmareUI.PrimaryUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.PluginUI;
public class TabSettings
{
		public void Draw()
		{
				new NuiBuilder().
						Section("General settings")
						.Widget(() =>
						{
								ImGui.Checkbox("Plugin enabled", ref P.Config.Enabled);
								ImGui.SameLine();
								ImGui.Checkbox("Debug mode", ref P.Config.Debug);
								ImGui.Checkbox("Autoteleport to different zone", ref P.Config.AutoTeleport);
								ImGui.Checkbox("Auto-open map when new location is linked", ref P.Config.AutoOpenMap);
								ImGui.Checkbox("When conductor is set, suppress other people's messages", ref P.Config.SuppressChatOtherPlayers);
								ImGui.Checkbox("Compensate for some aetherytes' position", ref P.Config.DistanceCompensationHack);
						})
						.Draw();
		}
}
