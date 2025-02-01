using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Ipc.Exceptions;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.UIHelpers.AddonMasterImplementations;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.Interop;
using HuntTrainAssistant.DataStructures;
using HuntTrainAssistant.Tasks;
using Lumina.Excel.Sheets;
using NightmareUI;
using NightmareUI.ImGuiElements;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkEventDispatcher;

namespace HuntTrainAssistant.PluginUI;
public unsafe class TabDebug
{
		public List<byte[]> TestMessages = [];
    public void Draw()
    {
        ref var str = ref Ref<string>.Get("lfgtest");
        ImGui.InputText("text", ref str, 150);
        if(ImGui.Button("set text")) S.LFGService.SetComment(str);
        if(TryGetAddonMaster<AddonMaster.LookingForGroupCondition>(out var ms))
				{
						ref var test = ref Ref<int>.Get("lfgtest");
						if(ImGui.Button("Normal")) ms.Normal();
						ImGui.InputInt("x", ref test);
						if(ImGui.Button("Select group")) ms.SelectDutyCategory((byte)test);
						//var b = ms.RemoveRoleRestriction;
						//if(ImGui.Checkbox("RemoveRoleRestriction", ref b)) ms.RemoveRoleRestriction = b;
				}

				ref var inst = ref Ref<int>.Get("Instance");
				ImGui.SetNextItemWidth(100f);
				ImGui.InputInt("Instance", ref inst);
				ImGui.SameLine();
				ref var world = ref Ref<int>.Get("World");
				ImGui.SetNextItemWidth(100f);
				WorldSelector.Instance.Draw(ref world);
				if(ImGui.Button("Fake Sonar message"))
				{
						var flag = AgentMap.Instance()->FlagMapMarker;
						var m = new XivChatEntry
						{
								Name = "Sonar",
								Message = new SeStringBuilder().AddText("Rank S: ").AddMapLink(flag.TerritoryId, flag.MapId, (int)flag.XFloat * 1000, (int)flag.YFloat * 1000).AddText(ExcelTerritoryHelper.GetName(flag.TerritoryId)).Add(RawPayload.LinkTerminator).AddText($"<{ExcelWorldHelper.GetName(world)}>").AddText(S.SonarMonitor.InstanceNumbers.SafeSelect(inst - 1) ?? "").Build()
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
				ImGuiEx.Text($"Is moving: {P.IsMoving}");
				if (ImGui.CollapsingHeader("Territory"))
				{
						foreach (var x in Svc.Data.GetExcelSheet<TerritoryType>())
						{
								try
								{
										ImGuiEx.Text($"{x.RowId}: {x.PlaceName.ValueNullable?.Name} / {x.TerritoryIntendedUse}");
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
										ImGuiEx.Text($"{x.RowId}: {x.PlaceName.ValueNullable?.Name} / {x.AethernetName.ValueNullable?.Name}");
								}
								catch (Exception e)
								{
										ImGuiEx.Text($"{e.Message}");
								}
						}
				}
		}
}
