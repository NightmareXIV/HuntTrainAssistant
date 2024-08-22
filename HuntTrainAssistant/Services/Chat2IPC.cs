using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Game.Text.SeStringHandling;
using ECommons.EzIpcManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommons.ChatMethods;
using ECommons.SimpleGui;

namespace HuntTrainAssistant.Services;
#pragma warning disable
public class Chat2IPC : IDisposable
{
    string? CurrentID;
    [EzIPC] Func<string> Register;
    [EzIPC] Action<string> Unregister;

    [EzIPCEvent]
    void Available()
    {
        CurrentID = Register();
        PluginLog.Debug($"Chat2 id={CurrentID}");
    }

    [EzIPCEvent]
    void Invoke(string id, PlayerPayload? snd, ulong contentId, Payload? payload, SeString? senderString, SeString? content)
    {
        if(!P.Config.ContextMenu) return;
        if(id == CurrentID && snd != null)
        {
            if(ImGui.Selectable("[HTA] Set as conductor"))
            {
                var player = snd.PlayerName.ToString();
                var world = snd.World.RowId;
                var s = new Sender(player, world);
                P.Config.Conductors.Add(s);
                EzConfigGui.Open();
            }
        }
    }

    private Chat2IPC()
    {
        EzIPC.Init(this, "ChatTwo", SafeWrapper.AnyException);
        Available();
    }

    public void Dispose()
    {
        if(CurrentID != null) Unregister(CurrentID);
    }
}
