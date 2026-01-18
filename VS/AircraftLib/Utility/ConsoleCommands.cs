using Nautilus.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AircraftLib.Utility
{
    internal static class ConsoleCommands
    {
        [ConsoleCommand("smoothfoundations")]
        public static void SmoothFoundations()
        {
            ALLogger.Log("Smoothing all foundations placed in the world, this may take a while");

            GameObject[] objects = GameObject.FindGameObjectsWithTag("MainPieceGeometry");

            foreach (GameObject obj in objects)
            {
                if (obj.name != "BaseFoundationPiece(Clone)") continue;

                ALLogger.Log(obj.name);

                BoxCollider collider = obj.transform.Find("models/BaseFoundationPlatform").GetComponent<BoxCollider>();

                collider.size = new Vector3(10.1f, 10.1f, 0.2f);
            }
        }
    }
}
