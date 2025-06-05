using System.Collections.Generic;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace MoloneyCustomCardInjector;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    
    private Harmony _harmony;
    private void Awake()
    {
        _harmony = new Harmony(MyPluginInfo.PLUGIN_NAME);
        _harmony.PatchAll(typeof(Patches));
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        
    }
    
}