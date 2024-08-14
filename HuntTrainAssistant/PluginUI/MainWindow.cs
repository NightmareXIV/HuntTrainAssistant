using ECommons.ImGuiMethods;
using ECommons.SimpleGui;
using System.Runtime.Intrinsics.X86;

namespace HuntTrainAssistant.PluginUI;

public unsafe class MainWindow : ConfigWindow
{
    public MainWindow() : base()
    {
				TitleBarButtons.Add(new()
				{
						Click = (m) => { if (m == ImGuiMouseButton.Left) S.SettingsWindow.IsOpen = true; },
						Icon = FontAwesomeIcon.Cog,
						IconOffset = new(2, 2),
						ShowTooltip = () => ImGui.SetTooltip("Open settings window"),
				});
		}

    public override void Draw()
    {
				ImGui.SetNextItemWidth(150f);
				var condIndex = 0;
				var condNames = P.Config.Conductors.Select(x => x.Name).ToArray();
				ImGuiEx.Text("Current conductors:");
				ImGui.SameLine();
				if (ImGui.SmallButton("Clear"))
				{
						P.Config.Conductors.Clear();
				}
				ImGui.SameLine();
				// Remove selected conductor
				if (ImGui.SmallButton("Remove selected"))
				{
						if (condIndex >= 0 && condIndex < P.Config.Conductors.Count)
						{
								P.Config.Conductors.RemoveAt(condIndex);
						}
				}
				ImGuiEx.SetNextItemFullWidth();
				ImGui.ListBox("##conds", ref condIndex, condNames, condNames.Length, Math.Clamp(condNames.Length, 1, 3));
				ImGuiEx.Text("Add conductor:");
				ImGui.SameLine();
				ImGui.SetNextItemWidth(150f);
				var newCond = "";
				if (ImGui.InputText("##newCond", ref newCond, 50, ImGuiInputTextFlags.EnterReturnsTrue))
				{
						if (newCond.Length > 0)
						{
								P.Config.Conductors.Add(new(newCond, 0));
								newCond = "";
						}
				}
				if (P.TeleportTo.Territory == 0)
				{
						ImGuiEx.Text("Autoteleport: inactive");
						if (ChatMessageHandler.LastMessageLoc.Aetheryte != null && ImGui.Button($"Autoteleport to {ChatMessageHandler.LastMessageLoc.Aetheryte.PlaceName.Value.Name}"))
						{
								P.TeleportTo = ChatMessageHandler.LastMessageLoc;
						}
				}
				else
				{
						ImGuiEx.Text($"Autoteleport active.");
						ImGui.SameLine();
						if (ImGui.SmallButton("Cancel"))
						{
								P.TeleportTo.Territory = 0;
								P.TeleportTo.Aetheryte = null;
						}
						ImGuiEx.Text($"{P.TeleportTo.Aetheryte.GetPlaceName()}@{P.TeleportTo.Territory.GetTerritoryName()} i{P.TeleportTo.Instance}");
				}
				ImGui.Checkbox($"Sonar Auto-teleport", ref P.Config.AutoVisitTeleportEnabled);
				if (P.Config.AutoVisitTeleportEnabled)
				{
						if (!Utils.IsInHuntingTerritory())
						{
								ImGuiEx.HelpMarker("You are not in a hunting zone. Teleport enabled.", EColor.GreenBright, FontAwesomeIcon.Check.ToIconString());
						}
						else
						{
								ImGuiEx.HelpMarker("You are in a hunting zone. Teleport disabled. ", EColor.RedBright, "\uf00d");
						}
						ImGui.SameLine();
						ImGui.Checkbox("C/W", ref P.Config.AutoVisitCrossWorld);
						ImGui.SameLine();
						ImGui.Checkbox("C/DC", ref P.Config.AutoVisitCrossDC);
				}
				if(S.SonarMonitor.Continuation != null)
				{
						ImGuiEx.Text(GradientColor.Get(EColor.RedBright, EColor.YellowBright), $"Waiting to arrive at: {S.SonarMonitor.Continuation.Value.World}/{S.SonarMonitor.Continuation.Value.Aetheryte.GetPlaceName()}");
						if (ImGui.SmallButton("Cancel##arrival"))
						{
								S.SonarMonitor.Continuation = null;
						}
				}
    }

    static void Help()
    {
        ImGuiEx.TextWrapped("- Be in one of Endwalker hunt zones;");
        ImGuiEx.TextWrapped("- Assign conductors either by right-clicking them in chat/world or enter their names manually;");
    }
}
