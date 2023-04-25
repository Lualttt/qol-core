using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System;
using System.Linq;
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

    public class RegisterCommands
    {
        public static Dictionary<string, Command> Commands = new Dictionary<string, Command>();

        public static void RegisterCommand(string name, string syntax, string description, Func<List<string>, bool> callback)
        {
            Commands.Add(name, new Command(name, syntax, description, callback));
        }

        public static void Unregister(string name)
        {
            Commands.Remove(name);
        }

        public static bool CommandExists(string name)
        {
            return Commands.ContainsKey(name);
        }

        public static Command GetCommand(string name)
        {
            return Commands[name];
        }
    }
}