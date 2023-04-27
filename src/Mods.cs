using System.Collections.Generic;

namespace qol_core
{
    public class Mod
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }

        public Mod(string name, string version, string description)
        {
            Name = name;
            Version = version;
            Description = description;
        }
    }

    public class Mods
    {
        public static Dictionary<string, Mod> ModList = new Dictionary<string, Mod>();

        public static Mod RegisterMod(string name, string version, string description)
        {
            Mod mod = new Mod(name, version, description);
            ModList.Add(name, mod);
            return mod;
        }

        public static void UnregisterMod(string name)
        {
            ModList.Remove(name);
        }

        public static bool  ModExists(string name)
        {
            return ModList.ContainsKey(name);
        }

        public static Mod GetMod(string name)
        {
            return ModList[name];
        }
    }
}