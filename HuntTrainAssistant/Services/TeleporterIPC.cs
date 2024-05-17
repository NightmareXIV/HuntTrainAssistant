using ECommons.EzIpcManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.Services;
public class TeleporterIPC
{
		[EzIPC(applyPrefix:false)] public Func<uint, byte, bool> Teleport;

		private TeleporterIPC()
		{
				EzIPC.Init(this, "Teleport", SafeWrapper.AnyException);
		}
}
