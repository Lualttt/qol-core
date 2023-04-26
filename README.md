# Quality Of Life Core

Example
```cs
public override void Load()
{
    Harmony.CreateAndPatchAll(typeof(Plugin));

    Mods.RegisterMod(PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION, "Quality Of Life Core");

    Commands.RegisterCommand("help", "/help (index|command)", "Shows a list of commands.", HelpCommand);
    Commands.RegisterCommand("mods", "/mods (index|mod)", "Shows a list of qol mods.", ModsCommand);
    Commands.RegisterCommand("plugins", "/plugins (index|plugins)", "Show a list of qol plugins.", ModsCommand);

    Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
}
```

```cs
qol_core.Plugin.SendMessage("Hello, world!");
```

Mods
```cs
Mods.RegisterMod(PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION, "Description");
Mods.UnregisterMod(PluginInfo.PLUGIN_NAME);
Mods.ModExists("mod-name");
Mods.GetMod("mod-name");
```

Commands
```cs
Commands.RegisterCommand("command-name", "/command-name (optional|optional2) [required|required2]", "description of the command", Callback);
Commands.UnregisterCommand("command-name");
Commands.CommandExists("command-name");
Commands.GetCommand("command-name");
```