using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bot
{
    public string playerName;
    public int cardIndex;
    public string botName;
    public int health;
    public int attack;
    public string abilityName;
    public int abilityParam;
    public string abilityCondition;
    public string abilityTrigger;
    public int[] location;
    public string specialEffect;

    public string desc;

    public Bot(Player player, Card card, int cardIndex, int[] location)
    {
        playerName = player.playerName;
        this.cardIndex = cardIndex;
        botName = card.cardName;
        health = card.health;
        attack = card.attack;
        abilityName = card.ability;
        abilityParam = card.abilityParam;
        abilityCondition = card.abilityCondition;
        abilityTrigger = card.abilityTrigger;
        this.location = location;
        specialEffect = "NaN";

        desc = card.desc;
    }
}
