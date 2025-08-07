using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Memory;
using ECommons.Automation;
using ECommons.Automation.UIInput;
using ECommons.EzHookManager;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using AtkEvent = FFXIVClientStructs.FFXIV.Component.GUI.AtkEvent;

namespace HuntTrainAssistant.Services;
public unsafe class LFGService : IDisposable
{
    private LFGService()
    {
        Svc.AddonLifecycle.RegisterListener(AddonEvent.PostReceiveEvent, "LookingForGroupCondition", OnReceiveEvent);
        EzSignatureHelper.Initialize(this);
    }

    public void Dispose()
    {
        Svc.AddonLifecycle.RegisterListener(AddonEvent.PostReceiveEvent, "LookingForGroupCondition", OnReceiveEvent);
    }

    private void OnReceiveEvent(AddonEvent type, AddonArgs args)
    {
        var evt = (AddonReceiveEventArgs)args;
        var atkEvent = (AtkEvent*)evt.AtkEvent;
        PluginLog.Information($"""
            Param: {evt.EventParam}
            AtkEventType: {evt.AtkEventType}
            AtkEvent.Type: {atkEvent->State.EventType}
            Data: {(evt.Data != nint.Zero ? MemoryHelper.ReadRaw(evt.Data, 0x40).ToHexString() : "null")}
            """);
    }



    public void SetComment(string s)
    {
        var san = Chat.Instance.SanitiseText(s);
        if(s != san) throw new ArgumentOutOfRangeException("String contains invalid characters!");
        if(s.Split("\n").Length > 2) throw new InvalidOperationException("String contains more than 2 lines");
        var bytes = Encoding.UTF8.GetBytes(s + "\0");
        if(bytes.Length > 192) throw new InvalidOperationException("String exceeds maximum length");
        AgentLookingForGroup.Instance()->StoredRecruitmentInfo.CommentString = s + "\0";
    }
}
