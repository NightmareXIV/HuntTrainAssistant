using ECommons.ExcelServices;
using HuntTrainAssistant.DataStructures;
using NightmareUI.PrimaryUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.PluginUI;
public class TabIntegrations
{
    private TabIntegrations() { }
    public void Draw()
    {
        new NuiBuilder()
        .Section("Plugins")
        .Widget(() =>
        {
            ImGui.Checkbox("Enable Sonar integration", ref P.Config.SonarIntegration);
            ImGuiEx.PluginAvailabilityIndicator([new("SonarPlugin", "Sonar")]);
            ImGui.Indent();
            ImGuiEx.TextWrapped("When a hunt mark announced in chat, automatically teleport to the target world and zone");
            ImGui.Checkbox("Add click to teleport link into chat message", ref P.Config.AutoVisitModifyChat);
            ImGui.Checkbox("Change instance after teleporting", ref P.Config.EnableSonarInstanceSwitching);
            ImGui.Unindent();
            ImGui.Separator();
            ImGui.Checkbox("Enable HuntAlerts integration", ref P.Config.HuntAlertsIntegration);
            ImGuiEx.PluginAvailabilityIndicator([new("HuntAlerts", new Version("1.2.1.3"))]);
            ImGuiEx.TextWrapped("When a hunt mark notification is received from server, automatically teleport to the target world and zone");
        })

        .Section("Common Settings")
        .Widget(() =>
        {
            ImGuiEx.TextWrapped($"These options are common for all integrations");
            ImGui.Separator();
            ImGui.Checkbox($"Teleport to nearest aetheryte upon receiving announcement", ref P.Config.AutoVisitTeleportEnabled);
            ImGuiEx.PluginAvailabilityIndicator([new("TeleporterPlugin", "Teleporter")]);
            ImGuiEx.PluginAvailabilityIndicator([new("Lifestream")]);
            ImGui.Checkbox("Allow cross-world teleports", ref P.Config.AutoVisitCrossWorld);
            ImGuiEx.PluginAvailabilityIndicator([new("TeleporterPlugin", "Teleporter"), new("Lifestream")]);
            ImGuiEx.PluginAvailabilityIndicator([new("Lifestream")]);
            ImGui.Checkbox("Allow cross-datacenter teleports", ref P.Config.AutoVisitCrossDC);
            ImGuiEx.PluginAvailabilityIndicator([new("TeleporterPlugin", "Teleporter"), new("Lifestream")]);
            ImGuiEx.PluginAvailabilityIndicator([new("Lifestream")]);
            ImGuiEx.TreeNodeCollapsingHeader($"Blacklist Worlds ({P.Config.WorldBlacklist.Count} currently blacklisted)###blworlds", DrawWorldBlacklist);
        })

        .Section("Trigger Filters")
        .Widget(() =>
        {
            foreach(var rank in Enum.GetValues<Rank>())
            {
                if (rank == Rank.Unknown) continue;
                ImGui.PushID($"{rank}");
                if (!P.Config.AutoVisitExpansionsBlacklist.TryGetValue(rank, out var list))
                {
                    list = [];
                    P.Config.AutoVisitExpansionsBlacklist[rank] = list;
                }
                ImGuiEx.CollectionCheckbox($"{rank}", Enum.GetValues<Expansion>(), list, true);
                ImGui.Indent();
                foreach(var ex in Enum.GetValues<Expansion>())
                {
                    if(ex == Expansion.Unknown) continue;
                    ImGuiEx.CollectionCheckbox($"{ex}", ex, list, true);
                }
                ImGui.Unindent();
                ImGui.PopID();
            }
        })

        .Draw();
    }

    void DrawWorldBlacklist()
    {
        ImGuiEx.TextWrapped($"Auto-teleport will not be engaged to the worlds selected below. You can still use chat link to get to them manually.");
        foreach(var r in Enum.GetValues<ExcelWorldHelper.Region>())
        {
            ImGuiEx.CollectionCheckbox($"Region {r}", ExcelWorldHelper.GetPublicWorlds(r).Select(x => x.RowId), P.Config.WorldBlacklist);
            ImGui.Indent();
            foreach(var dc in ExcelWorldHelper.GetDataCenters(r))
            {
                ImGuiEx.CollectionCheckbox($"{dc.Name} Data Center", ExcelWorldHelper.GetPublicWorlds(dc.RowId).Select(x => x.RowId), P.Config.WorldBlacklist);
                ImGui.Indent();
                foreach(var w in ExcelWorldHelper.GetPublicWorlds(dc.RowId))
                {
                    ImGuiEx.CollectionCheckbox($"{w.Name}", w.RowId, P.Config.WorldBlacklist);
                }
                ImGui.Unindent();
            }
            ImGui.Unindent();
        }
    }
}
