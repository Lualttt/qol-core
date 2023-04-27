using System;
using System.Collections.Generic;
using System.Linq;

namespace qol_core
{
    public class Command
    {
        public string Name { get; }
        public string Syntax { get; }
        public string Description { get; }
        public Mod ModInstance { get; }
        public Func<List<string>, bool> Callback { get; }

        public Command(string name, string syntax, string description, Mod modInstance, Func<List<string>, bool> callback)
        {
            Name = name;
            Syntax = syntax;
            Description = description;
            ModInstance = modInstance;
            Callback = callback;
        }
    }

    public class Commands
    {
        public static Dictionary<string, Command> CommandsList = new Dictionary<string, Command>();

        public static void RegisterCommand(string name, string syntax, string description, Mod mod, Func<List<string>, bool> callback)
        {
            CommandsList.Add($"{mod.Name}:{name}".ToLower(), new Command(name, syntax, description, mod, callback));
        }

        public static void UnregisterMod(string name)
        {
            CommandsList.Remove(name);
        }

        public static bool CommandExists(string name)
        {
            return CommandsList.ContainsKey(name);
        }

        public static Command GetCommand(string name)
        {
            return CommandsList[name];
        }

        public static string FindName(string name)
        {
            foreach(string key in CommandsList.Keys)
            {
                string modName = key.Split(":").Last();
                if (modName == name)
                {
                    return key;
                }
            }
            return "";
        }
    }
}