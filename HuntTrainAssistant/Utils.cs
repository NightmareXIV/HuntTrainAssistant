using ECommons.ExcelServices;
using ECommons.ExcelServices.TerritoryEnumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntTrainAssistant;
public static class Utils
{
		public static bool IsInHuntingTerritory()
		{
				if (ExcelTerritoryHelper.Get(Svc.ClientState.TerritoryType).TerritoryIntendedUse == (int)TerritoryIntendedUseEnum.Open_World) return true;
				if (Svc.ClientState.TerritoryType == 1024) return true;
				if (Svc.ClientState.TerritoryType == MainCities.Idyllshire) return true;
				return false;
		}
}
