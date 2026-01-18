using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using UnityEngine;
using AircraftLib.Utility;
using AircraftLib.Managers;
using AircraftLib.Prefabs;
using UWE;

namespace AircraftLib
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    [BepInDependency("com.mikjaw.subnautica.vehicleframework.mod")]
    [BepInDependency("com.snmodding.nautilus")]
    public class AircraftLibPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.Bobasaur.AircraftLib";
        private const string PluginName = "AircraftLib";
        private const string VersionString = "1.6.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        public static Config ModConfig { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        private void Awake()
        {
            Logger.LogInfo($"Will load {PluginName} version {VersionString}.");


            Harmony.PatchAll();
            Log = Logger;

            RCCrosshair.LoadAssets();

            ConsoleCommandsHandler.RegisterConsoleCommands(typeof(ConsoleCommands));



            Logger.LogInfo($"{PluginName} version {VersionString} is loaded.");
        }

        // keybinds
        public static GameInput.Button YawLeftKey = EnumHandler.AddEntry<GameInput.Button>("YawLeftKey").CreateInput("Yaw Left").SetBindable().WithKeyboardBinding(GameInputHandler.Paths.Keyboard.A).WithCategory("Aircraftlib Keybinds");
        public static GameInput.Button YawRightKey = EnumHandler.AddEntry<GameInput.Button>("YawRightKey").CreateInput("Yaw Right").SetBindable().WithKeyboardBinding(GameInputHandler.Paths.Keyboard.D).WithCategory("Aircraftlib Keybinds");
        public static GameInput.Button IncreaseThrustKey = EnumHandler.AddEntry<GameInput.Button>("IncreaseThrustKey").CreateInput("Increase Thrust").SetBindable().WithKeyboardBinding(GameInputHandler.Paths.Keyboard.W).WithCategory("Aircraftlib Keybinds");
        public static GameInput.Button DecreaseThrustKey = EnumHandler.AddEntry<GameInput.Button>("DecreaseThrustKey").CreateInput("Decrease Thrust").SetBindable().WithKeyboardBinding(GameInputHandler.Paths.Keyboard.S).WithCategory("Aircraftlib Keybinds");
        public static GameInput.Button BrakeKey = EnumHandler.AddEntry<GameInput.Button>("BrakeKey").CreateInput("Brakes").SetBindable().WithKeyboardBinding(GameInputHandler.Paths.Keyboard.Space).WithCategory("Aircraftlib Keybinds");
    }
}
