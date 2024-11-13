using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Game.Text.SeStringHandling;
using ECommons.ChatMethods;
using ECommons.ExcelServices;
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
		private MenuItem MenuItemAddConductor;

		private ContextMenuManager()
		{

				MenuItemAddConductor = new MenuItem()
				{
						Name = new SeStringBuilder().AddUiForeground("Add as conductor", 578).Build(),
						Prefix = Dalamud.Game.Text.SeIconChar.BoxedLetterH,
						PrefixColor = 578,
						OnClicked = AssignConductor,
				};
				Svc.ContextMenu.OnMenuOpened += OpenContextMenu;
		}

		private void OpenContextMenu(IMenuOpenedArgs args)
		{
				if(!P.Config.ContextMenu) return;
				if ((Utils.IsInHuntingTerritory() || P.Config.Debug)
						&& args.Target is MenuTargetDefault mt 
						&& mt.TargetName != null
						&& ValidAddons.Contains(args.AddonName) 
						&& mt.TargetHomeWorld.IsValid
						&& ExcelWorldHelper.GetPublicWorlds().Any(x => x.RowId == mt.TargetHomeWorld.RowId)
						)
				{
						args.AddMenuItem(MenuItemAddConductor);
				}
		}

		public void Dispose()
    {
        Svc.ContextMenu.OnMenuOpened -= OpenContextMenu;
    }

		private void AssignConductor(IMenuItemClickedArgs args)
		{
				if (args.Target is MenuTargetDefault mt)
				{
						var player = mt.TargetName.ToString();
						var world = mt.TargetHomeWorld;
						var s = new Sender(player, world);
						P.Config.Conductors.Add(s);
						EzConfigGui.Open();
				}
		}
}
