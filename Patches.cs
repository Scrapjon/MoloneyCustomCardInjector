using System;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using Network;
using Newtonsoft.Json;
using Logger = BepInEx.Logging.Logger;
using System.IO;



namespace MoloneyCustomCardInjector;

public class Patches
{
    private static string ReadFile(string key)
    {
        
        
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        docPath = Path.Combine(docPath, "Guilty as Sock Custom Decks");
        Directory.CreateDirectory(docPath);
        string filePath = Path.Combine(docPath, $"{key}.json");
        if (File.Exists(Path.Combine(filePath)))
        {
            using StreamReader inputFile = new StreamReader(filePath);
            return inputFile.ToString();
        } 
        return PlayerPrefs.GetString(key);
    } 
    private static string WriteFile(string key,string value) // file writing logic
    {
        
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        docPath = Path.Combine(docPath, "Guilty as Sock Custom Decks");
        Directory.CreateDirectory(docPath);
        string outputPath = Path.Combine(docPath, $"{key}.json");
        using StreamWriter outputFile = new StreamWriter(outputPath);
        outputFile.WriteLine(value);
        return $"Saved to: {outputPath}";
    }
    
    [HarmonyPatch(typeof(PlayerPrefClient), nameof(PlayerPrefClient.Load))]
    [HarmonyPatch(new Type[] { typeof(string) })]
    [HarmonyPrefix]
    static bool LoadCustom(ref Task __result, string key)
    {
        var fileReaderLogSource = Logger.CreateLogSource("FileReader");
        Logger.Sources.Add(fileReaderLogSource);
        fileReaderLogSource.LogInfo("FileReader");
        
        string str = ReadFile(key);
        if (!string.IsNullOrEmpty(str))
        {
            __result = Task.FromResult(JsonConvert.DeserializeObject(str));
            return false;
        }
        return true;
    }
    
    
    [HarmonyPatch(typeof(PlayerPrefClient), nameof(PlayerPrefClient.Save))]
    [HarmonyPatch(new Type[] { typeof(string), typeof(object) })]
    [HarmonyPrefix]
    static bool SaveCustom(ref Task __result, string key, object value) // Save deck data to json file
    {
        var myLogSource = Logger.CreateLogSource("MyLogSource");
        Logger.Sources.Add(myLogSource);
        myLogSource.LogInfo("Test");
        
        
        if (value is string value2)
        {
            PlayerPrefs.SetString(key, value2);
            myLogSource.LogInfo($"Value 2: {key}, {value2}");
        }
        else
        {
            string value3 = JsonConvert.SerializeObject(value);
            myLogSource.LogInfo($"Value 3: {key}, {value3}");
            PlayerPrefs.SetString(key, value3);
            myLogSource.LogInfo(WriteFile(key, value3)); //Logs path location (for now)
        }
        __result = Task.CompletedTask;
        return false;
    }
}

