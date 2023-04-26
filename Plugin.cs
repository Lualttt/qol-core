using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace qol_core
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));

            Mods.RegisterMod(PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION, "Quality Of Life Core");

            Commands.RegisterCommand("help", "/help (index|command)", "Shows a list of commands.", HelpCommand);
            Commands.RegisterCommand("mods", "/mods (index|mod)", "Shows a list of qol mods.", ModsCommand);
            Commands.RegisterCommand("plugins", "/plugins (index|plugins)", "Show a list of qol plugins.", ModsCommand);

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(ChatBox), nameof(ChatBox.SendMessage))]
        [HarmonyPrefix]
        public static bool ChatMessage(ChatBox __instance, string __0)
        {
            if (__0.Substring(0, 1) != "/") return true;

            List<string> arguments = __0.Substring(1).Split(" ").ToList();

            if (!Commands.CommandExists(arguments[0])) {
                SendMessage($"command \"{arguments[0]}\" doesn't exist");

                return false;
            }

            Command command = Commands.GetCommand(arguments[0]);
            bool succesfull = command.Callback(arguments);

            if (!succesfull)
                SendMessage($"something went wrong running \"{arguments[0]}\"");

            return false;
        }

        public static void SendMessage(string message)
        {
            ChatBox.Instance.AppendMessage(SteamManager.Instance.field_Private_CSteamID_0.m_SteamID, message, SteamManager.Instance.field_Private_String_0);
        }

        public static bool HelpCommand(List<string> arguments)
        {
            int listIndex = 1;
            int maxIndex = (((Commands.CommandsList.Count / 4) + 1) * 4) / 4;

            if (arguments.Count > 1) {
                try
                {
                    listIndex = Math.Clamp(Int32.Parse(arguments[1]), 1, maxIndex);
                } catch (FormatException)
                {
                    if (!Commands.CommandExists(arguments[1]))
                    {
                        SendMessage($"argument \"{arguments[1]}\" needs to be a number or command");
                        return true;
                    }

                    SendMessage($"-=+# Help ({arguments[1]}) #+=-");
                    Command command = Commands.GetCommand(arguments[1]);
                    SendMessage(command.Syntax);
                    SendMessage(command.Description);

                    return true;
                }
            }

            SendMessage($"-=+# Help ({listIndex}/{maxIndex}) #+=-");

            for (int i = (listIndex * 4)-4; i < listIndex * 4; i++)
            {
                Command command = Commands.CommandsList.Values.ToList()[i];
                SendMessage($"{command.Name}: {command.Description}");
            }

            return true;
        }

        public static bool ModsCommand(List<string> arguments)
        {
            int listIndex = 1;
            int maxIndex = (((Mods.ModList.Count / 4) + 1) * 4) / 4;

            if (arguments.Count > 1) {
                try
                {
                    listIndex = Math.Clamp(Int32.Parse(arguments[1]), 1, maxIndex);
                } catch (FormatException)
                {
                    if (!Mods.ModExists(arguments[1]))
                    {
                        SendMessage($"argument \"{arguments[1]}\" needs to be a number or command");
                        return true;
                    }

                    SendMessage($"-=+# Mods ({arguments[1]}) #+=-");
                    Mod mod = Mods.GetMod(arguments[1]);
                    SendMessage($"{mod.Name} ({mod.Version})");
                    SendMessage(mod.Description);

                    return true;
                }
            }

            SendMessage($"-=+# Mods ({listIndex}/{maxIndex}) #+=-");

            for (int i = (listIndex * 4)-4; i < listIndex * 4; i++)
            {
                Mod mod = Mods.ModList.Values.ToList()[i];
                SendMessage($"{mod.Name} ({mod.Version})");
            }

            return true;
        }
    }
}