using Dalamud.Configuration;
using ECommons.ChatMethods;
using ECommons.Configuration;
using HuntTrainAssistant.DataStructures;

namespace HuntTrainAssistant;

public class Config : IEzConfig
{
    public bool Enabled = true;
    public bool AutoTeleport = true;
    public float AutoTeleportAetheryteDistanceDiff = 3f;
    public bool SuppressChatOtherPlayers = true;
    public List<Sender> Conductors = [];
    public bool Debug = false;
    public bool AutoOpenMap = true;
    public bool DistanceCompensationHack = false;
    public bool SonarIntegration = false;
    public bool HuntAlertsIntegration = false;
    public bool AutoVisitTeleportEnabled = false;
    public bool AutoVisitCrossWorld = false;
    public bool AutoVisitCrossDC = false;
    public bool AutoVisitModifyChat = true;
    public Dictionary<Rank, List<Expansion>> AutoVisitExpansionsBlacklist = [];
    public List<uint> AetheryteBlacklist = [148];
    public bool EnableSonarInstanceSwitching = false;
    public bool AutoSwitchInstanceTwoRanks = false;
    public bool AutoSwitchInstanceToOne = false;
    public bool NoDuplicateFlags = true;
    public bool ContextMenu = true;
    public bool AudioAlert = false;
    public float AudioAlertVolume = 0.5f;
    public bool AudioAlertOnlyMinimized = false;
    public string AudioAlertPath = "";
    public int AudioThrottle = 500;
    public bool FlashTaskbar = false;
    public bool TrayNotification = true;
    public bool ExecuteMacroOnFlag = false;
    public int MacroIndex = 0;
}
