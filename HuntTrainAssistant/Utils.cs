using Dalamud.Game.ClientState.Objects.Types;
using ECommons;
using ECommons.ExcelServices;
using ECommons.ExcelServices.TerritoryEnumeration;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using HuntTrainAssistant.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant;
public static class Utils
{
		public static void DelayTeleport()
		{
				if(P.Config.TeleportDelayEnabled && P.Config.TeleportDelayMax > 0 && P.Config.TeleportDelayMax >= P.Config.TeleportDelayMin)
				{
						var num = P.Config.TeleportDelayMin + Random.Shared.Next(P.Config.TeleportDelayMax - P.Config.TeleportDelayMin);
						if(EzThrottler.GetRemainingTime("Teleport") < num)
						{
								EzThrottler.Throttle("Teleport", num, true);
						}
				}
		}

		public static bool CheckMultiMode()
		{
				if(S.AutoRetainerIPC.GetMultiModeStatus())
				{
						S.SonarMonitor.Continuation = null;
						P.TaskManager.Abort();
						P.Config.AutoVisitTeleportEnabled = false;
						P.TeleportTo = null;
						return true;
        }
				else
				{
						return false;
				}
		}

		public static bool IsNpcIdInARankList(uint npcId)
		{
				if(P.Config.Debug) return true;
				return Enum.GetValues<DawntrailARank>().Contains((DawntrailARank)npcId);
    }

		public static bool IsInHuntingTerritory()
		{
				if (ExcelTerritoryHelper.Get(Svc.ClientState.TerritoryType)?.TerritoryIntendedUse.RowId == (int)TerritoryIntendedUseEnum.Open_World) return true;
        if (Svc.ClientState.TerritoryType.EqualsAny((ushort[])[
            1024, //mare <-> garlemard gateway
						682, 739, 759, //doman enclave
						635, 659, //rhalgr's reach
            ])) return true; 
        if (Svc.ClientState.TerritoryType == MainCities.Idyllshire) return true;
				return false;
		}

		public static bool CanAutoInstanceSwitch()
		{
				if(P.KilledARanks.Count >= 2) return true;
				if(P.KilledARanks.Count == 1)
				{
						return Svc.Condition[ConditionFlag.InCombat] && Svc.Objects.OfType<IBattleNpc>().Any(x => Utils.IsNpcIdInARankList(x.NameId) && (float)x.CurrentHp / (float)x.MaxHp < 0.5f && !x.IsDead);
				}
				return false;
    }
}
