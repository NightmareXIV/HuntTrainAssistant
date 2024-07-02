using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant.Services;
public class LifestreamIPC
{
		private LifestreamIPCInternal IPC = new();

		private LifestreamIPC() {}

		public void TPAndChangeWorld(string world, bool isDcTransfer = false, string secondaryTeleport = null, bool noSecondaryTeleport = false, int? gateway = null, bool? doNotify = null, bool? returnToGateway = null) => IPC.TPAndChangeWorld(world, isDcTransfer, secondaryTeleport, noSecondaryTeleport, gateway, doNotify, returnToGateway);

		public bool CanVisitSameDC(string world) => IPC.CanVisitSameDC(world);

		public bool CanVisitCrossDC(string world) => IPC.CanVisitCrossDC(world);

		public bool IsBusy() => IPC.IsBusy();

		public bool Teleport(uint aetheryte) => IPC.Teleport(aetheryte, 0);
}
