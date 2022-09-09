using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text;

namespace HuntTrainAssistant;

internal static class ChatMessageHandler
{
    internal static void Chat_ChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (Svc.ClientState.LocalPlayer != null && P.config.Enabled && ((type.EqualsAny(XivChatType.Shout, XivChatType.Yell, XivChatType.Say) && Svc.ClientState.TerritoryType.EqualsAny(ValidZones)) || P.config.Debug))
        {
            var isMapLink = false;
            var isConductorMessage = (P.config.Debug && sender.ToString().Contains(Svc.ClientState.LocalPlayer.Name.ToString())) || (TryDecodeSender(sender, out var s) && s.Name == P.config.CurrentConductor.Name);
            foreach (var x in message.Payloads)
            {
                if (x is MapLinkPayload m)
                {
                    isMapLink = true;
                    if (isConductorMessage)
                    {
                        var nearestAetheryte = MapManager.GetNearestAetheryte(m);
                        PluginLog.Debug($"{m}");
                        if (m.TerritoryType.RowId.EqualsAny(ValidZonesInt) || P.config.Debug)
                        {
                            if (m.TerritoryType.RowId != Svc.ClientState.TerritoryType)
                            {
                                if (P.config.AutoTeleport)
                                {
                                    P.TeleportTo = (nearestAetheryte, m.TerritoryType.RowId);
                                }
                            }
                            else
                            {
                                if (P.config.AutoOpenMap)
                                {
                                    Svc.GameGui.OpenMapWithMapLink(m);
                                }
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
            }
        }
    }
}
