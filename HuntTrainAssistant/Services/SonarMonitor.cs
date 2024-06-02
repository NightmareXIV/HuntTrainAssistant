using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.ChatMethods;
using ECommons.Configuration;
using ECommons.Events;
using ECommons.ExcelServices;
using ECommons.EzEventManager;
using ECommons.EzIpcManager;
using ECommons.GameHelpers;
using ECommons.SimpleGui;
using HuntTrainAssistant.DataStructures;
using Lumina.Excel.GeneratedSheets;
using PayloadInfo = (Dalamud.Game.Text.SeStringHandling.Payloads.DalamudLinkPayload Payload, uint ID, string World, Lumina.Excel.GeneratedSheets.Aetheryte Aetheryte, Dalamud.Game.Text.SeStringHandling.Payloads.MapLinkPayload Link);
using UIColor = ECommons.ChatMethods.UIColor;

namespace HuntTrainAssistant.Services;
public class SonarMonitor : IDisposable
{
		private List<PayloadInfo> Payloads = [];
		public (Aetheryte Aetheryte, string World)? Continuation = null;

		private SonarMonitor()
		{
				Svc.Chat.ChatMessage += Chat_ChatMessage;
				new EzFrameworkUpdate(ContinueTeleport);
		}

		[EzIPCEvent("HuntAlerts.OnHuntTrainMessageReceived", false)]
		private void OnHuntTrainMessageReceived(HuntTrainMessage message)
		{
				PluginLog.Debug($"HTM received: {message}");
		}

		private void ContinueTeleport()
		{
				if(Continuation != null)
				{
						if (Player.Interactable && IsScreenReady() && Player.CurrentWorld == Continuation.Value.World)
						{
								P.TeleportTo = (Continuation.Value.Aetheryte, Continuation.Value.Aetheryte.Territory.Row);
								EzConfigGui.Window.IsOpen = true;
								Continuation = null;
						}
						EzConfigGui.Window.IsOpen = true;
				}
		}

		public void Dispose()
		{
				Svc.Chat.ChatMessage -= Chat_ChatMessage;
				Svc.PluginInterface.RemoveChatLinkHandler();
		}

		private PayloadInfo CreateLinkPayload(string world, Aetheryte aetheryte, MapLinkPayload link)
		{
				var id = Payloads.LastOrDefault().ID + 1;
				var payload = Svc.PluginInterface.AddChatLinkHandler(id, HandleLinkPayload);
				var info = (payload, id, world, aetheryte, link);
				Payloads.Add(info);
				PluginLog.Information($"Created payload {info}");
				if(Payloads.Count > 100)
				{
						Svc.PluginInterface.RemoveChatLinkHandler(Payloads[0].ID);
						PluginLog.Information($"Deleted first payload {Payloads[0]}");
						Payloads.RemoveAt(0);
				}
				return info;
		}

		private void HandleLinkPayload(uint commandId, SeString message)
		{
				if(Payloads.TryGetFirst(x => x.ID == commandId, out var info))
				{
						HandleAutoTeleport(info.World, info.Aetheryte, info.Link, true);
				}
		}

		public void HandleAutoTeleport(string world, Aetheryte aetheryte, MapLinkPayload payload, bool force = false)
		{
				if (!Player.Interactable) return;
				if (Utils.IsInHuntingTerritory() && !force) return;
				if (S.LifestreamIPC.IsBusy()) return;
				if (Continuation != null) return;
				if (force || P.Config.AutoVisitTeleportEnabled)
				{
						if (Player.CurrentWorld == world)
						{
								DuoLog.Information($"Same-world teleport: {world}");
								P.TeleportTo = (aetheryte, aetheryte.Territory.Row);
								if (payload != null) Svc.GameGui.OpenMapWithMapLink(payload);
								EzConfigGui.Window.IsOpen = true;
						}
						else
						{
								if (force || P.Config.AutoVisitCrossWorld)
								{
										if (S.LifestreamIPC.CanVisitSameDC(world))
										{
												S.LifestreamIPC.TPAndChangeWorld(world, false, null, true, null, false, false);
												if (payload != null) Svc.GameGui.OpenMapWithMapLink(payload);
												Continuation = (aetheryte, world);
												DuoLog.Information($"Cross-world teleport: {world}");
												EzConfigGui.Window.IsOpen = true;
										}
										else if ((force || P.Config.AutoVisitCrossDC) && S.LifestreamIPC.CanVisitCrossDC(world))
										{
												S.LifestreamIPC.TPAndChangeWorld(world, true, null, true, null, false, false);
												if (payload != null) Svc.GameGui.OpenMapWithMapLink(payload);
												Continuation = (aetheryte, world);
												DuoLog.Information($"Cross-DC teleport: {world}");
												EzConfigGui.Window.IsOpen = true;
										}
										else
										{
												DuoLog.Information($"Can not visit {world}");
										}
								}
						}
				}
		}

		private void Chat_ChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
		{
				if (P.Config.SonarIntegration && sender.ToString() == "Sonar")
				{
						var messageText = message.ExtractText().Replace("", "");
						if (messageText.Contains("killed")) return;
						var world = ParseWorldFromMessage(messageText);
						var rank = ParseRankFromMessage(messageText);
						var ex = ParseExpansionFromMessage(message);
						var link = message.Payloads.OfType<MapLinkPayload>().FirstOrDefault();
						var aetheryte = MapManager.GetNearestAetheryte(link);
						PluginLog.Information($"World={world}, rank={rank}, ex={ex}, aetheryte={aetheryte.GetPlaceName()}");
						if (world != null && rank != Rank.Unknown && ex != Expansion.Unknown && aetheryte != null)
						{
								if (P.Config.AutoVisitModifyChat)
								{
										var payload = CreateLinkPayload(world.Name, aetheryte, link);
										message = new SeStringBuilder()
												.Append(message)
												.Append(" ")
												.Add(payload.Payload)
												.AddUiForeground((int)UIColor.Green)
												.Append($"[{GetGoToString(world.Name)}]")
												.AddUiForegroundOff()
												.Add(RawPayload.LinkTerminator)
												.Build();
								}
								HandleAutoTeleport(world.Name.ToString(), aetheryte, link);
						}
				}
		}

		public string GetGoToString(string world)
		{
				if (S.LifestreamIPC.CanVisitCrossDC(world)) return $"Go To (Cross-DC)";
				return $"Go To";
		}

		public World ParseWorldFromMessage(string message)
		{
				foreach(var x in ExcelWorldHelper.GetPublicWorlds())
				{
						if (message.Contains($"<{x.Name}>")) return x;
				}
				return null;
		}

		public Rank ParseRankFromMessage(string message)
		{
				if (message.Contains("Rank SS")) return Rank.SS;
				if (message.Contains("Rank S")) return Rank.S;
				if (message.Contains("Rank A")) return Rank.A;
				return Rank.Unknown;
		}

		public Expansion ParseExpansionFromMessage(SeString message)
		{
				foreach(var x in message.Payloads.OfType<MapLinkPayload>())
				{
						var bg = x.TerritoryType.Bg.ToString();
						if (bg.StartsWith("ex1")) return Expansion.Heavensward;
						if (bg.StartsWith("ex2")) return Expansion.Stormblood;
						if (bg.StartsWith("ex3")) return Expansion.Shadowbringers;
						if (bg.StartsWith("ex4")) return Expansion.Endwalker;
						if (bg.StartsWith("ex5")) return Expansion.Dawntrail;
						return Expansion.ARealmReborn;
				}
				return Expansion.Unknown;
		}
}
