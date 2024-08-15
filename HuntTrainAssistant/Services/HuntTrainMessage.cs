using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.Services;
public record struct HuntTrainMessage
{
    //(string Message, string huntKind, string huntWorld, string currentworldName, string currentregionName, string huntregionName, string Posted_Time,string startLocation, string startZone,string locationCoords,bool openmaponArrival ,bool teleporterEnabled,bool lifestreamEnabled)
    public string Message;
    public string huntType;
    public string huntKind;
    public string huntWorld;
    public string currentworldName;
    public string currentregionName;
    public string huntregionName;
    public string Posted_Time;
    public string startLocation;
    public string startZone;
    public string locationCoords;
    public bool openmaponArrival;
    public bool teleporterEnabled;
    public bool lifestreamEnabled;
    public int instance;
}
