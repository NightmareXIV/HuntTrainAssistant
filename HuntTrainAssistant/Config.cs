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
    public bool AutoVisitTeleportEnabled = false;
    public bool AutoVisitCrossWorld = false;
    public bool AutoVisitCrossDC = false;
    public bool AutoVisitModifyChat = true;
    public Dictionary<Rank, List<Expansion>> AutoVisitExpansions = [];
    public List<uint> AetheryteBlacklist = [148];
}
