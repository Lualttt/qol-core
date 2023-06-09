using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using UnityEngine;

namespace qol_core
{
    public class Update
    {
        public static bool silenced = false;
        public static List<Tuple<Mod, string>> outdatedMods;

        public static bool UpdateCommand(List<string> arguments)
        {
            if (arguments.Count == 1)
            {
                Plugin.SendMessage("Wrong syntax use either; /update [silence|restart]", Plugin.modInstance);
                Plugin.SendMessage("or; /update install [mod|all]", Plugin.modInstance);

                return true;
            }

            if (arguments[1].ToLower() == "check")
                checkMods();
            if (arguments[1].ToLower() == "silence")
                toggleSilence();
            if (arguments[1].ToLower() == "install")
                updateMod(arguments[2]);
            if (arguments[1].ToLower() == "restart")
                restartGame();

            return true;
        }

        public static void toggleSilence()
        {
            silenced = !silenced;

            if (silenced)
            {
                Plugin.SendMessage("Update messages are temporarily silenced.", Plugin.modInstance);
            } else
            {
                Plugin.SendMessage("Update messages are  no longer silenced.", Plugin.modInstance);
            }
        }

        public static void updateMod(string modName)
        {
            if (modName.ToLower() == "all")
            {
                foreach(Tuple<Mod, string> mod in outdatedMods)
                {
                    using (var client = new WebClient())
                    {
                        DirectoryInfo directory = new DirectoryInfo("BepInEx/plugins");
                        FileInfo[] files = directory.GetFiles($"*{mod.Item1.Name}*");
                        foreach (FileInfo file in files)
                        {
                            file.Delete();
                        }
                        client.DownloadFileAsync(new Uri($"https://github.com/{mod.Item1.Github}/releases/download/{mod.Item2}/{mod.Item1.Name}.dll"), $"BepInEx/plugins/{mod.Item1.Name}-{mod.Item2}.dll");
                    }
                }
                Plugin.SendMessage("Done installing mods, run /update restart to apply changes.", Plugin.modInstance);
            } else if (outdatedMods.Any(m => m.Item1.Name == modName))
            {
                // genuine garbage code
                foreach(Tuple<Mod, string> mod in outdatedMods)
                {
                    if (mod.Item1.Name != modName)
                        continue;

                    using (var client = new WebClient())
                    {
                        DirectoryInfo directory = new DirectoryInfo("BepInEx/plugins");
                        FileInfo[] files = directory.GetFiles($"*{mod.Item1.Name}*");
                        foreach (FileInfo file in files)
                        {
                            file.Delete();
                        }
                        client.DownloadFileAsync(new Uri($"https://github.com/{mod.Item1.Github}/releases/download/{mod.Item2}/{mod.Item1.Name}.dll"), $"BepInEx/plugins/{mod.Item1.Name}-{mod.Item2}.dll");
                    }
                }
                Plugin.SendMessage($"Done installing {modName}, run /update restart to apply changes.", Plugin.modInstance);
            } else
            {
                Plugin.SendMessage($"The mod \"{modName}\" doesn't need an update, or doesn't support it.", Plugin.modInstance);
            }
        }

        public static void checkMods()
        {
            outdatedMods = new List<Tuple<Mod, string>>();

            if (silenced) return;

            List<Tuple<Mod, string>> needUpdates = new List<Tuple<Mod, string>>();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("qol-core", "1.0"));

            foreach(Mod mod in Mods.ModList.Values)
            {
                if (mod.Github == "") continue;

                client.BaseAddress = new Uri($"https://api.github.com/repos/{mod.Github}/tags");

                HttpResponseMessage response = client.GetAsync("").Result;
                if (response.IsSuccessStatusCode)
                {
                    string version = response.Content.ReadAsStringAsync().Result.Split("\"")[3];

                    if (version.IndexOf(".") == version.LastIndexOf("."))
                    {
                        Plugin.SendMessage("Something went wrong checking the version.", mod);
                        continue;
                    }

                    if (mod.Version != version)
                        needUpdates.Add(new Tuple<Mod, string>(mod, version));
                } else
                {
                    Plugin.SendMessage("Something went wrong checking the version.", mod);
                }

                System.Threading.Thread.Sleep(50);
            }

            if (needUpdates.Count == 0)
                return;

            Plugin.SendMessage("You have some unupdated mods.", Plugin.modInstance);
            foreach(Tuple<Mod, string> mod in needUpdates)
            {
                Plugin.SendMessage($"{mod.Item1.Version} -> {mod.Item2}", mod.Item1);
            }

            outdatedMods = needUpdates;
        }

        static void restartGame(){
            Process.Start("steam://run/1782210");
            Application.Quit();
        }
    }
}