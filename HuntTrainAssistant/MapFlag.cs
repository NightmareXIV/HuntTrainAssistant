using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant
{
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct MapFlag
    {
        [FieldOffset(22963)] [MarshalAs(UnmanagedType.I1)] internal bool IsFlagSet;
        [FieldOffset(14320)] internal uint TerritoryType;
        [FieldOffset(14328)] internal float X;
        [FieldOffset(14332)] internal float Y;

        internal static MapFlag* Instance()
        {
            return (MapFlag*)FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUIModule()->GetAgentModule()->GetAgentByInternalId(AgentId.Map);
        }
    }
}
