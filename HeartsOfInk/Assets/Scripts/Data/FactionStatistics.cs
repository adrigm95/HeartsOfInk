using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    public class FactionStatistics
    {
        public Player Player { get; private set; }
        public int OwnUnitsLost { get; private set; }
        public int EnemyUnitsDistroyed { get; private set; }
        public int MaxUnits { get; private set; }
        public int OwnArmysLost { get; private set; }
        public int EnemyArmysDistroyed { get; private set; }
        public int MaxArmys { get; private set; }
        public int OwnCitiesLost { get; private set; }
        public int EnemyCitiesConquered { get; private set; }
        public int MaxCities { get; private set; }
        public int CitiesAtEnd { get; private set; }

        public FactionStatistics(Player player)
        {
            Player = player;
            OwnUnitsLost = 0;
            EnemyUnitsDistroyed = 0;
            MaxUnits = 0;
            OwnArmysLost = 0;
            EnemyArmysDistroyed = 0;
            MaxArmys = 0;
            OwnCitiesLost = 0;
            EnemyCitiesConquered = 0;
            MaxCities = 0;
            CitiesAtEnd = 0;
        }

        public void CombatReport(int unitsLost, int unitsDistroyed)
        {
            OwnUnitsLost += unitsLost;
            EnemyUnitsDistroyed += unitsDistroyed;
        }

        public void ReportArmyDefeated(Player armyDefeated)
        {
            if (Player == armyDefeated)
            {
                OwnArmysLost++;
            }
            else
            {
                EnemyArmysDistroyed++;
            }
        } 

        public void ReportCityConquered(Player conqueror, Player conquered)
        {
            if (Player == conquered)
            {
                OwnCitiesLost++;
            }
            else if (Player == conqueror)
            {
                EnemyCitiesConquered++;
            }
        }

        /// <summary>
        /// Actualiza las estadisticas en base al estado actual total de la facción.
        /// </summary>
        /// <param name="currentUnits"> Suma de las unidades de todos los ejercitos.</param>
        /// <param name="currentArmys"> Total de ejercitos. </param>
        /// <param name="currentCities"> Total de ciudades poseidas actualmente. </param>
        public void ReportFactionState(int currentUnits, int currentArmys, int currentCities)
        {
            if (currentUnits > MaxUnits)
            {
                MaxUnits = currentUnits;
            }

            if (currentArmys > MaxArmys)
            {
                MaxArmys = currentArmys;
            }
            
            if (currentCities > MaxCities)
            {
                MaxCities = currentCities;
            }
        }

        /// <summary>
        /// Setea la cantidad de ciudades que la facción posee al terminar la partida, se usa para determinar si ha sido derrotado o no.
        /// </summary>
        /// <param name="cities"></param>
        public void SetCitiesAtEnd(List<CityController> cities)
        {
            CitiesAtEnd = 0;

            foreach (CityController city in cities)
            {
                if (city.Owner == Player)
                {
                    CitiesAtEnd++;
                }
            }
        }
    }
}
