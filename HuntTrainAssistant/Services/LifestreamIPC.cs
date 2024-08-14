using ECommons.EzIpcManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.Services;
public class LifestreamIPC
{
    [EzIPC] public Func<bool> CanChangeInstance;
    [EzIPC] public Func<int> GetNumberOfInstances;
    [EzIPC] public Action<int> ChangeInstance;
		[EzIPC] public Func<int> GetCurrentInstance;
    private LifestreamIPCInternal IPC = new();

		private LifestreamIPC() 
		{
        EzIPC.Init(this, "Lifestream", SafeWrapper.AnyException);
    }

		public void TPAndChangeWorld(string world, bool isDcTransfer = false, string secondaryTeleport = null, bool noSecondaryTeleport = false, int? gateway = null, bool? doNotify = null, bool? returnToGateway = null) => IPC.TPAndChangeWorld(world, isDcTransfer, secondaryTeleport, noSecondaryTeleport, gateway, doNotify, returnToGateway);

		public bool CanVisitSameDC(string world) => IPC.CanVisitSameDC(world);

		public bool CanVisitCrossDC(string world) => IPC.CanVisitCrossDC(world);

		public bool IsBusy() => IPC.IsBusy();

		public bool Teleport(uint aetheryte) => IPC.Teleport(aetheryte, 0);
}
