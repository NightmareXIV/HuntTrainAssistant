using Dalamud.Configuration;
using ECommons.ChatMethods;

namespace HuntTrainAssistant;

internal class Config : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public bool Enabled = true;
    public bool AutoTeleport = true;
    public float AutoTeleportAetheryteDistanceDiff = 3f;
    public bool SuppressChatOtherPlayers = true;
    public List<Sender> Conductors = new();
    public Sender CurrentConductor = new("", 0);
    public bool Debug = false;
    public bool AutoOpenMap = true;
}
