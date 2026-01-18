using AircraftLib.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AircraftLib.Managers
{
    public class RCCrosshair
    {
        public static GameObject rcCrosshairCanvas;

        public static void LoadAssets()
        {
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(directory, "assets/aircraftlib"));
            if (assetBundle == null)
            {
                ALLogger.Error("Failure loading RCCrosshair assetbundle");
            }
            else
            {
                rcCrosshairCanvas = assetBundle.LoadAsset<GameObject>("TetherCanvas");

                ALLogger.Log("RCCrosshair assets loaded");
            }
        }
    }
}
