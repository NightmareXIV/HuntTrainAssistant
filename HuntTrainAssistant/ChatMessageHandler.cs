using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text;
using Lumina.Excel.GeneratedSheets;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace HuntTrainAssistant;

internal unsafe static class ChatMessageHandler
{
    internal static (Aetheryte Aetheryte, uint Territory, int Instance) LastMessageLoc = default;
    internal static void Chat_ChatMessage(XivChatType type, int a2, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        var conductorNames = P.Config.Conductors.Select(x => x.Name).ToList();
        if (Svc.ClientState.LocalPlayer != null && P.Config.Enabled && ((type.EqualsAny(XivChatType.Shout, XivChatType.Yell, XivChatType.Say, XivChatType.CustomEmote, XivChatType.StandardEmote, XivChatType.Echo) && Utils.IsInHuntingTerritory()) || P.Config.Debug))
        {
            var isMapLink = false;
            var isConductorMessage = (P.Config.Debug && (sender.ToString().Contains(Svc.ClientState.LocalPlayer.Name.ToString()) || type == XivChatType.Echo)) || (TryDecodeSender(sender, out var s) && conductorNames.Contains(s.Name));
            //InternalLog.Debug($"Message: {message.ToString()} from {sender}, isConductor = {isConductorMessage}");
            foreach (var x in message.Payloads)
            {
                if (x is MapLinkPayload m)
                {
                    isMapLink = true;
                    if (isConductorMessage)
                    {
                        var nearestAetheryte = MapManager.GetNearestAetheryte(m);
                        //PluginLog.Debug($"{m}");
                        if (Utils.IsInHuntingTerritory() || P.Config.Debug)
                        {
                            if(P.Config.AutoTeleport)
                            {
                                if(m.TerritoryType.RowId != Svc.ClientState.TerritoryType)
                                {
                                    P.TeleportTo = (nearestAetheryte, m.TerritoryType.RowId, 0);
                                    Notify.Info("Engaging Autoteleport");
                                }
                                else if(Utils.CanAutoInstanceSwitch() && P.Config.AutoSwitchInstanceTwoRanks && S.LifestreamIPC.GetCurrentInstance() < S.LifestreamIPC.GetNumberOfInstances())
                                {
                                    P.TeleportTo = (nearestAetheryte, m.TerritoryType.RowId, S.LifestreamIPC.GetCurrentInstance() + 1);
                                    Notify.Info("Engaging Autoteleport");
                                }
                            }
                            if (P.Config.AutoOpenMap)
                            {
                                if (MapFlag.Instance()->IsFlagSet && MapFlag.Instance()->TerritoryType == m.TerritoryType.RowId)
                                {
                                    if (Svc.Data.GetExcelSheet<Map>().TryGetFirst(x => x.TerritoryType.Row == m.TerritoryType.RowId, out var place))
                                    {
                                        var pos = new Vector2(m.RawX / 1000, m.RawY / 1000);
                                        var distance = Vector2.Distance(new(MapFlag.Instance()->X, MapFlag.Instance()->Y), pos);
                                        PluginLog.Information($"Distance between map marker and linked position is {distance}");
                                        if(distance > 10)
                                        {
                                            Svc.GameGui.OpenMapWithMapLink(m);
                                        }
                                    }
                                }
                                else
                                {
                                    Svc.GameGui.OpenMapWithMapLink(m);
                                }
                                LastMessageLoc = (MapManager.GetNearestAetheryte(m), m.TerritoryType.RowId, 0);
                            }
                        }
                    }
                    break;
                }
            }
            if (P.Config.SuppressChatOtherPlayers && !isMapLink && !isConductorMessage && conductorNames.Count > 0)
            {
                isHandled = true;
            }
            if (isConductorMessage)
            {
                var msg = new SeStringBuilder();
                msg.AddUiForeground(578);
                foreach (var x in message.Payloads)
                {
                    msg.Add(x);
                }
                msg.AddUiForegroundOff();
                message = msg.Build();
                if (Framework.Instance()->WindowInactive || P.Config.Debug)
                {
                    TryNotify($"{message.ExtractText()}");
                }
            }
        }
    }
}
