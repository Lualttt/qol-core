using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace qol_core
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static Plugin instance;
        public static Mod modInstance;

        ConfigEntry<string> commandPrefix;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));
            Harmony.CreateAndPatchAll(typeof(Update));

            instance = this;

            commandPrefix = Config.Bind<string>(
                "Commands",
                "commandPrefix",
                "/",
                "The prefix used for commands example \"/help\""
                );

            modInstance = Mods.RegisterMod(PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION, "Quality Of Life Core", "LualtOfficial/qol-core");

            Commands.RegisterCommand("help", "help (index|command)", "Shows a list of commands.", modInstance, HelpCommand);
            Commands.RegisterCommand("mods", "mods (index|mod)", "Shows a list of qol mods.", modInstance, ModsCommand);
            Commands.RegisterCommand("plugins", "plugins (index|plugins)", "Show a list of qol plugins.", modInstance, ModsCommand);
            Commands.RegisterCommand("prefix", "prefix (prefix)", "Change the prefix of all commands", modInstance, PrefixCommand);
            Commands.RegisterCommand("update", "update [install|silence|restart] [install -> mod|all]", "Update your qol mods.", modInstance, Update.UpdateCommand);

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(ChatBox), nameof(ChatBox.SendMessage))]
        [HarmonyPrefix]
        public static bool ChatMessage(ChatBox __instance, string __0)
        {
            if (!__0.StartsWith(instance.commandPrefix.Value)) return true;

            List<string> arguments = __0.Substring(1).Split(" ").ToList();

            if (!arguments[0].Contains(":"))
            {
                arguments[0] = Commands.FindName(arguments[0]);
            }

            if (!Commands.CommandExists(arguments[0].ToLower())) {
                SendMessage($"command \"{arguments[0]}\" doesn't exist", modInstance);

                return false;
            }

            Command command = Commands.GetCommand(arguments[0].ToLower());
            bool succesfull = command.Callback(arguments);

            if (!succesfull)
                SendMessage($"something went wrong running \"{arguments[0]}\"", modInstance);

            return false;
        }

        public static void SendMessage(string message, Mod mod)
        {
            ChatBox.Instance.AppendMessage(SteamManager.Instance.field_Private_CSteamID_0.m_SteamID, message, mod.Name);
        }

        public static bool HelpCommand(List<string> arguments)
        {
            int listIndex = 1;
            int maxIndex = (((Commands.CommandsList.Count / 5) + 1) * 5) / 5;

            if (arguments.Count > 1) {
                try
                {
                    listIndex = Math.Clamp(Int32.Parse(arguments[1]), 1, maxIndex);
                } catch (FormatException)
                {
                    if (!arguments[1].Contains(":"))
                    {
                        arguments[1] = Commands.FindName(arguments[1]);
                    }

                    if (!Commands.CommandExists(arguments[1]))
                    {
                        SendMessage($"argument \"{arguments[1]}\" needs to be a number or command", modInstance);
                        return true;
                    }

                    SendMessage($"-=+# Help ({arguments[1]}) #+=-", modInstance);
                    Command command = Commands.GetCommand(arguments[1]);
                    SendMessage(instance.commandPrefix.Value + command.Syntax, modInstance);
                    SendMessage(command.Description, modInstance);

                    return true;
                }
            }

            SendMessage($"-=+# Help ({listIndex}/{maxIndex}) #+=-", modInstance);

            for (int i = (listIndex * 4)-4; i < listIndex * 4; i++)
            {
                Command command = Commands.CommandsList.Values.ToList()[i];
                SendMessage($"{command.Name}: {command.Description}", command.ModInstance);
            }

            return true;
        }

        public static bool ModsCommand(List<string> arguments)
        {
            int listIndex = 1;
            int maxIndex = (((Mods.ModList.Count / 5) + 1) * 5) / 5;

            if (arguments.Count > 1) {
                try
                {
                    listIndex = Math.Clamp(Int32.Parse(arguments[1]), 1, maxIndex);
                } catch (FormatException)
                {
                    if (!Mods.ModExists(arguments[1]))
                    {
                        SendMessage($"argument \"{arguments[1]}\" needs to be a number or command", modInstance);
                        return true;
                    }

                    SendMessage($"-=+# Mods ({arguments[1]}) #+=-", modInstance);
                    Mod mod = Mods.GetMod(arguments[1]);
                    SendMessage($"{mod.Name} ({mod.Version})", modInstance);
                    SendMessage(mod.Description, modInstance);

                    return true;
                }
            }

            SendMessage($"-=+# Mods ({listIndex}/{maxIndex}) #+=-", modInstance);

            for (int i = (listIndex * 4)-4; i < listIndex * 4; i++)
            {
                Mod mod = Mods.ModList.Values.ToList()[i];
                SendMessage($"{mod.Name} ({mod.Version})", modInstance);
            }

            return true;
        }

        public static bool PrefixCommand(List<string> arguments)
        {
            if (arguments.Count == 1)
            {
                SendMessage($"current prefix \"{instance.commandPrefix.Value}\"", modInstance);
            } else
            {
                instance.commandPrefix.Value = arguments[1];
                instance.Config.Save();
                SendMessage($"current prefix \"{instance.commandPrefix.Value}\"", modInstance);
            }

            return true;
        }
    }
}