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

						.Section("Integrations")
						.Widget(() =>
						{
								ImGui.Checkbox("Enable Sonar integration", ref P.Config.SonarIntegration);
								ImGuiEx.PluginAvailabilityIndicator([new("SonarPlugin", "Sonar")]);
								ImGui.Indent();
								ImGuiEx.TextWrapped("When a hunt mark announced in chat, automatically teleport to the target world and zone");
								ImGui.Unindent();
								ImGui.Checkbox($"Enable auto-teleport to nearest aetheryte", ref P.Config.AutoVisitTeleportEnabled);
								ImGuiEx.PluginAvailabilityIndicator([new("TeleporterPlugin", "Teleporter")]);
								ImGui.Checkbox("Allow cross-world teleports", ref P.Config.AutoVisitCrossWorld);
								ImGuiEx.PluginAvailabilityIndicator([new("TeleporterPlugin", "Teleporter"), new("Lifestream", new Version("2.1.1.0"))]);
								ImGui.Checkbox("Allow cross-datacenter teleports", ref P.Config.AutoVisitCrossDC);
								ImGuiEx.PluginAvailabilityIndicator([new("TeleporterPlugin", "Teleporter"), new("Lifestream", new Version("2.1.1.0"))]);
								ImGui.Checkbox("Add click to teleport link into chat message", ref P.Config.AutoVisitModifyChat);
						})

						.Draw();
		}
}
