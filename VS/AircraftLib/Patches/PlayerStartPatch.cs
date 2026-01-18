using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AircraftLib.Prefabs;
using UWE;

namespace AircraftLib.Patches
{
    [HarmonyPatch(typeof(Player))]
    public class PlayerStartPatch
    {
        [HarmonyPatch(nameof(Player.Start))]
        [HarmonyPostfix]
        public static void StartPostfix()
        {
            CoroutineHost.StartCoroutine(SmoothPlatform.ModifyFoundation());
        }
    }
}
