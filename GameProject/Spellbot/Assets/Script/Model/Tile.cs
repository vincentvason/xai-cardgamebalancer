using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    // "Player": bot["Player"],
    // "Name": "Fire",
    // "Param": bot["AbilityParam"],
    // "Location": bot["Location"]
    public string playerName;
    public string tileName;
    public int tileParam;
    public int[] location;

    // Start is called before the first frame update
    public Tile(Bot bot, string tileName)
    {
        playerName = bot.playerName;
        this.tileName = tileName;
        tileParam = bot.abilityParam;
        location = new int[] {bot.location[0], bot.location[1]};
    }
}
