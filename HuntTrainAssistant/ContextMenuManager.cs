using Dalamud.ContextMenu;
using Dalamud.Game.Text.SeStringHandling;
using ECommons.ChatMethods;
using ECommons.SimpleGui;

namespace HuntTrainAssistant;

internal class ContextMenuManager
{
    static readonly string[] ValidAddons = new string[]
{
        null,
        "PartyMemberList",
        "FriendList",
        "FreeCompany",
        "LinkShell",
        "CrossWorldLinkshell",
        "_PartyList",
        "ChatLog",
        "LookingForGroup",
        "BlackList",
        "ContentMemberList",
        "SocialList",
        "ContactList",
};

    GameObjectContextMenuItem openMessenger;
    DalamudContextMenu contextMenu;

    internal ContextMenuManager()
    {
        contextMenu = new();
        openMessenger = new GameObjectContextMenuItem(
            new SeStringBuilder().AddUiForeground("Set as conductor", 578).Build(), AssignConductor);
        contextMenu.OnOpenGameObjectContextMenu += OpenContextMenu;
    }

    private void OpenContextMenu(GameObjectContextMenuOpenArgs args)
    {
        //Svc.Chat.Print($"{args.ParentAddonName.NullSafe()}/{args.Text}/{args.ObjectWorld}");
        if ((Svc.ClientState.TerritoryType.EqualsAny(ValidZones) || P.config.Debug)
            && args.Text != null
            && ValidAddons.Contains(args.ParentAddonName) && args.ObjectWorld != 0 && args.ObjectWorld != 65535)
        {
            args.AddCustomItem(openMessenger);
        }
    }

    public void Dispose()
    {
        contextMenu.Dispose();
    }

    private void AssignConductor(GameObjectContextMenuItemSelectedArgs args)
    {
        var player = args.Text.ToString();
        var world = args.ObjectWorld;
        var s = new Sender(player, world);
        P.config.CurrentConductor = s;
        EzConfigGui.Open();
    }
}
