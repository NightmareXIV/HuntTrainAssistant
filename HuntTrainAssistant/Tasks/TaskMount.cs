using ECommons.Automation;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.Tasks;
public static unsafe class TaskMount
{
    public static void EnqueueIfEnabled()
    {
        if(P.Config.UseMount)
        {
            P.TaskManager.Enqueue(() => !S.LifestreamIPC.IsBusy() && IsScreenReady() && Player.Interactable, "Wait for player");
            P.TaskManager.Enqueue(MountIfCan);
        }
    }

    public static bool MountIfCan()
    {
        if(Svc.Condition[ConditionFlag.Mounted])
        {
            return true;
        }
        if(P.Config.Mount == -1) return true;
        if(Svc.Condition[ConditionFlag.MountOrOrnamentTransition] || Svc.Condition[ConditionFlag.Casting])
        {
            EzThrottler.Throttle("CheckMount", 2000, true);
        }
        if(!EzThrottler.Check("CheckMount")) return false;
        if(ActionManager.Instance()->GetActionStatus(ActionType.GeneralAction, 9) == 0)
        {
            var mount = P.Config.Mount;
            if(mount == 0 || !PlayerState.Instance()->IsMountUnlocked((uint)mount))
            {
                var mounts = Svc.Data.GetExcelSheet<Mount>().Where(x => x.Singular != "" && PlayerState.Instance()->IsMountUnlocked(x.RowId));
                if(mounts.Any())
                {
                    if(mounts.Count() == 1)
                    {
                        var newMount = (int)mounts.First().RowId;
                        PluginLog.Warning($"Mount {Utils.GetMountName(mount)} is not unlocked. Selecting {Utils.GetMountName(newMount)}.");
                        mount = newMount;
                    }
                    else
                    {
                        mount = 0;
                    }
                }
                else
                {
                    PluginLog.Warning("No unlocked mounts found");
                    return true;
                }
            }
            if(!Player.IsAnimationLocked && EzThrottler.Throttle("SummonMount"))
            {
                if(mount == 0)
                {
                    Chat.ExecuteGeneralAction(9);
                }
                else
                {
                    Chat.ExecuteCommand($"/mount \"{Utils.GetMountName(mount)}\"");
                }
            }
        }
        else
        {
            return true;
        }
        return false;
    }
}