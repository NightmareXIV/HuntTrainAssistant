using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text;
using Lumina.Excel.GeneratedSheets;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace HuntTrainAssistant;

internal unsafe static class ChatMessageHandler
{
    internal static (string Aetheryte, uint Territory) LastMessageLoc = (null, 0);
    internal static void Chat_ChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (Svc.ClientState.LocalPlayer != null && P.config.Enabled && ((type.EqualsAny(XivChatType.Shout, XivChatType.Yell, XivChatType.Say, XivChatType.CustomEmote, XivChatType.StandardEmote) && Svc.ClientState.TerritoryType.EqualsAny(ValidZones)) || P.config.Debug))
        {
            var isMapLink = false;
            var isConductorMessage = (P.config.Debug && (sender.ToString().Contains(Svc.ClientState.LocalPlayer.Name.ToString()) || type == XivChatType.Echo)) || (TryDecodeSender(sender, out var s) && s.Name == P.config.CurrentConductor.Name);
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
                        if (m.TerritoryType.RowId.EqualsAny(ValidZonesInt) || P.config.Debug)
                        {
                            if (m.TerritoryType.RowId != Svc.ClientState.TerritoryType)
                            {
                                if (P.config.AutoTeleport)
                                {
                                    P.TeleportTo = (nearestAetheryte, m.TerritoryType.RowId);
                                    Notify.Info("Engaging Autoteleport");
                                }
                            }
                            if (P.config.AutoOpenMap)
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
                                LastMessageLoc = (MapManager.GetNearestAetheryte(m), m.TerritoryType.RowId);
                            }
                        }
                    }
                    break;
                }
            }
            if (P.config.SuppressChatOtherPlayers && !isMapLink && !isConductorMessage && P.config.CurrentConductor.Name != string.Empty)
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
                if (Framework.Instance()->WindowInactive || P.config.Debug)
                {
                    TryNotify($"{message.ExtractText()}");
                }
            }
        }
    }
}
