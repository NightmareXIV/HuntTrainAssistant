using ECommons.EzIpcManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.Services;
public class AutoRetainerIPC
{
    private AutoRetainerIPC()
    {
        EzIPC.Init(this, "AutoRetainer.PluginState", SafeWrapper.AnyException);
    }

    [EzIPC("AutoRetainer.GC.EnqueueInitiation", false)] public readonly Action EnqueueInitiation;

    [EzIPC("AbortAllTasks")] public readonly Action AbortAllTasks;
    [EzIPC("DisableAllFunctions")] public readonly Action DisableAllFunctions;
    [EzIPC("EnableMultiMode")] public readonly Action EnableMultiMode;
    [EzIPC("IsBusy")] public readonly Func<bool> IsBusy;
    [EzIPC("GetInventoryFreeSlotCount")] public readonly Func<int> GetInventoryFreeSlotCount;
    [EzIPC("EnqueueHET")] public readonly Action<bool, bool> EnqueueHET;
    [EzIPC("IsItemProtected")] public readonly Func<uint, bool> IsItemProtected;
    [EzIPC("GetMultiModeStatus")] public readonly Func<bool> GetMultiModeStatus;
}
