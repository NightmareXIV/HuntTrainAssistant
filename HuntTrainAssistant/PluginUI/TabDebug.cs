using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.PluginUI;
public unsafe class TabDebug
{
		public void Draw()
		{
				ImGuiEx.Text($"Flag set: {MapFlag.Instance()->IsFlagSet}");
				ImGuiEx.Text($"X: {MapFlag.Instance()->X}");
				ImGuiEx.Text($"Y: {MapFlag.Instance()->Y}");
				ImGuiEx.Text($"Territory: {MapFlag.Instance()->TerritoryType.GetTerritoryName()}");
				ImGuiEx.Text($"Is moving: {P.IsMoving}");
		}
}
