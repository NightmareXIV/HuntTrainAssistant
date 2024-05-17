using Dalamud.ContextMenu;
using Dalamud.Game.Text.SeStringHandling;
using ECommons.ChatMethods;
using ECommons.SimpleGui;

namespace HuntTrainAssistant;

public class ContextMenuManager : IDisposable
{
		private static readonly string[] ValidAddons = new string[]
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
		private GameObjectContextMenuItem MenuItemAddConductor;
		private DalamudContextMenu ContextMenu;

		private ContextMenuManager()
		{
				ContextMenu = new(Svc.PluginInterface);
				MenuItemAddConductor = new GameObjectContextMenuItem(
						new SeStringBuilder().AddUiForeground("Add as conductor", 578).Build(), AssignConductor);
				ContextMenu.OnOpenGameObjectContextMenu += OpenContextMenu;
		}

		private void OpenContextMenu(GameObjectContextMenuOpenArgs args)
		{
				//Svc.Chat.Print($"{args.ParentAddonName.NullSafe()}/{args.Text}/{args.ObjectWorld}");
				if ((Utils.IsInHuntingTerritory() || P.Config.Debug)
						&& args.Text != null
						&& ValidAddons.Contains(args.ParentAddonName) && args.ObjectWorld != 0 && args.ObjectWorld != 65535)
				{
						args.AddCustomItem(MenuItemAddConductor);
				}
		}

		public void Dispose()
		{
				ContextMenu.Dispose();
		}

		private void AssignConductor(GameObjectContextMenuItemSelectedArgs args)
		{
				var player = args.Text.ToString();
				var world = args.ObjectWorld;
				var s = new Sender(player, world);
				P.Config.Conductors.Add(s);
				EzConfigGui.Open();
		}
}
