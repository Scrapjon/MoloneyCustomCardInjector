using System;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using Network;
using Newtonsoft.Json;
using Logger = BepInEx.Logging.Logger;

namespace MoloneyCustomCardInjector;

public class Patches
{
    [HarmonyPatch(typeof(PlayerPrefClient), nameof(PlayerPrefClient.Save))]
    [HarmonyPatch(new Type[] { typeof(string), typeof(object) })]
    [HarmonyPrefix]
    static bool SaveCustom(ref Task __result, string key, object value)
    {
        var myLogSource = Logger.CreateLogSource("MyLogSource");
        Logger.Sources.Add(myLogSource);
        myLogSource.LogInfo("Test");
        
        
        if (value is string value2)
        {
            PlayerPrefs.SetString(key, value2);
        }
        else
        {
            string value3 = JsonConvert.SerializeObject(value);
            PlayerPrefs.SetString(key, value3);
        }
        PlayerPrefs.SetString("Test", "Balls");
        __result = Task.CompletedTask;
        return false;
    }
    //public static Task Save(string key, object value) { }
}