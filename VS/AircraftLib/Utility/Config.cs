using System;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using UnityEngine;

namespace AircraftLib.Utility
{
    [Menu("Aircraftlib Configuration")]
    public class Config : ConfigFile
    {
        [Toggle("Invert pitch")]
        public bool invertPitch = false;

        [Toggle("Invert pitch when submerged")]
        public bool invertPitchSubmerged = false;

        [Slider("Control Crosshair Sensitivity", Max = 4f, Min = .1f, Step = 0.05f)]
        public float CrosshairSensitivity = 1f;

        [Toggle("More realistic waves (can cause motion sickness)")]
        public bool useAccurateWaterSurface = true;
    }
}
