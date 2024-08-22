using Dalamud.Game.ClientState.Objects.Types;
using ECommons.Automation.NeoTaskManager;
using ECommons.Configuration;
using ECommons.EzIpcManager;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Reflection;
using ECommons.SimpleGui;
using ECommons.Singletons;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using HuntTrainAssistant.DataStructures;
using HuntTrainAssistant.PluginUI;
using HuntTrainAssistant.Services;
using HuntTrainAssistant.Tasks;
using Lumina.Excel.GeneratedSheets;

namespace HuntTrainAssistant;

public unsafe class HuntTrainAssistant : IDalamudPlugin
{
    internal static HuntTrainAssistant P;
    internal Config Config;
    internal (Aetheryte Aetheryte, uint Territory, int Instance) TeleportTo = default;
    internal bool IsMoving = false;
    internal Vector3 LastPosition = Vector3.Zero;
    public TaskManager TaskManager;
    public int LastInstance = 0;
    public HashSet<DawntrailARank> KilledARanks = [];

    public HuntTrainAssistant(IDalamudPluginInterface pi)
    {
        P = this;
        ECommonsMain.Init(pi, this, Module.DalamudReflector);
        EzConfig.Migrate<Config>();
        Config = EzConfig.Init<Config>();
        EzConfigGui.Init(new MainWindow());
        EzConfigGui.Window.RespectCloseHotkey = false;
        EzCmd.Add("/hta", OnChatCommand, "toggle plugin interface\n/hta clear: clear current conductors\n/hta <player name>: add new conductor");
        Svc.Chat.ChatMessage += ChatMessageHandler.Chat_ChatMessage;
        Svc.Framework.Update += Framework_Update;
        Svc.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        SingletonServiceManager.Initialize(typeof(ServiceManager));
				EzIPC.OnSafeInvocationException += EzIPC_OnSafeInvocationException;
        TaskManager = new(new(timeLimitMS: 60*1000, showDebug:true));
        EzIPC.OnSafeInvocationException += (x) => x.LogInternal();
    }

		private void EzIPC_OnSafeInvocationException(Exception obj)
		{
        InternalLog.Error($"During handling IPC call, exception has occurred: \n{obj}");
		}

		private void ClientState_TerritoryChanged(ushort e)
    {
        LastInstance = 0;
        if(TeleportTo.Instance > 0 && e == TeleportTo.Territory)
        {
            TaskChangeInstanceAfterTeleport.Enqueue(TeleportTo.Instance, (int)TeleportTo.Aetheryte.Territory.Row);
        }
        TeleportTo = default;
        if (!Utils.IsInHuntingTerritory())
        {
            P.Config.Conductors.Clear();
        }
        KilledARanks.Clear();
        PluginLog.Debug($"Cleared killed A ranks list (cs_tt)");
    }

    private void Framework_Update(object framework)
    {
        if(LastInstance != UIState.Instance()->PublicInstance.InstanceId)
        {
            LastInstance = (int)UIState.Instance()->PublicInstance.InstanceId;
            //instance changed event
            KilledARanks.Clear();
            PluginLog.Debug($"Cleared killed A ranks list (inst.ch.)");
        }
        if(P.Config.Conductors.Count > 0 && Utils.IsInHuntingTerritory())
        {
            foreach(var x in Svc.Objects)
            {
                if(x is IBattleNpc b && b.CurrentHp == 0 && Utils.IsNpcIdInARankList(b.NameId) && !KilledARanks.Contains((DawntrailARank)b.NameId))
                {
                    PluginLog.Debug($"Added killed A rank: {(DawntrailARank)b.NameId}. Killed A ranks: {KilledARanks.Print()}");
                    KilledARanks.Add((DawntrailARank)b.NameId);
                }
            }
        }
        if (Player.Interactable && TeleportTo.Aetheryte != null && Svc.ClientState.LocalPlayer.CurrentHp > 0) 
        {
            if (IsScreenReady())
            {
                if (Svc.ClientState.LocalPlayer.IsCasting)
                {
                    if (Svc.ClientState.LocalPlayer.CastActionId == 5)
                    {
                        if (!Svc.Condition[ConditionFlag.Casting])
                        {
                            EzThrottler.Throttle("Teleport", 2000, true);
                        }
                        else
                        {
                            EzThrottler.Throttle("Teleport", 500, true);
                        }
                    }
                    else
                    {
                        EzThrottler.Throttle("Teleport", 500, true);
                    }
                }
                if (Svc.Condition[ConditionFlag.Unknown57])
                {
                    EzThrottler.Throttle("Teleport", 500, true);
                }
                if (!Svc.Condition[ConditionFlag.InCombat] && !Svc.Condition[ConditionFlag.BetweenAreas] && !Svc.Condition[ConditionFlag.BetweenAreas51] && !Svc.Condition[ConditionFlag.Casting] && !IsMoving)
                {
                    if (EzThrottler.Throttle("Teleport"))
                    {
                        if (S.TeleporterIPC.Teleport(TeleportTo.Aetheryte.RowId, 0))
                        {
                            PluginLog.Information($"Teleporting using Teleporter plugin");
                        }
                        else if (S.LifestreamIPC.Teleport(TeleportTo.Aetheryte.RowId))
                        {
                            PluginLog.Information($"Teleporting using Lifestream plugin");
                        }
                        else
                        {
                            PluginLog.Warning($"Failed to teleport. ");
                        }
                    }
                }
            }
            IsMoving = Svc.ClientState.LocalPlayer.Position != LastPosition;
            LastPosition = Svc.ClientState.LocalPlayer.Position;
        }
        if (Svc.Condition[ConditionFlag.BetweenAreas] || Svc.Condition[ConditionFlag.BetweenAreas51])
        {
            if(TeleportTo.Instance > 0)
            {
                TaskChangeInstanceAfterTeleport.Enqueue(TeleportTo.Instance, (int)TeleportTo.Aetheryte.Territory.Row);
            }
            TeleportTo = default;
        }
    }

    public string Name => "HuntTrainAssistant";

    public void Dispose()
    {
        Svc.Chat.ChatMessage -= ChatMessageHandler.Chat_ChatMessage;
        Svc.Framework.Update -= Framework_Update;
        Svc.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
        ECommonsMain.Dispose();
        P = null;
    }

    internal static void TryNotify(string s)
    {
        if (DalamudReflector.TryGetDalamudPlugin("NotificationMaster", out var instance, true, true))
        {
            Safe(delegate
            {
                instance.GetType().Assembly.GetType("NotificationMaster.TrayIconManager", true).GetMethod("ShowToast").Invoke(null, new object[] { s, "HuntTrainAssistant" });
            });
        }
    }
    private void OnChatCommand(string command, string arguments)
    {
        arguments = arguments.Trim();

        if (arguments == string.Empty)
        {
            EzConfigGui.Window.Toggle();
        }
        else if (arguments.StartsWith("clear"))
        {
            P.Config.Conductors.Clear();
        }
        else
        {
            if (arguments.StartsWith("add "))
                arguments = arguments[4..].Trim();
            if (!P.Config.Conductors.Contains(new(arguments, 0)))
                P.Config.Conductors.Add(new(arguments, 0));
            EzConfigGui.Open();
        }
    }
}
