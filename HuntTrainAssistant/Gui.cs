using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant
{
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
            ImGui.Checkbox("Autoteleport", ref P.config.AutoTeleport);
            ImGui.SetNextItemWidth(150f);
            var cond = P.config.CurrentConductor.Name;
            if(ImGui.InputText("Current conductor", ref cond, 50))
            {
                P.config.CurrentConductor = new(cond, 0);
            }
            ImGui.SetNextItemWidth(60f);
            ImGui.DragFloat("Autoteleport aetheryte distance multiplier", ref P.config.AutoTeleportAetheryteDistanceDiff);
            ImGuiEx.Text($"Destination: {P.TeleportTo.Name} {P.TeleportTo.Territory}");
        }
    }
}
