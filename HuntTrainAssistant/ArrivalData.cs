using ECommons.MathHelpers;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant;
public class ArrivalData
{
    public readonly Aetheryte Aetheryte;
    public readonly Number Territory;
    public readonly Number Instance;
    public string World { get; init; }

    public ArrivalData(Aetheryte aetheryte, Number territory, Number instance)
    {
        Aetheryte = aetheryte;
        Territory = territory;
        Instance = instance;
    }

    public static ArrivalData CreateOrNull(Number aetheryte, Number territory, Number instance)
    {
        if(Svc.Data.GetExcelSheet<Aetheryte>().TryGetRow(aetheryte, out var sheet))
        {
            return new(sheet, territory, instance);
        }
        return null;
    }

    public static ArrivalData CreateOrNull(Aetheryte? aetheryte, Number territory, Number instance)
    {
        if(aetheryte != null)
        {
            return new(aetheryte.Value, territory, instance);
        }
        return null;
    }
}
