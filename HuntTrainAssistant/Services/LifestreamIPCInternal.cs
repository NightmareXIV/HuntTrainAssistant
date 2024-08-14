using ECommons.EzIpcManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.Services;
public class LifestreamIPCInternal
{
		[EzIPC] public Func<string, bool> CanVisitSameDC;
		[EzIPC] public Func<string, bool> CanVisitCrossDC;
		[EzIPC] public Action<string, bool, string, bool, int?, bool?, bool?> TPAndChangeWorld;
		[EzIPC] public Func<bool> IsBusy;
		[EzIPC] public Func<uint, byte, bool> Teleport;


    public LifestreamIPCInternal()
		{
				EzIPC.Init(this, "Lifestream", SafeWrapper.AnyException);
		}
}
