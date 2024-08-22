using ECommons.Automation;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.Tasks;
public static class TaskChangeInstanceAfterTeleport
{
    public static void Enqueue(int num, int territory)
    {
        P.TaskManager.Enqueue(() => Player.Territory == territory && Player.Interactable);
        P.TaskManager.Enqueue(() =>
        {
            if(S.LifestreamIPC.GetNumberOfInstances() == 0 || num == 0 || S.LifestreamIPC.GetCurrentInstance() == num) return null;
            return true;
        });
        P.TaskManager.Enqueue(() => IsScreenReady() && Player.Interactable);
        P.TaskManager.Enqueue(() =>
        {
            if(!S.LifestreamIPC.CanChangeInstance())
            {
                var nearestAetheryte = Svc.Objects.Where(x => x.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Aetheryte && x.IsTargetable).OrderBy(x => Vector3.Distance(Player.Position, x.Position)).FirstOrDefault();
                if(nearestAetheryte != null)
                {
                    if(nearestAetheryte.IsTarget() && EzThrottler.Throttle("Lockon"))
                    {
                        Chat.Instance.ExecuteCommand("/lockon");
                        P.TaskManager.Insert(() => Chat.Instance.ExecuteCommand("/automove on"));
                        return true;
                    }
                    else
                    {
                        if(EzThrottler.Throttle("SetTarget"))
                        {
                            Svc.Targets.Target = nearestAetheryte;
                        }
                        return false;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return true;
            }
        });
        P.TaskManager.Enqueue(() =>
        {
            if(S.LifestreamIPC.GetCurrentInstance() == num) return true;
            if(S.LifestreamIPC.CanChangeInstance())
            {
                Chat.Instance.ExecuteCommand("/automove off");
                S.LifestreamIPC.ChangeInstance(num);
                return true;
            }
            return false;
        }, new(timeLimitMS:15000));
    }
}
