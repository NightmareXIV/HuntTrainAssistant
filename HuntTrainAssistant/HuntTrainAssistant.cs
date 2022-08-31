using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons;
using ECommons.SimpleGui;
using Lumina.Excel.GeneratedSheets;

namespace HuntTrainAssistant
{
    public class HuntTrainAssistant : IDalamudPlugin
    {
        internal static HuntTrainAssistant P;
        internal Config config;
        internal (string Name, uint Territory) TeleportTo = (null, 0);
        internal long NextCommandAt = 0;

        public HuntTrainAssistant(DalamudPluginInterface pi)
        {
            P = this;
            ECommons.ECommons.Init(pi);
            config = Svc.PluginInterface.GetPluginConfig() as Config ?? new();
            ConfigGui.Init(this.Name, Gui.Draw, config);
            EzCmd.Add("/hta", ConfigGui.Open, "open plugin interface");
            Svc.Chat.ChatMessage += Chat_ChatMessage;
            Svc.Framework.Update += Framework_Update;
            Svc.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        }

        private void ClientState_TerritoryChanged(object sender, ushort e)
        {
            TeleportTo = (null, 0);
        }

        private void Framework_Update(Dalamud.Game.Framework framework)
        {
            if (Svc.ClientState.LocalPlayer != null && TeleportTo.Territory != 0) 
            {
                if (!Svc.Condition[ConditionFlag.InCombat] && !Svc.Condition[ConditionFlag.BetweenAreas] && !Svc.Condition[ConditionFlag.BetweenAreas51] && !Svc.Condition[ConditionFlag.Casting])
                {
                    if (Environment.TickCount64 > NextCommandAt)
                    {
                        NextCommandAt = Environment.TickCount64 + 500;
                        Svc.Commands.ProcessCommand($"/tp {TeleportTo.Name}");
                    }
                }
                if (Svc.ClientState.LocalPlayer.IsCasting && Svc.ClientState.LocalPlayer.CastActionId == 5)
                {
                    NextCommandAt = Environment.TickCount64 + 3000;
                }
            }
        }

        private void Chat_ChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if(config.Enabled && ((type.EqualsAny(XivChatType.Shout, XivChatType.Yell, XivChatType.Say) && TryDecodeSender(sender, out var s) && s.Name == P.config.CurrentConductor.Name && Svc.ClientState.TerritoryType.EqualsAny<ushort>(956, 957, 958, 959, 960, 961)) || P.config.Debug))
            {
                foreach (var x in message.Payloads)
                {
                    if (x is MapLinkPayload m)
                    {
                        var nearestAetheryte = GetNearestAetheryte(m);
                        PluginLog.Debug($"{m}");
                        if(m.TerritoryType.RowId != Svc.ClientState.TerritoryType && m.TerritoryType.RowId.EqualsAny<uint>(956, 957, 958, 959, 960, 961))
                        {
                            TeleportTo = (nearestAetheryte, m.TerritoryType.RowId);
                        }
                    }
                }
            }
        }

        public string GetNearestAetheryte(MapLinkPayload maplinkMessage)
        {
            string aetheryteName = "";
            double distance = 0;
            foreach (var data in Svc.Data.GetExcelSheet<Aetheryte>())
            {
                if (!data.IsAetheryte) continue;
                if (data.Territory.Value == null) continue;
                if (data.PlaceName.Value == null) continue;
                var place = Svc.Data.GetExcelSheet<Map>().FirstOrDefault(m => m.TerritoryType.Row == maplinkMessage.TerritoryType.RowId);
                var scale = place.SizeFactor;
                if (data.Territory.Value.RowId == maplinkMessage.TerritoryType.RowId)
                {
                    var mapMarker = Svc.Data.GetExcelSheet<MapMarker>().FirstOrDefault(m => (m.DataType == 3 && m.DataKey == data.RowId));
                    if (mapMarker == null)
                    {
                        DuoLog.Error($"Cannot find aetherytes position for {maplinkMessage.PlaceName}#{data.PlaceName.Value.Name}");
                        continue;
                    }
                    var AethersX = ConvertMapMarkerToMapCoordinate(mapMarker.X, scale);
                    var AethersY = ConvertMapMarkerToMapCoordinate(mapMarker.Y, scale);
                    PluginLog.Debug($"Aetheryte: {data.PlaceName.Value.Name} ({AethersX} ,{AethersY})");
                    double temp_distance = Math.Pow(AethersX - maplinkMessage.XCoord, 2) + Math.Pow(AethersY - maplinkMessage.YCoord, 2);
                    if (aetheryteName == "" || temp_distance < distance)
                    {
                        distance = temp_distance;
                        aetheryteName = data.PlaceName.Value.Name;
                    }
                }
            }
            return aetheryteName;
        }

        float ConvertMapMarkerToMapCoordinate(int pos, float scale)
        {
            float num = scale / 100f;
            var rawPosition = (int)((float)(pos - 1024.0) / num * 1000f);
            return ConvertRawPositionToMapCoordinate(rawPosition, scale);
        }

        float ConvertRawPositionToMapCoordinate(int pos, float scale)
        {
            float num = scale / 100f;
            return (float)((pos / 1000f * num + 1024.0) / 2048.0 * 41.0 / num + 1.0);
        }

        double ToMapCoordinate(double val, float scale)
        {
            var c = scale / 100.0;

            val *= c;
            return ((41.0 / c) * ((val + 1024.0) / 2048.0)) + 1;
        }

        public string Name => "HuntTrainAssistant";

        public void Dispose()
        {
            Svc.Chat.ChatMessage -= Chat_ChatMessage;
            Svc.Framework.Update -= Framework_Update;
            Svc.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
            ECommons.ECommons.Dispose();
            P = null;
        }
    }
}