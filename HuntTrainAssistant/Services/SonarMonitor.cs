using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.ExcelServices;
using ECommons.EzEventManager;
using ECommons.EzIpcManager;
using ECommons.GameHelpers;
using ECommons.SimpleGui;
using HuntTrainAssistant.DataStructures;
using Lumina.Excel.Sheets;
using PayloadInfo = (Dalamud.Game.Text.SeStringHandling.Payloads.DalamudLinkPayload Payload, uint ID, string World, Lumina.Excel.Sheets.Aetheryte Aetheryte, Dalamud.Game.Text.SeStringHandling.Payloads.MapLinkPayload Link, int Instance);
using UIColor = ECommons.ChatMethods.UIColor;

namespace HuntTrainAssistant.Services;
public class SonarMonitor : IDisposable
{
		private List<PayloadInfo> Payloads = [];
		public ArrivalData Continuation = null;
		public string[] InstanceNumbers = ["", "", ""];

		private SonarMonitor()
		{
				Svc.Chat.ChatMessage += Chat_ChatMessage;
				new EzFrameworkUpdate(ContinueTeleport);
				EzIPC.Init(this);
		}

		public int ParseInstanceNumber(string content)
		{
				PluginLog.Debug($"Parsing instance number from {content}");
				for(int i = 0; i < InstanceNumbers.Length; i++)
				{
						if(content.Contains(InstanceNumbers[i])) return i + 1;
				}
				return 0;
		}

		[EzIPCEvent("HuntAlerts.OnHuntTrainMessageReceived", false)]
		private void OnHuntTrainMessageReceived(HuntTrainMessage message)
		{
				try
        {
            if(Utils.CheckMultiMode()) return;
            PluginLog.Debug($"HTM received: {message}");
						if (P.Config.HuntAlertsIntegration)
						{
								var aetheryte = Svc.Data.GetExcelSheet<Aetheryte>(ClientLanguage.English).FirstOrNull(x => x.GetPlaceName() == message.startLocation);
								if (aetheryte == null)
								{
										PluginLog.Warning($"Received Aetheryte = null from message {message}");
										return;
								}
								var coords = message.locationCoords.Split(", ");
								var payload = new MapLinkPayload(aetheryte.Value.Territory.RowId, aetheryte.Value.Map.RowId, float.Parse(coords[0]), float.Parse(coords[1]));
								var rank = message.huntType switch
								{
										"new_hunt" => Rank.A,
										"srank" => Rank.S,
										_ => throw new ArgumentOutOfRangeException(message.huntType)
								};
								var worldId = ExcelWorldHelper.Get(message.huntWorld)?.RowId ?? 0;

                if(!Svc.Condition[ConditionFlag.BoundByDuty] && !Svc.Condition[ConditionFlag.BoundByDuty56] && !Svc.Condition[ConditionFlag.InDutyQueue] && (!P.Config.WorldBlacklist.Contains(worldId) || Player.CurrentWorldId == worldId))
								{
										HandleAutoTeleport(message.huntWorld, aetheryte.Value, payload, false, rank, ParseExpansion(payload), message.instance);
								}
						}
        }
				catch(Exception ex)
				{
						ex.Log();
						return;
				}
		}

		private void ContinueTeleport()
    {
        if(Continuation != null)
        {
            if(Utils.CheckMultiMode()) return;
            if (Player.Interactable && IsScreenReady() && Player.CurrentWorld == Continuation.World)
						{
								P.TeleportTo = Continuation;
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

		private PayloadInfo CreateLinkPayload(string world, Aetheryte aetheryte, MapLinkPayload link, int instance)
		{
				var id = Payloads.LastOrDefault().ID + 1;
				var payload = Svc.PluginInterface.AddChatLinkHandler(id, HandleLinkPayload);
				var info = (payload, id, world, aetheryte, link, instance);
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
						HandleAutoTeleport(info.World, info.Aetheryte, info.Link, true, default, default, info.Instance);
				}
		}

		public void HandleAutoTeleport(string world, Aetheryte aetheryte, MapLinkPayload payload, bool force, Rank rank, Expansion ex, int instance)
    {
        if(Utils.CheckMultiMode()) return;
        if (!Player.Interactable) return;
				if (Utils.IsInHuntingTerritory() && !force) return;
				if (S.LifestreamIPC.IsBusy()) return;
				if (Continuation != null) return;
        if (!force && P.Config.AutoVisitExpansionsBlacklist.TryGetValue(rank, out var exList) && exList.Contains(ex))
				{
						PluginLog.Debug($"{rank}/{ex} is blacklisted, skipping");
						return;
				}
        if (force || P.Config.AutoVisitTeleportEnabled)
				{
						if (Player.CurrentWorld == world)
						{
								DuoLog.Information($"Same-world teleport: {world}");
								P.TeleportTo = ArrivalData.CreateOrNull(aetheryte, aetheryte.Territory.RowId, instance);
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
												Continuation = new(aetheryte, aetheryte.Territory.RowId, instance)
												{
														World = world,
												};
												DuoLog.Information($"Cross-world teleport: {world}");
												EzConfigGui.Window.IsOpen = true;
										}
										else if ((force || P.Config.AutoVisitCrossDC) && S.LifestreamIPC.CanVisitCrossDC(world))
										{
												S.LifestreamIPC.TPAndChangeWorld(world, true, null, true, null, false, false);
												if (payload != null) Svc.GameGui.OpenMapWithMapLink(payload);
                        Continuation = new(aetheryte, aetheryte.Territory.RowId, instance)
                        {
                            World = world,
                        };
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

		private void Chat_ChatMessage(XivChatType type, int a2, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if(Utils.CheckMultiMode()) return;
        if (P.Config.SonarIntegration && sender.ToString() == "Sonar")
				{
						var messageText = message.GetText().Replace("", "");
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
										var payload = CreateLinkPayload(world.Value.Name.ToString(), aetheryte.Value, link, ParseInstanceNumber(messageText.ToString()));
										message = new SeStringBuilder()
												.Append(message)
												.Append(" ")
												.Add(payload.Payload)
												.AddUiForeground((int)UIColor.Green)
												.Append($"[{GetGoToString(world.Value.Name.ToString())}]")
												.AddUiForegroundOff()
												.Add(RawPayload.LinkTerminator)
												.Build();
								}
								if(world.Value.RowId == Player.CurrentWorldId || !P.Config.WorldBlacklist.Contains(world.Value.RowId))
								{
										HandleAutoTeleport(world.Value.Name.ToString(), aetheryte.Value, link, false, rank, ex, ParseInstanceNumber(message.ToString()));
								}
						}
				}
		}

		public string GetGoToString(string world)
		{
				if (S.LifestreamIPC.CanVisitCrossDC(world)) return $"Go To (Cross-DC)";
				return $"Go To";
		}

		public World? ParseWorldFromMessage(string message)
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
						return ParseExpansion(x);
        }
				return Expansion.Unknown;
		}

		public Expansion ParseExpansion(MapLinkPayload x)
		{
        var bg = x.TerritoryType.ValueNullable?.Bg.ToString();
        if (bg.StartsWith("ex1")) return Expansion.Heavensward;
        if (bg.StartsWith("ex2")) return Expansion.Stormblood;
        if (bg.StartsWith("ex3")) return Expansion.Shadowbringers;
        if (bg.StartsWith("ex4")) return Expansion.Endwalker;
        if (bg.StartsWith("ex5")) return Expansion.Dawntrail;
        return Expansion.ARealmReborn;
    }
}
