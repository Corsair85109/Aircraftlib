using AircraftLib.Utility;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static VFXParticlesPool;

namespace AircraftLib.Patches
{
    [HarmonyPatch(typeof(BaseFoundationPiece), nameof(BaseFoundationPiece.Start))]
    public class FoundationStartPatch
    {
        public static void Postfix(BaseFoundationPiece __instance)
        {
            Collider[] colliders = __instance.GetComponentsInChildren<Collider>(true);
            foreach (Collider collider in colliders)
            {
                if (collider.name == "BaseFoundationPlatform")
                {
                    ALLogger.Log("Modifying collider");
                    (collider as BoxCollider).size = new Vector3(10f, 10f, 0.2f);
                }
            }            
        }
    }
}
