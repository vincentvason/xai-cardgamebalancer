using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : ScriptableObject
{
    public int cardIndex;
    public int cardID;
    public string cardName;
    public int mana;
    public int health;
    public int attack;
    public string desc;
    public string ability;
    public int abilityParam;
    public string abilityCondition;
    public string abilityTrigger;
}
