using ECommons.GameFunctions;
using ECommons.Reflection;
using ECommons.SimpleGui;

namespace HuntTrainAssistant;

public unsafe class HuntTrainAssistant : IDalamudPlugin
{
    internal static HuntTrainAssistant P;
    internal Config config;
    internal (string Name, uint Territory) TeleportTo = (null, 0);
    internal long NextCommandAt = 0;
    internal static ushort[] ValidZones = { 956, 957, 958, 959, 960, 961 };
    internal static uint[] ValidZonesInt = { 956, 957, 958, 959, 960, 961 };
    ContextMenuManager contextMenuManager;
    internal bool IsMoving = false;
    internal Vector3 LastPosition = Vector3.Zero;
    

    public HuntTrainAssistant(DalamudPluginInterface pi)
    {
        P = this;
        ECommonsMain.Init(pi, this, Module.DalamudReflector);
        config = Svc.PluginInterface.GetPluginConfig() as Config ?? new();
        EzConfigGui.Init(Gui.Draw, config);
        EzConfigGui.Window.RespectCloseHotkey = false;
        EzCmd.Add("/hta", EzConfigGui.Open, "open plugin interface");
        Svc.Chat.ChatMessage += ChatMessageHandler.Chat_ChatMessage;
        Svc.Framework.Update += Framework_Update;
        Svc.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        contextMenuManager = new();
    }

    private void ClientState_TerritoryChanged(object sender, ushort e)
    {
        TeleportTo = (null, 0);
        if (!e.EqualsAny(ValidZones))
        {
            P.config.Conductors.Clear();
        }
    }

    private void Framework_Update(Dalamud.Game.Framework framework)
    {
        if (Svc.ClientState.LocalPlayer != null && TeleportTo.Territory != 0 && Svc.ClientState.LocalPlayer.CurrentHp > 0) 
        {
            if (!Svc.Condition[ConditionFlag.InCombat] && !Svc.Condition[ConditionFlag.BetweenAreas] && !Svc.Condition[ConditionFlag.BetweenAreas51] && !Svc.Condition[ConditionFlag.Casting] && !IsMoving)
            {
                if (Environment.TickCount64 > NextCommandAt)
                {
                    NextCommandAt = Environment.TickCount64 + 500;
                    Svc.Commands.ProcessCommand($"/tp {TeleportTo.Name}");
                }
            }
            if (Svc.ClientState.LocalPlayer.IsCasting && Svc.ClientState.LocalPlayer.CastActionId == 5)
            {
                if (!Svc.Condition[ConditionFlag.Casting])
                {
                    NextCommandAt = Environment.TickCount64 + 2000;
                }
                else
                {
                    NextCommandAt = Environment.TickCount64 + 500;
                }
            }
            if(Svc.Condition[ConditionFlag.BetweenAreas] || Svc.Condition[ConditionFlag.BetweenAreas51])
            {
                TeleportTo = (null, 0);
            }
            IsMoving = Svc.ClientState.LocalPlayer.Position != LastPosition;
            LastPosition = Svc.ClientState.LocalPlayer.Position;
        }
    }

    public string Name => "HuntTrainAssistant";

    public void Dispose()
    {
        contextMenuManager.Dispose();
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
}