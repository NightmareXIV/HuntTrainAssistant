using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Memory;
using ECommons.Automation;
using ECommons.EzHookManager;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace HuntTrainAssistant.Services;
public unsafe class LFGService : IDisposable
{
    delegate nint AgentLookingForGroup_ReceiveEvent(nint a1, nint a2, nint a3, uint a4, nint a5);
    [EzHook("48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC 40 45 8B D1")]
    EzHook<AgentLookingForGroup_ReceiveEvent> AgentLookingForGroup_ReceiveEventHook;
    private LFGService()
    {
        Svc.AddonLifecycle.RegisterListener(AddonEvent.PostReceiveEvent, "LookingForGroupCondition", OnReceiveEvent);
        EzSignatureHelper.Initialize(this);
    }

    public void Dispose()
    {
        Svc.AddonLifecycle.RegisterListener(AddonEvent.PostReceiveEvent, "LookingForGroupCondition", OnReceiveEvent);
    }

    nint AgentLookingForGroup_ReceiveEventDetour(nint a1, nint a2, nint a3, uint a4, nint a5)
    {
        try
        {
            if(a5 == 3)
            {
                PluginLog.Information($"AgentLookingForGroup_ReceiveEventDetour: {a2:X}б {a3:X}, {a4}, {a5:X}");
                var str = (AtkValue*)(a2 + 16);
                PluginLog.Information($"   {MemoryHelper.ReadStringNullTerminated((nint)(a1+ 9680))}/{MemoryHelper.ReadRaw(a3, 40).ToArray().ToHexString()}");
            }
        }
        catch(Exception e) { e.Log(); }
        return AgentLookingForGroup_ReceiveEventHook.Original(a1, a2, a3, a4, a5);
    }

    private void OnReceiveEvent(AddonEvent type, AddonArgs args)
    {
        var evt = (AddonReceiveEventArgs)args;
        var atkEvent = (AtkEvent*)evt.AtkEvent;
        PluginLog.Information($"""
            Param: {evt.EventParam}
            AtkEventType: {evt.AtkEventType}
            AtkEvent.Type: {atkEvent->Type}
            Data: {MemoryHelper.ReadRaw(evt.Data, 0x40).ToHexString()}
            """);
    }



    public void SetComment(string s)
    {
        var san = Chat.Instance.SanitiseText(s);
        if(s != san) throw new ArgumentOutOfRangeException("String contains invalid characters!");
        if(s.Split("\n").Length > 2) throw new InvalidOperationException("String contains more than 2 lines");
        var bytes = Encoding.UTF8.GetBytes(s + "\0");
        if(bytes.Length > 192) throw new InvalidOperationException("String exceeds maximum length");
        var agent = (nint)AgentLookingForGroup.Instance();
        Marshal.Copy(bytes, 0, agent + 9680, bytes.Length);
    }
}
