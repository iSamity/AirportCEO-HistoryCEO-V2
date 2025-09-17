using AirportCEOHistoryCEO.History;
using AirportCEOModLoader.WatermarkUtils;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using ShortcutCeo.config;
using UnityEngine;

namespace AirportCEOHistoryCEO;

[BepInPlugin("org.iSamity.plugins." + MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
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
            "General",
            "Undo action",
            new KeyboardShortcut(KeyCode.Z, KeyCode.LeftControl)
        );

        ConfigManager.AddShortcut(undoAction, () => HistoryManager.Undo());

#if DEBUG
        var redoAction = Config.Bind(
            "General",
            "Redo shortcut",
            new KeyboardShortcut(KeyCode.Y, KeyCode.LeftControl)
        );


        ConfigManager.AddShortcut(redoAction, () => HistoryManager.Redo());

        var debugShortcut = Config.Bind(
             "Debug Action",
             "debugShortcut",
             new KeyboardShortcut(KeyCode.KeypadEnter),
             "Shortcut for debug action"
             );
        ConfigManager.AddShortcut(debugShortcut, () => HistoryManager.Log());
#endif

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

#if DEBUG
        WatermarkUtils.Register(new WatermarkInfo(MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION, false));
#else
        WatermarkUtils.Register(new WatermarkInfo("HyC", MyPluginInfo.PLUGIN_VERSION, true));
#endif
    }
}
