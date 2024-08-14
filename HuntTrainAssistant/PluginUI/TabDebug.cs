using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using HuntTrainAssistant.DataStructures;
using HuntTrainAssistant.Tasks;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.PluginUI;
public unsafe class TabDebug
{
		public List<byte[]> TestMessages = []; 

		public void Draw()
		{
				ref var inst = ref Ref<int>.Get("Instance");
				ImGui.SetNextItemWidth(100f);
				ImGui.InputInt("Instance", ref inst);
				ImGui.SameLine();
				if(ImGui.Button("Fake Sonar message"))
				{
						var flag = AgentMap.Instance()->FlagMapMarker;
						var m = new XivChatEntry
						{
								Name = "Sonar",
								Message = new SeStringBuilder().AddText("Rank S: ").AddMapLink(flag.TerritoryId, flag.MapId, (int)flag.XFloat * 1000, (int)flag.YFloat * 1000).AddText(ExcelTerritoryHelper.GetName(flag.TerritoryId)).Add(RawPayload.LinkTerminator).AddText($"<{Player.CurrentWorld}>").AddText(S.SonarMonitor.InstanceNumbers.SafeSelect(inst - 1) ?? "").Build()
						};

            Svc.Chat.Print(m);
				}
				ImGui.SameLine();
				if(ImGui.Button("Switch instance"))
				{
						TaskChangeInstanceAfterTeleport.Enqueue(inst, (int)Player.Territory);
				}
				ImGuiEx.Text($"Utils.CanAutoInstanceSwitch: {Utils.CanAutoInstanceSwitch()}");
				ImGuiEx.Text($"Killed mobs: {P.KilledARanks.Print(",")}");
				if(ImGui.Button("Add killed mob"))
				{
						P.KilledARanks.Add(Enum.GetValues<DawntrailARank>().GetRandom());
				}
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
