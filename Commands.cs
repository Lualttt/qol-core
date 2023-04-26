using System;
using System.Collections.Generic;

namespace qol_core
{
    public class Command
    {
        public string Name { get; }
        public string Syntax { get; }
        public string Description { get; }
        public Func<List<string>, bool> Callback { get; }

        public Command(string name, string syntax, string description, Func<List<string>, bool> callback)
        {
            Name = name;
            Syntax = syntax;
            Description = description;
            Callback = callback;
        }
    }

    public class Commands
    {
        public static Dictionary<string, Command> CommandsList = new Dictionary<string, Command>();

        public static void RegisterCommand(string name, string syntax, string description, Func<List<string>, bool> callback)
        {
            CommandsList.Add(name, new Command(name, syntax, description, callback));
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
    }
}