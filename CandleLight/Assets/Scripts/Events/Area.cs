using General;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Events {

    public class Area {

        public SubArea[] subAreas = new SubArea[10];    // max sub area number is 10
        public string areaName;
        public int subAreasNum = 0;

        public Area(string areaName, string[] subAreaNames, IDbConnection dbConnection) {
            Debug.Log("Area " + areaName);
            this.areaName = areaName;

            for (int i = 0; i < subAreas.Length; i++) {
                string subAreaName = subAreaNames[i];
                if (subAreaName != "none") {
                    Debug.Log(subAreaName);
                    subAreas[i] = GameManager.instance.DB.GetSubAreaByAreaName(subAreaName, dbConnection);
                    subAreasNum++;
                }
            }

            
        }

        /// <summary>
        /// subAreas[0] will always be the start area
        /// subAreas[subAreaNum - 1] will be the exit
        /// subAreas[subAreaNum - 2] will be the boss
        /// </summary>
        /// <returns></returns>
        public SubArea GetSubArea(int index) {
            return subAreas[index];
        }
    }
}
