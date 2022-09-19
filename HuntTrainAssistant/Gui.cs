using ECommons.ImGuiMethods;

namespace HuntTrainAssistant;

internal unsafe static class Gui
{
    internal static void Draw()
    {
        ImGuiEx.EzTabBar("HTA",
            ("Current", Control, null, true),
            ("Settings", General, null, true),
            //("Help", Help, null, true),
            ("Debug", Debug, ImGuiColors.DalamudGrey3, true),
            ("Log", InternalLog.PrintImgui, ImGuiColors.DalamudGrey3, false),
            ("Contribute", Donation.DonationTabDraw, ImGuiColors.ParsedGold, true)
            );
    }

    static void Control()
    {
        ImGui.SetNextItemWidth(150f);
        var cond = P.config.CurrentConductor.Name;
        ImGuiEx.Text("Current conductor:");
        ImGui.SameLine();
        if (ImGui.SmallButton("Clear"))
        {
            P.config.CurrentConductor = new("", 0);
        }
        ImGuiEx.SetNextItemFullWidth();
        if (ImGui.InputText("##curcond", ref cond, 50))
        {
            P.config.CurrentConductor = new(cond, 0);
        }
        if (P.TeleportTo.Territory == 0)
        {
            ImGuiEx.Text("Autoteleport: inactive");
            if(ChatMessageHandler.LastMessageLoc.Aetheryte != null && ImGui.Button($"Autoteleport to {ChatMessageHandler.LastMessageLoc.Aetheryte}"))
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
                P.TeleportTo.Name = "";
            }
            ImGuiEx.Text($"{P.TeleportTo.Name}@{GenericHelpers.GetTerritoryName(P.TeleportTo.Territory)}");
        }
    }

    static void General()
    {
        ImGui.Checkbox("Plugin enabled", ref P.config.Enabled);
        ImGui.SameLine();
        ImGui.Checkbox("Debug mode", ref P.config.Debug);
        ImGui.Checkbox("Autoteleport to different zone", ref P.config.AutoTeleport);
        ImGui.Checkbox("Auto-open map when new location is linked", ref P.config.AutoOpenMap);
        ImGui.Checkbox("When conductor is set, suppress other people's messages", ref P.config.SuppressChatOtherPlayers);
        //ImGui.SetNextItemWidth(60f);
        //ImGui.DragFloat("Autoteleport aetheryte distance multiplier", ref P.config.AutoTeleportAetheryteDistanceDiff);
    }

    static void Debug()
    {
        ImGuiEx.Text($"Flag set: {MapFlag.Instance()->IsFlagSet}");
        ImGuiEx.Text($"X: {MapFlag.Instance()->X}");
        ImGuiEx.Text($"Y: {MapFlag.Instance()->Y}");
        ImGuiEx.Text($"Territory: {GenericHelpers.GetTerritoryName(MapFlag.Instance()->TerritoryType)}");
        ImGuiEx.Text($"Is moving: {P.IsMoving}");
    }

    static void Help()
    {
        ImGuiEx.TextWrapped("- Be in one of Endwalker hunt zones;");
        ImGuiEx.TextWrapped("- Assign conductor either by right-clicking them in chat/world or enter their name manually;");
    }
}
