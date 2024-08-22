using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.Services;
public class Notificator : NotificationMasterAPI.NotificationMasterApi
{
    private Notificator() : base(Svc.PluginInterface)
    {
    }
}
