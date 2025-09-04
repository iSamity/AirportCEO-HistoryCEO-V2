using AirportCEOHistoryCEO.History;
using AirportCEOModLoader.WatermarkUtils;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using ShortcutCeo.config;
using UnityEngine;

namespace AirportCEOHistoryCEO;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("org.airportceomodloader.humoresque")]
[BepInDependency("org.iSamity.plugins.ShortcutCeo")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} - Is starting up.");

        SetupConfig();

        SetupHarmony();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} - Is loaded!");
    }

    private void Start()
    {
        SetupModLoader();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} - Finished start");
    }


    private void SetupConfig()
    {
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} - Setting up config.");

        var undoAction = Config.Bind(
            "Undo Action",
            "UndoAction",
            new KeyboardShortcut(KeyCode.Z, KeyCode.LeftControl),
            "Shortcut for undo action"
        );

        var redoAction = Config.Bind(
            "Redo Action",
            "RedoAction",
            new KeyboardShortcut(KeyCode.Y, KeyCode.LeftControl),
            "Shortcut for redo action"
        );


        ConfigManager.AddShortcut(undoAction, () => HistoryManager.Undo());
        // Disable redo for now as it doesn't work as expected
        //ConfigManager.AddShortcut(redoAction, () => HistoryManager.Redo());

        var debugShortcut = Config.Bind(
            "Debug Action",
            "debugShortcut",
            new KeyboardShortcut(KeyCode.KeypadEnter),
            "Shortcut for debug action"
        );
        ConfigManager.AddShortcut(debugShortcut, () => HistoryManager.Log());

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} - Finished up config.");
    }

    private void SetupHarmony()
    {
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} - Setting up Harmony.");

        var harmony = new HarmonyLib.Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
        
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} - Finished up Harmony.");
    }

    private void SetupModLoader()
    {
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} - Setting up Mod Loader.");
        WatermarkUtils.Register(new WatermarkInfo(MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION, false));
    }
}
