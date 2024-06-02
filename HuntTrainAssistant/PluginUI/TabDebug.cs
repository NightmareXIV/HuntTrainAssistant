using Lumina.Excel.GeneratedSheets;
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
				if (ImGui.CollapsingHeader("Territory"))
				{
						foreach (var x in Svc.Data.GetExcelSheet<TerritoryType>())
						{
								try
								{
										ImGuiEx.Text($"{x.RowId}: {x.PlaceName?.Value?.Name} / {x.TerritoryIntendedUse}");
								}
								catch (Exception e)
								{
										ImGuiEx.Text($"{e.Message}");
								}
						}
				}
				if (ImGui.CollapsingHeader("Weather"))
				{
						foreach (var x in Svc.Data.GetExcelSheet<Weather>())
						{
								try
								{
										ImGuiEx.Text($"{x.RowId}: {x.Name} / {x.Description}");
								}
								catch (Exception e)
								{
										ImGuiEx.Text($"{e.Message}");
								}
						}
				}
				if (ImGui.CollapsingHeader("Aetheryte"))
				{
						foreach (var x in Svc.Data.GetExcelSheet<Aetheryte>())
						{
								try
								{
										ImGuiEx.Text($"{x.RowId}: {x.PlaceName?.Value?.Name} / {x.AethernetName.Value?.Name}");
								}
								catch (Exception e)
								{
										ImGuiEx.Text($"{e.Message}");
								}
						}
				}
		}
}
