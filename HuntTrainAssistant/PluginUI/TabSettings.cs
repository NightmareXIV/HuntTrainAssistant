using Dalamud.Memory;
using Dalamud.Memory.Exceptions;
using ECommons.Automation;
using ECommons.Interop;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using NightmareUI.PrimaryUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.PluginUI;
public unsafe class TabSettings
{
		public void Draw()
		{
				if(OpenFileDialog.IsSelecting())
				{
						ImGuiEx.Text("Waiting for file selection...");
						return;
				}
				new NuiBuilder().
						Section("General settings")
						.Widget(() =>
						{
								ImGui.Checkbox("Plugin enabled", ref P.Config.Enabled);
								ImGui.SameLine();
								ImGui.Checkbox("Debug mode", ref P.Config.Debug);
								ImGui.Checkbox("Autoteleport to different zone", ref P.Config.AutoTeleport);
                ImGui.Indent();
                ImGui.Checkbox("Auto-switch to instance 1 after teleporting", ref P.Config.AutoSwitchInstanceToOne);
                ImGui.Unindent();
                ImGui.Checkbox("Auto-open map when new location is linked", ref P.Config.AutoOpenMap);
								ImGui.Indent();
								ImGui.Checkbox("Don't open flag that was previously open", ref P.Config.NoDuplicateFlags);
								ImGui.Unindent();
								ImGui.Checkbox("When conductor is set, suppress other people's messages", ref P.Config.SuppressChatOtherPlayers);
								ImGui.Checkbox("Compensate for some aetherytes' position", ref P.Config.DistanceCompensationHack);
								ImGui.Checkbox("Auto-teleport to next instance if two A-marks killed", ref P.Config.AutoSwitchInstanceTwoRanks);
								ImGui.Checkbox("Context menu integration", ref P.Config.ContextMenu);
								ImGui.Checkbox("Enable one-click party finder creation button", ref P.Config.PfinderEnable);
								ImGui.Indent();
								ImGuiEx.Text($"Party finder comment");
								ImGuiEx.SetNextItemFullWidth();
								ImGui.InputText($"##pfindercommenr", ref P.Config.PfinderString, 150);
								ImGui.Unindent();
								ImGui.Checkbox("Enable random auto-teleport delay", ref P.Config.TeleportDelayEnabled);
								ImGui.Indent();
								ImGui.SetNextItemWidth(150f);
                ImGuiEx.SliderIntAsFloat("Minimum delay", ref P.Config.TeleportDelayMin, 0, 1000);
                ImGui.SetNextItemWidth(150f);
                ImGuiEx.SliderIntAsFloat("Maximum delay", ref P.Config.TeleportDelayMax, 0, 1000);
                ImGui.Unindent();
						})
						.Section("Notifications")
						.Widget(() =>
						{
								ImGuiEx.Text("NotificationMaster plugin required");
								ImGuiEx.PluginAvailabilityIndicator([new("NotificationMaster"), new("NotificationMaster.NXIV", "NotificationMaster (from NightmareXIV repo)")], "", false);
								ImGui.Checkbox("Play audio when conductor posts message", ref P.Config.AudioAlert);
								ImGui.Indent();
								ImGuiEx.InputWithRightButtonsArea(() => ImGui.InputTextWithHint("##pathToAudio", "Path to audio file", ref P.Config.AudioAlertPath, 500), () =>
								{
										if(ImGui.Button("Select..."))
										{
												OpenFileDialog.SelectFile((x) =>
												{
														if(x != null) new TickScheduler(() => P.Config.AudioAlertPath = x.file);
												});
										}
										ImGui.SameLine();
										if(ImGuiEx.IconButton(FontAwesomeIcon.Play))
										{
												S.Notificator.PlaySound(P.Config.AudioAlertPath, P.Config.AudioAlertVolume, false, false);
										}
								});
								ImGui.SetNextItemWidth(150f);
								ImGui.SliderFloat("Volume", ref P.Config.AudioAlertVolume, 0f, 1f);
								ImGui.Checkbox("Play only when game is not minimized", ref P.Config.AudioAlertOnlyMinimized);
                ImGui.SetNextItemWidth(150f);
                ImGuiEx.SliderIntAsFloat("Minimal interval between audio notifications", ref P.Config.AudioThrottle, 0, 10000);
                ImGui.Unindent();
                ImGui.Checkbox("Flash taskbar on conductor message", ref P.Config.FlashTaskbar);
								ImGui.Checkbox("Show tray popup notification on conductor message", ref P.Config.TrayNotification);
            })
						.Section("Triggers")
						.Widget(() =>
						{
								ImGui.Checkbox("Execute macro after receiving conductor's message with flag", ref P.Config.ExecuteMacroOnFlag);
								if(P.Config.ExecuteMacroOnFlag)
								{
										ImGui.Indent();
										var m = RaptureMacroModule.Instance();
										var macroName = "Not set";
										if(P.Config.MacroIndex >= 0 && P.Config.MacroIndex < m->Shared.Length)
										{
												var macro = m->Shared[P.Config.MacroIndex];
												if(macro.IsNotEmpty())
												{
														macroName = MemoryHelper.ReadSeString(&macro.Name).ToString();
												}
										}
										if(ImGui.BeginCombo("Select System Macro", macroName, ImGuiComboFlags.HeightLarge))
										{
												for(int i = 0; i < m->Shared.Length; i++)
												{
														if(m->Shared[i].IsNotEmpty())
														{
                                var macro = m->Shared[i];
                                macroName = MemoryHelper.ReadSeString(&macro.Name).ToString();
																if(ImGui.Selectable($"#{i + 1}: {macroName}", i == P.Config.MacroIndex))
																{
																		P.Config.MacroIndex = i;
																}
														}
												}
												ImGui.EndCombo();
										}
										ImGui.Unindent();
								}
						})
						.Draw();
		}
}
