using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System;
using System.Collections.Generic;
using qol_core;

namespace example
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static Mod modInstance;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));

            modInstance = Mods.RegisterMod(PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION, "An example mod.");

            Commands.RegisterCommand("test", "test", "A test command.", modInstance, TestCommand);

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }


        public static bool TestCommand(List<string> arguments)
        {
            qol_core.Plugin.SendMessage("Hello, world! " + string.Join(", ", arguments), modInstance);
        }
    }
}