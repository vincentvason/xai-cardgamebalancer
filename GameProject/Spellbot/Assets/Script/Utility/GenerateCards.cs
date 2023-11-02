using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerateCards
{
    #if (UNITY_EDITOR)
    private static string CSVPath = "/CSV/data.csv";

    [MenuItem("Utilities/Generate Cards")]
    public static void Generate()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + CSVPath);

        int index = 0;
        foreach(string s in allLines)
        {
            string[] splitData = s.Split(',');

            if(splitData.Length != 12)
            {
                Debug.Log(s + "does not have 12 values");
                return;
            }

            Card card = ScriptableObject.CreateInstance<Card>();
            card.cardIndex = index;
            Debug.Log(splitData[0]);
            card.cardID = int.Parse(splitData[0]);
            card.cardName = splitData[2];
            card.mana = int.Parse(splitData[4]);
            card.health = int.Parse(splitData[5]);
            card.attack = int.Parse(splitData[6]);
            card.desc = splitData[7];
            card.ability = splitData[8];
            card.abilityParam = int.Parse(splitData[9]);
            card.abilityCondition = splitData[10];
            card.abilityTrigger = splitData[11];

            AssetDatabase.CreateAsset(card, $"Assets/Cards/" + (card.cardID).ToString() + $".asset");
        }
        
        AssetDatabase.SaveAssets();
    }
    #endif
}
