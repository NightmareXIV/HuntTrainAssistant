using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.Logging;
using Lumina.Excel.Sheets;

namespace HuntTrainAssistant;

internal static class MapManager
{

    public static string GetPlaceName(this Aetheryte? aetheryte)
    {
        if(aetheryte == null) return null;
        return aetheryte?.PlaceName.ValueNullable?.Name.ToString();
    }
    public static string GetPlaceName(this Aetheryte aetheryte)
    {
        return aetheryte.PlaceName.ValueNullable?.Name.ToString();
    }

    internal static Aetheryte? GetNearestAetheryte(MapLinkPayload maplinkMessage)
    {
        if(maplinkMessage == null) return null;
				Aetheryte? aetheryte = null;
        double distance = 0;
        PluginLog.Debug($"Link: {maplinkMessage.PlaceName} ({maplinkMessage.XCoord:0.00} : {maplinkMessage.YCoord:0.00})");
        foreach (var data in Svc.Data.GetExcelSheet<Aetheryte>())
        {
            if (!data.IsAetheryte) continue;
            if (data.Territory.ValueNullable == null) continue;
            if (data.PlaceName.ValueNullable == null) continue;
            if (data.RowId.EqualsAny(P.Config.AetheryteBlacklist)) continue;
            if (Svc.Data.GetExcelSheet<Map>().TryGetFirst(m => m.TerritoryType.RowId == maplinkMessage.TerritoryType.RowId, out var place))
            {
                var scale = place.SizeFactor;
                if (data.Territory.Value.RowId == maplinkMessage.TerritoryType.RowId)
                {
                    var mapMarker = Svc.Data.GetSubrowExcelSheet<MapMarker>().AllRows().FirstOrNull(m => (m.DataType == 3 && m.DataKey.RowId == data.RowId));
                    if (mapMarker == null)
                    {
                        DuoLog.Error($"Cannot find aetherytes position for {maplinkMessage.PlaceName}#{data.PlaceName.Value.Name}");
                        continue;
                    }
                    Vector2 compensationDelta = getDistanceCompensationHackDelta(data.PlaceName.ValueNullable?.Name.ToString());
                    var AethersX = ConvertMapMarkerToMapCoordinate(mapMarker.Value.X, scale) + compensationDelta.X;
                    var AethersY = ConvertMapMarkerToMapCoordinate(mapMarker.Value.Y, scale) + compensationDelta.Y;
                    double temp_distance = Math.Pow(AethersX - maplinkMessage.XCoord, 2) + Math.Pow(AethersY - maplinkMessage.YCoord, 2);
                    PluginLog.Debug($"Aetheryte: {data.PlaceName.Value.Name} ({AethersX:0.00}, {AethersY:0.00}), distance to flag: {temp_distance:0.00}");
                    if (aetheryte == null || temp_distance < distance)
                    {
                        distance = temp_distance;
												aetheryte = data;
                    }
                }
            }
        }
        return aetheryte;
    }

    internal static Vector2 getDistanceCompensationHackDelta(string AetheryteName)
    {
        float X = 0f;
        float Y = 0f;
        if (P.Config.DistanceCompensationHack)
        {
            // Distance hacks to account for Aetheryte's Z value (or weird exits like Tertium)
            switch (AetheryteName)
            {
                case "Tertium":
                    // only two spawn points are actually close enough to it
                    Y -= 5f;
                    break;
                case "Base Omicron":
                    // no spawn points closest to it compared to other two aetherytes
                    X += 5f;
                    break;
                case "Bestways Burrow":
                    // height difference only makes it closer to two points
                    Y -= 3f;
                    X -= 2f;
                    break;
                case "The Great Work":
                    // one spawn point is closer to it than Palaka accounting for height
                    Y -= 2f;
                    break;
                case "The Macarenses Angle":
                    // never worth teleporting to, not even for the spawn point that's right above, since upward movement is very slow
                    Y += 999f;
                    break;
            }
        }
        return new Vector2()
        {
            X = X,
            Y = Y
        };
    }

    internal static float ConvertMapMarkerToMapCoordinate(int pos, float scale)
    {
        float num = scale / 100f;
        var rawPosition = (int)((float)(pos - 1024.0) / num * 1000f);
        return ConvertRawPositionToMapCoordinate(rawPosition, scale);
    }

    internal static float ConvertRawPositionToMapCoordinate(int pos, float scale)
    {
        float num = scale / 100f;
        return (float)((pos / 1000f * num + 1024.0) / 2048.0 * 41.0 / num + 1.0);
    }

    internal static double ToMapCoordinate(double val, float scale)
    {
        var c = scale / 100.0;

        val *= c;
        return ((41.0 / c) * ((val + 1024.0) / 2048.0)) + 1;
    }
}
