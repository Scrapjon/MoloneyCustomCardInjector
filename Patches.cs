using System;
using System.Collections.Generic;
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
            return inputFile.ReadToEnd();
        } 
        return PlayerPrefs.GetString(key);
    }



    [HarmonyPatch(typeof(DeckList), nameof(DeckList.LoadDeckList))]
    [HarmonyPrefix]
    static bool InjectDeckList(DeckList __instance, ref Task __result)
    {
        __result = InjectDeckListAsync(__instance);
        return false;
    }
    public static async Task InjectDeckListAsync(DeckList deckListObj)
    {
        {
            var deckList = await LoadCustom("CUSTOM_DECKS");
            if (deckList is { Count: > 0 })
            {
                deckListObj.InitList(deckList);
            }
            
        }

    }
    
    static Task<List<EvidenceDeck>> LoadCustom(string key)
    {
        var fileReaderLogSource = Logger.CreateLogSource("FileReader");
        Logger.Sources.Add(fileReaderLogSource);
        fileReaderLogSource.LogInfo("FileReader");
        
        string str = ReadFile(key);
        var output = !string.IsNullOrEmpty(str) ? Task.FromResult<List<EvidenceDeck>>(JsonConvert.DeserializeObject<List<EvidenceDeck>>(str)) : Task.FromResult<List<EvidenceDeck>>(default (List<EvidenceDeck>));
        return output;
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
    
  
    [HarmonyFinalizer]
   
    
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

