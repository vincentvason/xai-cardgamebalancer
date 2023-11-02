using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] public View view;

    [SerializeField] public int width = 8;

    [SerializeField] public int frame = 0;
    [SerializeField] public int turn = 1;
    [SerializeField] public int turnLimit = 50;

    [SerializeField] public List<Player> players;

    [SerializeField] public List<Bot> spaces;
    [SerializeField] public List<Tile> tiles;

    public void Start()
    {
        spaces = new List<Bot>();
        tiles = new List<Tile>();
    }
    
    
    public List<int[]> GetAvaliableLocation(int playerIndex)
    {
        List<int[]> list = new List<int[]>();

        for(int lane = 0; lane < 3; lane++)
        {
            for(int col = 0; col < width; col++)
            {
                int[] location = new int[] {lane, col};

                if((col == 0) && (players[0].hp[lane] == 0))
                {
                    // wall is destroyed
                }
                else if((col == width-1) && (players[1].hp[lane] == 0))
                {
                    // wall is destoryed
                }
                else if(GetIndexOfBot(location) >= 0)
                {
                    // already occupied
                }
                else
                {
                    if(playerIndex == 0 && players[0].ctrl[lane] >= col+1)
                    {
                        list.Add(location);
                    }
                    if(playerIndex == 1 && players[1].ctrl[lane] >= width-col)
                    {
                        list.Add(location);
                    }
                }
            }
        }

        return list;
    }

    // Place Bot
    public void PlaceBot(int playerIndex, int cardIndex, int[] location)
    {
        Card card = players[playerIndex].GetCardFromHand(cardIndex);
        Bot bot = new Bot(players[playerIndex], card, cardIndex, location);
        players[playerIndex].AddMana(-card.mana);
        players[playerIndex].UsedCard(cardIndex);
        spaces.Add(bot);
    }

    // Proceed - All action finished, move a bot, inflict tile ailment and attack
    public int Proceed()
    {
        EffectStart();
        MarchForward();
        EffectTile();
        AttackBot();
        AttackTower();
        WipeDeadBot();
        int res = CheckResult();
        DistributeMana();
        DrawCard();
        ExpandTurf();
        turn++;
        return res;
    }

    // Check if effect it is right condition
    public bool IsEffectMetCondition(Bot bot)
    {
        if(bot.abilityCondition == "PlaceTower")
        {
            if(((bot.playerName == players[0].playerName) && (bot.location[1] == 0)) || ((bot.playerName == players[1].playerName) && (bot.location[1] == width-1)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(bot.abilityCondition == "Death")
        {
            if(bot.health <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(bot.abilityCondition == "2InLane")
        {
            if(GetNumberOfBot(bot.playerName, bot.location[0]) >= 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(bot.abilityCondition == "IsInFront")
        {
            if(GetOrderOfBotInLane(bot.playerName, bot.location) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(bot.abilityCondition == "EnemyTurf")
        {
            if(((bot.playerName == players[0].playerName) && (bot.location[1] > width/2)) || ((bot.playerName == players[0].playerName) && (bot.location[1] < width/2)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public int GetOrderOfBotInLane(string playerName, int[] location)
    {
        List<int> list = new List<int>();
        if(spaces != null)
        {
            foreach (Bot bot in spaces)
            {
                if((bot.playerName == playerName) && (bot.location[0] == location[0]))
                {
                    list.Add(bot.location[1]);
                }
            }
        
            if(players[0].playerName == playerName)
            {
                list.Sort();
            }
            else if(players[1].playerName == playerName)
            {
                list.Sort();
                list.Reverse();
            }

            return list.IndexOf(location[1]);
        }
        else
        {
            return -1;
        }
                    
    }

    // get number of bot in each lane
    public int GetNumberOfBot(string playerName, int lane)
    {
        int count = 0;
        if(spaces != null)
        {
            foreach (Bot bot in spaces)
            {
                if((bot.playerName == playerName) && (bot.location[0] == lane))
                {
                    count++;
                }
            }
        }
        return count;
    }

    // Get the first bot in lane
    public int GetIndexOfFrontestBotInLane(string playerName, int lane)
    {
        List<int> list = new List<int>();
        if(spaces != null)
        {
            foreach (Bot bot in spaces)
            {
                if((bot.playerName == playerName) && (bot.location[0] == lane))
                {
                    list.Add(bot.location[1]);
                }
            }
        
            if(players[0].playerName == playerName)
            {
                list.Sort();
            }
            else if(players[1].playerName == playerName)
            {
                list.Sort();
                list.Reverse();
            }

            if(list.Count > 0)
            {
                list.Reverse();
                int[] location = new int[] {lane,list[0]};
                return GetIndexOfBot(location);
            }
        }
    
        return -1;
  
    }

    // Check if location forward to enemy is occupied
    public int GetIndexOfBot(int[] location, string playerName = "nan", int spaceForward = 0)
    {
        if(spaces != null)
        {
            foreach (Bot bot in spaces)
            {
                if((playerName == players[0].playerName) && (bot.location[0] == location[0]) && (bot.location[1] == location[1] + spaceForward))
                {
                    return spaces.IndexOf(bot);
                }
                else if((playerName == players[1].playerName) && (bot.location[0] == location[0]) && (bot.location[1] == location[1] - spaceForward))
                {
                    return spaces.IndexOf(bot);
                }
                else if((bot.location[0] == location[0]) && (bot.location[1] == location[1]) && (spaceForward == 0))
                {
                    return spaces.IndexOf(bot);
                }
            }
        }
        
        return -1;
    }

    // Check if tile effect applied
    public int IsTileApplied(Bot bot, string effectCheck)
    {
        if(tiles != null)
        {
            foreach (Tile tile in tiles)
            {
                if(bot.location == tile.location)
                {
                    if((effectCheck == "Energy") && (effectCheck == tile.tileName) && (bot.playerName == tile.playerName))
                    {
                        return tile.tileParam;
                    }
                    else if((effectCheck == "Fire") && (effectCheck == tile.tileName) && (bot.playerName != tile.playerName))
                    {
                        return tile.tileParam;
                    }
                    else if((effectCheck == "Frozen") && (effectCheck == tile.tileName) && (bot.playerName != tile.playerName))
                    {
                        return tile.tileParam;
                    }
                }
            }
        } 
        
        return 0;
    }

    // Effect at start
    public void EffectStart()
    {
        for(int currentBotIndex = 0; currentBotIndex < spaces.Count; currentBotIndex++)
        {
            if(spaces[currentBotIndex].abilityTrigger == "StartOnce" || spaces[currentBotIndex].abilityTrigger == "Start")
            {
                if(IsEffectMetCondition(spaces[currentBotIndex]) == true)
                {
                    // AllStatsUp - Increase both HP and Attack
                    if(spaces[currentBotIndex].abilityName == "AllStatsUp")
                    {
                        view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " HP rise from " + spaces[currentBotIndex].health + " to " + (spaces[currentBotIndex].health + spaces[currentBotIndex].abilityParam));
                        spaces[currentBotIndex].health = spaces[currentBotIndex].health + spaces[currentBotIndex].abilityParam;
                        view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " ATK rise from " + spaces[currentBotIndex].attack + " to " + (spaces[currentBotIndex].attack + spaces[currentBotIndex].abilityParam));
                        spaces[currentBotIndex].attack = spaces[currentBotIndex].attack + spaces[currentBotIndex].abilityParam;
                    }
                    // PlaceFireTrap - Place fire trap
                    else if(spaces[currentBotIndex].abilityName == "PlaceFireTrap")
                    {
                        Tile tile = new Tile(spaces[currentBotIndex],"Fire");
                        tiles.Add(tile);
                    }
                    // AtkUpAdjacent - Increase Attack on bot on the left and right
                    else if(spaces[currentBotIndex].abilityName == "AtkUpAdjacent")
                    {
                        if(spaces[currentBotIndex].location[0] == 0 || spaces[currentBotIndex].location[0] == 2)
                        {
                            int[] targetLocation = new int[] {1,spaces[currentBotIndex].location[1]};
                            if((GetIndexOfBot(targetLocation) >= 0) && (spaces[GetIndexOfBot(targetLocation)].playerName != spaces[currentBotIndex].playerName))
                            {
                                int targetBotIndex = GetIndexOfBot(targetLocation);
                                view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " ATK rise from " + spaces[targetBotIndex].attack + " to " + (spaces[targetBotIndex].attack + spaces[currentBotIndex].abilityParam));
                                spaces[targetBotIndex].attack = spaces[targetBotIndex].attack + spaces[currentBotIndex].abilityParam;
                            }
                        }
                        else if(spaces[currentBotIndex].location[0] == 1)
                        {
                            int[] targetLocation = new int[] {0,spaces[currentBotIndex].location[1]};
                            if((GetIndexOfBot(targetLocation) >= 0) && (spaces[GetIndexOfBot(targetLocation)].playerName != spaces[currentBotIndex].playerName))
                            {
                                int targetBotIndex = GetIndexOfBot(targetLocation);
                                view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " ATK rise from " + spaces[targetBotIndex].attack + " to " + (spaces[targetBotIndex].attack + spaces[currentBotIndex].abilityParam));
                                spaces[targetBotIndex].attack = spaces[targetBotIndex].attack + spaces[currentBotIndex].abilityParam;
                            }
                            
                            targetLocation = new int[] {0,spaces[currentBotIndex].location[1]};
                            if((GetIndexOfBot(targetLocation) >= 0) && (spaces[GetIndexOfBot(targetLocation)].playerName != spaces[currentBotIndex].playerName))
                            {
                                int targetBotIndex = GetIndexOfBot(targetLocation);
                                view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " ATK rise from " + spaces[targetBotIndex].attack + " to " + (spaces[targetBotIndex].attack + spaces[currentBotIndex].abilityParam));
                                spaces[targetBotIndex].attack = spaces[targetBotIndex].attack + spaces[currentBotIndex].abilityParam;
                            }
                        }
                    }
                    // Shockwave - Increase Attack on bot on the left and right
                    else if(spaces[currentBotIndex].abilityName == "Shockwave")
                    {
                        if(spaces[currentBotIndex].playerName == players[0].playerName)
                        {
                            if(GetIndexOfFrontestBotInLane(players[1].playerName,spaces[currentBotIndex].location[0]) >= 0)
                            {
                                int targetBotIndex = GetIndexOfFrontestBotInLane(players[0].playerName,spaces[currentBotIndex].location[0]);
                                view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " deal shockwave " + spaces[currentBotIndex].abilityParam + " damage to " + spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1]);
                                view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + (spaces[targetBotIndex].health - spaces[currentBotIndex].abilityParam));
                                spaces[targetBotIndex].health = spaces[targetBotIndex].health - spaces[currentBotIndex].abilityParam;
                            }
                        }
                        else if(spaces[currentBotIndex].playerName == players[1].playerName)
                        {
                            if(GetIndexOfFrontestBotInLane(players[0].playerName,spaces[currentBotIndex].location[0]) >= 0)
                            {
                                int targetBotIndex = GetIndexOfFrontestBotInLane(players[1].playerName,spaces[currentBotIndex].location[0]);
                                view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " deal shockwave " + spaces[currentBotIndex].abilityParam + " damage to " + spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1]);
                                view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + (spaces[targetBotIndex].health - spaces[currentBotIndex].abilityParam));
                                spaces[targetBotIndex].health = spaces[targetBotIndex].health - spaces[currentBotIndex].abilityParam;
                            }
                        }
                    }
                    // ManaBonus - Add Mana at the start of the game
                    else if(spaces[currentBotIndex].abilityName == "ManaBonus")
                    {
                        if(spaces[currentBotIndex].playerName == players[0].playerName)
                        {
                            players[0].AddMana(spaces[currentBotIndex].abilityParam);
                        }
                        else if(spaces[currentBotIndex].playerName == players[1].playerName)
                        {
                            players[1].AddMana(spaces[currentBotIndex].abilityParam);
                        }
                    }
                    // Place Energy Tile
                    else if(spaces[currentBotIndex].abilityName == "EnergyTile")
                    {
                        Tile tile = new Tile(spaces[currentBotIndex],"Energy");
                        tiles.Add(tile);
                    }
                    // Allies Atk Up
                    else if(spaces[currentBotIndex].abilityName == "AlliesAtkUp")
                    {
                        for(int targetBotIndex = 0; targetBotIndex < spaces.Count; targetBotIndex++)
                        {
                            view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " ATK rise from " + spaces[targetBotIndex].attack + " to " + (spaces[targetBotIndex].attack + spaces[currentBotIndex].abilityParam));
                            spaces[targetBotIndex].attack = spaces[targetBotIndex].attack + spaces[currentBotIndex].abilityParam;
                        }
                    }
                    // Atk Up
                    else if(spaces[currentBotIndex].abilityName == "AtkUp")
                    {
                        view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " ATK rise from " + spaces[currentBotIndex].attack + " to " + (spaces[currentBotIndex].attack + spaces[currentBotIndex].abilityParam));
                        spaces[currentBotIndex].attack = spaces[currentBotIndex].attack + spaces[currentBotIndex].abilityParam;
                    }
                    // Frozen Tile
                    else if(spaces[currentBotIndex].abilityName == "FrozenTile")
                    {
                        Tile tile = new Tile(spaces[currentBotIndex],"Frozen");
                        tiles.Add(tile);
                    }
                }
            }

            // If Start Once, change to used status
            if(spaces[currentBotIndex].abilityTrigger == "StartOnce")
            {
                spaces[currentBotIndex].abilityTrigger = "StartUsed";
            }
        }
        return;
    }

    // Move all piece
    public void MarchForward()
    {
        // If there is a wall in front, cannot move
        for(int currentBotIndex = 0; currentBotIndex < spaces.Count; currentBotIndex++)
        {
            int targetBotIndex = GetIndexOfBot(spaces[currentBotIndex].location, spaces[currentBotIndex].playerName, 1);
            int targetBot2Index = GetIndexOfBot(spaces[currentBotIndex].location, spaces[currentBotIndex].playerName, 1);

            // If there is a wall in front, cannot move
            if(((spaces[currentBotIndex].playerName == players[0].playerName) && (spaces[currentBotIndex].location[1] == width-2)) || ((spaces[currentBotIndex].playerName == players[1].playerName) && (spaces[currentBotIndex].location[1] == 1)))
            {
                view.Log("Cannot Move " + spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " because this bot is in front of opponent's wall.");
            }
            // If there is a bot in front, check if bot can jump
            else if(targetBotIndex >= 0 && spaces[currentBotIndex].playerName != spaces[targetBotIndex].playerName)
            {
                Debug.Log(GetIndexOfBot(spaces[currentBotIndex].location, spaces[currentBotIndex].playerName, 1));
                if((spaces[currentBotIndex].abilityName == "Jump") && targetBot2Index < 0)
                {
                    if((spaces[currentBotIndex].playerName == players[0].playerName) && (spaces[currentBotIndex].location[1]+2 <= width-1))
                    {
                        view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " jump to " + spaces[currentBotIndex].location[0] + "," + (spaces[currentBotIndex].location[1] + 2));
                        spaces[currentBotIndex].location[1] += 2;
                    }
                    else if((spaces[currentBotIndex].playerName == players[1].playerName) && (spaces[currentBotIndex].location[1]-2 >= 1))
                    {
                        view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " jump to " + spaces[currentBotIndex].location[0] + "," + (spaces[currentBotIndex].location[1] + 2));
                        spaces[currentBotIndex].location[1] += 2;
                    }
                }
                else
                {
                    view.Log("Cannot Move " + spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " because there is obstacle in front of them.");
                }
            }
            // If there is a bot two space in a front, only bot that near net (width/2 + 0.5) will move
            else if(targetBot2Index >= 0 && spaces[currentBotIndex].playerName != spaces[targetBot2Index].playerName && (targetBot2Index >= currentBotIndex))
            {
                if((spaces[currentBotIndex].playerName == players[0].playerName) && (Mathf.Abs(spaces[currentBotIndex].location[1] - width/2.0f) < Mathf.Abs(spaces[currentBotIndex].location[1]+2 - width/2.0f)))
                {
                    view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " move to " + spaces[currentBotIndex].location[0] + "," + (spaces[currentBotIndex].location[1] + 1) + "via net.");
                    spaces[currentBotIndex].location[1] += 1;
                }
                else if((spaces[currentBotIndex].playerName == players[1].playerName) && (Mathf.Abs(spaces[currentBotIndex].location[1] - width/2.0f) < Mathf.Abs(spaces[currentBotIndex].location[1]-2 - width/2.0f)))
                {
                    view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " move to " + spaces[currentBotIndex].location[0] + "," + (spaces[currentBotIndex].location[1] - 1) + "via net.");
                    spaces[currentBotIndex].location[1] -= 1;
                }
                else
                {
                    view.Log("Cannot Move " + spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " because there you are far from net more than opponent.");
                }
            }
            // If it has frozen tile, frozen for one turn
            else if(IsTileApplied(spaces[currentBotIndex],"Frozen") > 0)
            {
                if(spaces[currentBotIndex].specialEffect == "Freeze")
                {
                    if(spaces[currentBotIndex].playerName == players[0].playerName)
                    {
                        view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " move to " + spaces[currentBotIndex].location[0] + "," + (spaces[currentBotIndex].location[1] + 1));
                        spaces[currentBotIndex].location[1] += 1;
                    }
                    else if(spaces[currentBotIndex].playerName == players[1].playerName)
                    {
                        view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " move to " + spaces[currentBotIndex].location[0] + "," + (spaces[currentBotIndex].location[1] - 1));
                        spaces[currentBotIndex].location[1] -= 1;
                    }
                    spaces[currentBotIndex].specialEffect = "NaN";
                }
                else
                {
                    view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " is frozen.");
                    spaces[currentBotIndex].specialEffect = "Freeze";
                }
            }
            // else, moving
            else
            {
                if(spaces[currentBotIndex].playerName == players[0].playerName)
                {
                    view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " move to " + spaces[currentBotIndex].location[0] + "," + (spaces[currentBotIndex].location[1] + 1));
                    spaces[currentBotIndex].location[1] += 1;
                }
                else if(spaces[currentBotIndex].playerName == players[1].playerName)
                {
                    view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " move to " + spaces[currentBotIndex].location[0] + "," + (spaces[currentBotIndex].location[1] - 1));
                    spaces[currentBotIndex].location[1] -= 1;
                }
            }
        }

        return;
    }

    // Move all piece
    public void EffectTile()
    {
        // If there is a wall in front, cannot move
        for(int currentBotIndex = 0; currentBotIndex < spaces.Count; currentBotIndex++)
        {
            if(IsTileApplied(spaces[currentBotIndex],"Fire") > 0)
            {
                view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " HP fall from " + spaces[currentBotIndex].health + " to " + (spaces[currentBotIndex].health - IsTileApplied(spaces[currentBotIndex],"Fire")) + "by fire.");
                spaces[currentBotIndex].health = spaces[currentBotIndex].health - IsTileApplied(spaces[currentBotIndex],"Fire");
            }
        }
    }

    // Bot began to attack each other
    public void AttackBot()
    {
        // If there is a wall in front, cannot move
        for(int currentBotIndex = 0; currentBotIndex < spaces.Count; currentBotIndex++)
        {

            // Special Targeting: Diagonal
            if((spaces[currentBotIndex].abilityName == "DiagAttack"))
            {
                if(IsEffectMetCondition(spaces[currentBotIndex]) == true)
                {
                    if(spaces[currentBotIndex].location[0] == 0 || spaces[currentBotIndex].location[0] == 2)
                    {
                        int[] targetLocation = new int[] {1,spaces[currentBotIndex].location[1]};
                        int targetBotIndex = GetIndexOfBot(targetLocation, spaces[currentBotIndex].playerName, 1);
                        
                        if((targetBotIndex >= 0) && (spaces[targetBotIndex].playerName != spaces[currentBotIndex].playerName))
                        {
                            view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " attack to " + spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1]);
                            if((spaces[targetBotIndex].abilityName == "Defense") && (IsEffectMetCondition(spaces[targetBotIndex])))
                            {
                                Debug.Log("A");
                                view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + Mathf.Max(0, spaces[targetBotIndex].health - (spaces[currentBotIndex].attack - spaces[currentBotIndex].abilityParam)));
                                spaces[targetBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - (spaces[currentBotIndex].attack - spaces[currentBotIndex].abilityParam));
                            }
                            else
                            {
                                Debug.Log("B");
                                view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + (spaces[targetBotIndex].health - spaces[currentBotIndex].attack));
                                spaces[currentBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - spaces[currentBotIndex].attack);
                            }
                        }
                    }
                    if(spaces[currentBotIndex].location[0] == 1)
                    {
                        int[] targetLocation = new int[] {0,spaces[currentBotIndex].location[1]};
                        int targetBotIndex = GetIndexOfBot(targetLocation, spaces[currentBotIndex].playerName, 1);
                        
                        if((targetBotIndex >= 0) && (spaces[targetBotIndex].playerName != spaces[currentBotIndex].playerName))
                        {
                            view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " attack to " + spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1]);
                            if((spaces[targetBotIndex].abilityName == "Defense") && (IsEffectMetCondition(spaces[targetBotIndex])))
                            {
                                Debug.Log("C");
                                view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + Mathf.Max(0, spaces[targetBotIndex].health - (spaces[currentBotIndex].attack - spaces[currentBotIndex].abilityParam)));
                                spaces[targetBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - (spaces[currentBotIndex].attack - spaces[currentBotIndex].abilityParam));
                            }
                            else
                            {
                                Debug.Log("D");
                                view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + (spaces[targetBotIndex].health - spaces[currentBotIndex].attack));
                                spaces[currentBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - spaces[currentBotIndex].attack);
                            }
                        }

                        targetLocation = new int[] {2,spaces[currentBotIndex].location[1]};
                        targetBotIndex = GetIndexOfBot(targetLocation, spaces[currentBotIndex].playerName, 1);
                        
                        if((targetBotIndex >= 0) && (spaces[targetBotIndex].playerName != spaces[currentBotIndex].playerName))
                        {
                            view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " attack to " + spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1]);
                            if((spaces[targetBotIndex].abilityName == "Defense") && (IsEffectMetCondition(spaces[targetBotIndex])))
                            {
                                Debug.Log("E");
                                view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + Mathf.Max(0, spaces[targetBotIndex].health - (spaces[currentBotIndex].attack - spaces[currentBotIndex].abilityParam)));
                                spaces[targetBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - (spaces[currentBotIndex].attack - spaces[currentBotIndex].abilityParam));
                            }
                            else
                            {
                                Debug.Log("F");
                                view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + (spaces[targetBotIndex].health - spaces[currentBotIndex].attack));
                                spaces[currentBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - spaces[currentBotIndex].attack);
                            }
                        }
                    }
                }
            }
            // Special Targeting: Wide
            else if((spaces[currentBotIndex].abilityName == "WideAttack"))
            {
                if(IsEffectMetCondition(spaces[currentBotIndex]) == true)
                {
                    if(spaces[currentBotIndex].location[0] == 0 || spaces[currentBotIndex].location[0] == 1)
                    {
                        int[] targetLocation = new int[] {0,spaces[currentBotIndex].location[1]};
                        int targetBotIndex = GetIndexOfBot(targetLocation, spaces[currentBotIndex].playerName, 1);
                        
                        if((targetBotIndex >= 0) && (spaces[targetBotIndex].playerName != spaces[currentBotIndex].playerName))
                        {
                            view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " attack to " + spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1]);
                            if((spaces[targetBotIndex].abilityName == "Defense") && (IsEffectMetCondition(spaces[targetBotIndex])))
                            {
                                Debug.Log("G");
                                view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + Mathf.Max(0, spaces[targetBotIndex].health - (spaces[currentBotIndex].attack - spaces[currentBotIndex].abilityParam)));
                                spaces[targetBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - (spaces[currentBotIndex].attack - spaces[currentBotIndex].abilityParam));
                            }
                            else
                            {
                                Debug.Log("H");
                                view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + (spaces[targetBotIndex].health - spaces[currentBotIndex].attack));
                                spaces[currentBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - spaces[currentBotIndex].attack);
                            }
                        }
                    }

                    if(spaces[currentBotIndex].location[0] == 0 || spaces[currentBotIndex].location[0] == 1 || spaces[currentBotIndex].location[0] == 2)
                    {
                        int[] targetLocation = new int[] {1,spaces[currentBotIndex].location[1]};
                        int targetBotIndex = GetIndexOfBot(targetLocation, spaces[currentBotIndex].playerName, 1);
                        
                        if((targetBotIndex >= 0) && (spaces[targetBotIndex].playerName != spaces[currentBotIndex].playerName))
                        {
                            view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " attack to " + spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1]);
                            if((spaces[targetBotIndex].abilityName == "Defense") && (IsEffectMetCondition(spaces[targetBotIndex])))
                            {
                                Debug.Log("I");
                                view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + Mathf.Max(0, spaces[targetBotIndex].health - (spaces[currentBotIndex].attack - spaces[currentBotIndex].abilityParam)));
                                spaces[targetBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - (spaces[currentBotIndex].attack - spaces[currentBotIndex].abilityParam));
                            }
                            else
                            {
                                Debug.Log("J");
                                view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + (spaces[targetBotIndex].health - spaces[currentBotIndex].attack));
                                spaces[currentBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - spaces[currentBotIndex].attack);
                            }
                        }
                    }

                    if(spaces[currentBotIndex].location[0] == 1 || spaces[currentBotIndex].location[0] == 2)
                    {
                        int[] targetLocation = new int[] {2,spaces[currentBotIndex].location[1]};
                        int targetBotIndex = GetIndexOfBot(targetLocation, spaces[currentBotIndex].playerName, 1);

                        if((targetBotIndex >= 0) && (spaces[targetBotIndex].playerName != spaces[currentBotIndex].playerName))
                        {
                            view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " attack to " + spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1]);
                            if((spaces[targetBotIndex].abilityName == "Defense") && (IsEffectMetCondition(spaces[targetBotIndex])))
                            {
                                Debug.Log("K");
                                view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + Mathf.Max(0, spaces[targetBotIndex].health - (spaces[currentBotIndex].attack - spaces[currentBotIndex].abilityParam)));
                                spaces[targetBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - (spaces[currentBotIndex].attack - spaces[currentBotIndex].abilityParam));
                            }
                            else
                            {
                                Debug.Log("L");
                                view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + (spaces[targetBotIndex].health - spaces[currentBotIndex].attack));
                                spaces[currentBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - spaces[currentBotIndex].attack);
                            }
                        }
                    }
                    
                }
            }
            // if it is not special attack, perform normal attack
            else
            {
                int targetBotIndex = GetIndexOfBot(spaces[currentBotIndex].location, spaces[currentBotIndex].playerName, 1);
                int targetBot2Index = GetIndexOfBot(spaces[currentBotIndex].location, spaces[currentBotIndex].playerName, 2);
                int attackPower = spaces[currentBotIndex].attack;

                // Situational Attack Up
                if(spaces[currentBotIndex].abilityTrigger == "Attack")
                {
                    if((spaces[currentBotIndex].abilityName == "AtkUp") && (IsEffectMetCondition(spaces[currentBotIndex])))
                    {
                        Debug.Log("M");
                        view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " ATK rise from " + spaces[currentBotIndex].attack + " to " + (spaces[currentBotIndex].attack + spaces[currentBotIndex].abilityParam));
                        attackPower += spaces[currentBotIndex].abilityParam;
                    }
                }

                // Energy Tile
                if(IsTileApplied(spaces[currentBotIndex],"Energy") > 0)
                {
                    view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " ATK rise from " + spaces[currentBotIndex].attack + " to " + (spaces[currentBotIndex].attack + spaces[currentBotIndex].abilityParam) + "by energy.");
                    attackPower += spaces[currentBotIndex].abilityParam;
                }

                
                if((targetBotIndex >= 0) && (spaces[targetBotIndex].playerName != spaces[currentBotIndex].playerName))
                {
                    Debug.Log(spaces[targetBotIndex].botName);
                    Debug.Log(spaces[currentBotIndex].botName);
                    // Frozen
                    if((spaces[currentBotIndex].abilityName == "EnemyFrozenBot") && (spaces[targetBotIndex].specialEffect == "Freeze"))
                    {
                        attackPower += spaces[currentBotIndex].abilityParam;
                        view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " gain extra " +  spaces[currentBotIndex].abilityParam + " attack from frozen target");
                    }
                    
                    if((spaces[targetBotIndex].abilityName == "Defense") && (IsEffectMetCondition(spaces[targetBotIndex])))
                    {
                        Debug.Log("N");
                        view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + Mathf.Max(0, spaces[targetBotIndex].health - (attackPower - spaces[currentBotIndex].abilityParam)));
                        spaces[targetBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - (attackPower - spaces[currentBotIndex].abilityParam));
                    }
                    else
                    {
                        Debug.Log("O");
                        view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[targetBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + Mathf.Max(0, spaces[targetBotIndex].health - attackPower));
                        spaces[targetBotIndex].health = Mathf.Max(0, spaces[targetBotIndex].health - attackPower);
                    }
                }
                else if((spaces[currentBotIndex].abilityName == "Range") && (targetBot2Index >= 0) && (spaces[targetBot2Index].playerName != spaces[currentBotIndex].playerName))
                {
                    if((spaces[targetBot2Index].abilityName == "Defense") && (IsEffectMetCondition(spaces[targetBot2Index])))
                    {
                        Debug.Log("P");
                        view.Log(spaces[targetBot2Index].playerName + "'s " + spaces[targetBot2Index].botName + " @ " + spaces[targetBot2Index].location[0] + "," + spaces[targetBot2Index].location[1] + " HP fall from " + spaces[targetBot2Index].health + " to " + Mathf.Max(0, spaces[targetBot2Index].health - (attackPower - spaces[currentBotIndex].abilityParam)));
                        spaces[targetBot2Index].health = Mathf.Max(0, spaces[targetBot2Index].health - (attackPower - spaces[currentBotIndex].abilityParam));
                    }
                    else
                    {
                        Debug.Log("Q");
                        view.Log(spaces[targetBot2Index].playerName + "'s " + spaces[targetBot2Index].botName + " @ " + spaces[targetBot2Index].location[0] + "," + spaces[targetBot2Index].location[1] + " HP fall from " + spaces[targetBot2Index].health + " to " + Mathf.Max(0, spaces[targetBot2Index].health - attackPower));
                        spaces[targetBot2Index].health = Mathf.Max(0, spaces[targetBot2Index].health - attackPower);
                    }
                }
            }
        }
    
        return;
    }

    // Bot began to attack tower
    public void AttackTower()
    {
        for(int currentBotIndex = 0; currentBotIndex < spaces.Count; currentBotIndex++)
        {
            if((spaces[currentBotIndex].playerName == players[0].playerName) && (spaces[currentBotIndex].location[1] == width-2) && (players[1].hp[spaces[currentBotIndex].location[0]] > 0))
            {
                view.Log(players[1].playerName + "'s Wall in lane " + players[1].hp[spaces[currentBotIndex].location[0]] + " HP fall from " + players[1].hp[spaces[currentBotIndex].location[0]] + " to " + Mathf.Max(0,players[1].hp[spaces[currentBotIndex].location[0]] - spaces[currentBotIndex].attack));
                players[1].hp[spaces[currentBotIndex].location[0]] = Mathf.Max(0, players[1].hp[spaces[currentBotIndex].location[0]] - spaces[currentBotIndex].attack);
            }
            else if((spaces[currentBotIndex].playerName == players[1].playerName) && (spaces[currentBotIndex].location[1] == 1) && (players[0].hp[spaces[currentBotIndex].location[0]] > 0))
            {
                view.Log(players[1].playerName + "'s Wall in lane " + players[0].hp[spaces[currentBotIndex].location[0]] + " HP fall from " + players[0].hp[spaces[currentBotIndex].location[0]] + " to " + Mathf.Max(0,players[0].hp[spaces[currentBotIndex].location[0]] - spaces[currentBotIndex].attack));
                players[0].hp[spaces[currentBotIndex].location[0]] = Mathf.Max(0, players[0].hp[spaces[currentBotIndex].location[0]] - spaces[currentBotIndex].attack);
            }
        }

        return;
    }

    public void WipeDeadBot()
    {
        int[] manaBonus = new int[] {0, 0};
        for(int currentBotIndex = 0; currentBotIndex < spaces.Count; currentBotIndex++)
        {
            if(spaces[currentBotIndex].health <= 0)
            {
                // Wipe Skill before gain mana
                if((spaces[currentBotIndex].abilityTrigger == "Wipe") && (IsEffectMetCondition(spaces[currentBotIndex])))
                {
                    if(spaces[currentBotIndex].abilityName == "Bomb")
                    {
                        int targetBotIndex = GetIndexOfBot(spaces[currentBotIndex].location, spaces[currentBotIndex].playerName, 1);

                        if((targetBotIndex >= 0) && (spaces[targetBotIndex].playerName != spaces[currentBotIndex].playerName))
                        {
                            view.Log(spaces[targetBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[targetBotIndex].location[0] + "," + spaces[targetBotIndex].location[1] + " HP fall from " + spaces[targetBotIndex].health + " to " + (spaces[targetBotIndex].health - spaces[currentBotIndex].abilityParam));
                            spaces[targetBotIndex].health = spaces[targetBotIndex].health - spaces[currentBotIndex].abilityParam;
                        }
                    }
                }

                // Gain mana
                if(spaces[currentBotIndex].playerName == players[0].playerName)
                {
                    manaBonus[1]++;
                    players[0].RecallCard(spaces[currentBotIndex].cardIndex);
                }
                else if(spaces[currentBotIndex].playerName == players[1].playerName)
                {
                    manaBonus[0]++;
                    players[1].RecallCard(spaces[currentBotIndex].cardIndex);
                }
            }
        }

        for(int currentBotIndex = 0; currentBotIndex < spaces.Count; currentBotIndex++)
        {
            if(spaces[currentBotIndex].health <= 0)
            {
                // Wipe Skill after gain mana
                if((spaces[currentBotIndex].abilityTrigger == "Wipe") && (IsEffectMetCondition(spaces[currentBotIndex])))
                {
                    if(spaces[currentBotIndex].abilityName == "OwnBotWipe")
                    {
                        if((spaces[currentBotIndex].playerName == players[0].playerName) && (manaBonus[0] > 0))
                        {
                            view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " HP rise from " + spaces[currentBotIndex].health + " to " + (spaces[currentBotIndex].health + spaces[currentBotIndex].abilityParam));
                            spaces[currentBotIndex].health = spaces[currentBotIndex].health + spaces[currentBotIndex].abilityParam;
                            view.Log(spaces[currentBotIndex].playerName + "'s " + spaces[currentBotIndex].botName + " @ " + spaces[currentBotIndex].location[0] + "," + spaces[currentBotIndex].location[1] + " ATK rise from " + spaces[currentBotIndex].attack + " to " + (spaces[currentBotIndex].attack + spaces[currentBotIndex].abilityParam));
                            spaces[currentBotIndex].attack = spaces[currentBotIndex].attack + spaces[currentBotIndex].abilityParam;
                        }
                    }
                }
            }
        }

        //check if bot reach opposite side of wall
        for(int lane = 0; lane < 3; lane++)
        {
            int[] currentBotLocation = new int[] {lane,1};
            int currentBotIndex = GetIndexOfBot(currentBotLocation);
            if((currentBotIndex >= 0) && (spaces[currentBotIndex].playerName == players[1].playerName) && (players[0].hp[lane] == 0))
            {
                players[1].RecallCard(spaces[currentBotIndex].cardIndex);
            }
            
            currentBotLocation = new int[] {lane,width-2};
            currentBotIndex = GetIndexOfBot(currentBotLocation);
            if((currentBotIndex >= 0)  && (spaces[currentBotIndex].playerName == players[0].playerName) && (players[1].hp[lane] == 0))
            {
                players[0].RecallCard(spaces[currentBotIndex].cardIndex);
            }
        }

        //Remove bot
        for(int currentBotIndex = spaces.Count - 1; currentBotIndex >= 0; currentBotIndex--)
        {
            if(spaces[currentBotIndex].health <= 0)
            {
                spaces.RemoveAt(currentBotIndex);
            }
            else
            {
                //check if bot reach opposite side of wall
                for(int lane = 0; lane < 3; lane++)
                {
                    if((spaces[currentBotIndex].playerName == players[1].playerName) && (spaces[currentBotIndex].location[0] == lane) && (spaces[currentBotIndex].location[1] == 1) && (players[0].hp[lane] <= 0))
                    {       
                        spaces.RemoveAt(currentBotIndex);
                        break;
                    }
                    else
                    {
                        if((spaces[currentBotIndex].playerName == players[0].playerName) && (spaces[currentBotIndex].location[0] == lane) && (spaces[currentBotIndex].location[1] == width-2) && (players[1].hp[lane] <= 0))
                        {
                            spaces.RemoveAt(currentBotIndex);
                            break;
                        }
                    }
                }
            } 
        }

        

        players[0].AddMana(manaBonus[0]);
        players[1].AddMana(manaBonus[1]);

        return;
    }

    // Check win condition
    public int CheckResult()
    {
        List<int> p0hp = new List<int> {players[0].hp[0], players[0].hp[1], players[0].hp[2]};
        p0hp.Sort();
        List<int> p1hp = new List<int> {players[1].hp[0], players[1].hp[1], players[1].hp[2]};
        p1hp.Sort();

        int p0hpEliminatedCount = 0;
        int p1hpEliminatedCount = 0;

        foreach(int lane in p0hp)
        {
            if(lane == 0) p0hpEliminatedCount++;
        }

        foreach(int lane in p1hp)
        {
            if(lane == 0) p1hpEliminatedCount++;
        }

        if(p0hpEliminatedCount >= 2 && p1hpEliminatedCount >= 2)
        {
            if(p0hp[2] > p1hp[2])
            {
                view.Log("Player 1 Win by Last Tower!");
                return 0;
            }
            else if(p0hp[2] < p1hp[2])
            {
                view.Log("Player 2 Win by Last Tower!");
                return 1;
            }
            else
            {
                view.Log("Draw!");
                return 2; //Draw
            }
        }
        else if(p0hpEliminatedCount >= 2)
        {
            view.Log("Player 2 Win!");
            return 1;
        }
        else if(p1hpEliminatedCount >= 2)
        {
            view.Log("Player 1 Win!");
            return 0;
        }
        else if(turn >= turnLimit)
        {
            if(p0hp[0] > p1hp[0])
            {
                view.Log("Player 1 Win at sudden death!");
                return 0;
            }
            else if(p0hp[0] < p1hp[0])
            {
                view.Log("Player 2 Win at sudden death!");
                return 1;
            }
            else
            {
                if(p0hp[1] > p1hp[1])
                {
                    view.Log("Player 1 Win at sudden death!");
                    return 0;
                }
                else if(p0hp[1] < p1hp[1])
                {
                    view.Log("Player 2 Win at sudden death!");
                    return 1;
                }
                else
                {
                    if(p0hp[2] > p1hp[2])
                    {
                        view.Log("Player 1 Win at sudden death!");
                        return 0;
                    }
                    else if(p0hp[2] < p1hp[2])
                    {
                        view.Log("Player 2 Win at sudden death!");
                        return 1;
                    }
                    else
                    {
                        view.Log("Draw!");
                        return 2; //Draw
                    }
                }
            }
        }

        return -1; // No result
    }

    public void DrawCard()
    {
        players[0].DrawCard(1);
        players[1].DrawCard(1);
    }

    public void ExpandTurf()
    {
        for(int lane = 0; lane < 3; lane++)
        {
            if(GetIndexOfFrontestBotInLane(players[0].playerName, lane) >= 0)
            {
                int col = spaces[GetIndexOfFrontestBotInLane(players[0].playerName, lane)].location[1];
                players[0].ctrl[lane] = Mathf.Clamp(col+1, 2, width/2);
            }
            else
            {
                players[0].ctrl[lane] = 2;
            }

            if(GetIndexOfFrontestBotInLane(players[1].playerName, lane) >= 0)
            {
                int col = spaces[GetIndexOfFrontestBotInLane(players[1].playerName, lane)].location[1];
                players[1].ctrl[lane] = Mathf.Clamp(width-col, 2, width/2);
            }
            else
            {
                players[1].ctrl[lane] = 2;
            }
        }
    }

    // Check win condition
    public void DistributeMana()
    {
        List<int> p0hp = new List<int> {players[0].hp[0], players[0].hp[1], players[0].hp[2]};
        p0hp.Sort();
        List<int> p1hp = new List<int> {players[1].hp[0], players[1].hp[1], players[1].hp[2]};
        p1hp.Sort();

        int p0hpEliminatedCount = 0;
        int p1hpEliminatedCount = 0;

        if(p0hpEliminatedCount >= 1)
        {
            players[0].AddMana(5);
        }
        else
        {
            players[0].AddMana(2);
        }

        if(p1hpEliminatedCount >= 1)
        {
            players[1].AddMana(5);
        }
        else
        {
            players[1].AddMana(2);
        }

        return;
    }
}
