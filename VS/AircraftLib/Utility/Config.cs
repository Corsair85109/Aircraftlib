using System;
using Nautilus.Options.Attributes;
using Nautilus.Json;
using UnityEngine;

namespace AircraftLib.Utility
{
    [Menu("Aircraftlib Configuration")]
    public class Config : ConfigFile
    {
        [Keybind("Yaw Left")]
        public KeyCode yawLeftBind = KeyCode.A;

        [Keybind("Yaw Right")]
        public KeyCode yawRightBind = KeyCode.D;

        [Toggle("Invert pitch")]
        public bool invertPitch = false;

        [Toggle("Invert pitch when submerged")]
        public bool invertPitchSubmerged = false;

        [Slider("Control Tether Sensitivity", Max = 4f, Min = .1f, Step = 0.05f)]
        public float TetherSensitivity = 1f;

        [Keybind("Increase Thrust")]
        public KeyCode thrustIncreaseBind = KeyCode.W;

        [Keybind("Decrease Thrust")]
        public KeyCode thrustDecreaseBind = KeyCode.S;

        [Toggle("Use accurate water surface height")]
        public bool useAccurateWaterSurface = true;
    }
}
