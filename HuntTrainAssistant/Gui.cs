namespace HuntTrainAssistant;

internal static class Gui
{
    internal static void Draw()
    {
        ImGuiEx.EzTabBar("HTA",
            ("General", General, null, true)
            );
    }

    static void General()
    {
        ImGui.Checkbox("Plugin enabled", ref P.config.Enabled);
        ImGui.SameLine();
        ImGui.Checkbox("Debug mode", ref P.config.Debug);
        ImGui.Checkbox("Autoteleport to different zone", ref P.config.AutoTeleport);
        ImGui.Checkbox("Auto-open map on same zone", ref P.config.AutoOpenMap);
        ImGui.Checkbox("When conductor is set, suppress other people's messages", ref P.config.SuppressChatOtherPlayers);
        ImGui.SetNextItemWidth(150f);
        var cond = P.config.CurrentConductor.Name;
        if(ImGui.InputText("Current conductor", ref cond, 50))
        {
            P.config.CurrentConductor = new(cond, 0);
        }
        //ImGui.SetNextItemWidth(60f);
        //ImGui.DragFloat("Autoteleport aetheryte distance multiplier", ref P.config.AutoTeleportAetheryteDistanceDiff);
        ImGuiEx.Text($"Destination: {P.TeleportTo.Name} {P.TeleportTo.Territory}");
    }
}
