using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class View : MonoBehaviour
{
    [Header("Log")]
    public GameObject logContent;
    public GameObject logPrefab;

    [Header("Command")]
    public TMP_Text commandText;
    public GameObject endBtnObj;
    public GameObject endGameObj;
    
    [Header("Card View")]
    public GameObject cardContent;
    public GameObject cardPrefab;

    [Header("Placeable View")]
    public List<GameObject> placeBtnObject;

    [Header("Bot View")]
    public List<GameObject> botObject;

    [Header("Tile View")]
    public List<GameObject> tileStatusObject;

    [Header("Turf View")]
    public GameObject[] tileObject;
    public Color redColor;
    public Color neutralColor;
    public Color blueColor;

    [Header("Stats View")]
    public TMP_Text turn;
    public TMP_Text whoFirst;
    public GameObject p1CurrentTurn;
    public GameObject p2CurrentTurn;
    public TMP_Text p1Mana;
    public TMP_Text p2Mana;
    public TMP_Text[] p1Health;
    public TMP_Text[] p2Health;

    public void UpdateTurfUI(Board board)
    {
        int[] p1ctrl = board.players[0].ctrl;
        int[] p2ctrl = board.players[1].ctrl;
        int width = tileObject.Length/3;

        for(int lane = 0; lane < 3; lane++)
        {
            for(int i = 1; i <= width/2; i++)
            {
                if(p1ctrl[lane] >= i) tileObject[(lane*width)+i-1].GetComponent<Image>().color = redColor;
                else tileObject[(lane*width)+i-1].GetComponent<Image>().color = neutralColor;

                if(p2ctrl[lane] >= i) tileObject[((lane+1)*width)-i].GetComponent<Image>().color = blueColor;
                else tileObject[((lane+1)*width)-i].GetComponent<Image>().color = neutralColor;
            }
        }
    }

    public void UpdatePlaceButtonUI(Board board, int currentPlayerIndex)
    {
        List<int[]> locations = board.GetAvaliableLocation(currentPlayerIndex);
        
        foreach (GameObject placeBtn in placeBtnObject)
        {
            placeBtn.SetActive(false);
        }

        foreach (int[] location in locations)
        {
            //Debug.Log(location[0] + "," + location[1]);
            int index = (location[0]*board.width) + location[1];
            placeBtnObject[index].SetActive(true);
        }
    }

    public void CleanPlaceButtonUI()
    {
        foreach (GameObject placeBtn in placeBtnObject)
        {
            placeBtn.SetActive(false);
        }
    }

    public void CleanBotUI()
    {
        foreach (GameObject bot in botObject)
        {
            bot.SetActive(false);
        }
    }

    public void UpdateBotUI(List<Bot> bots)
    {
        foreach (GameObject bot in botObject)
        {
            bot.SetActive(false);
        }

        if(bots != null)
        {
            foreach (Bot bot in bots)
            {
                int index = (bot.location[0] * (botObject.Count/3)) + bot.location[1];
                botObject[index].SetActive(true);
                botObject[index].GetComponent<BotUI>().SetBotDetail(bot);
            }
        }
    }

    public void CleanTileStatusUI()
    {
        foreach (GameObject tileStatus in tileStatusObject)
        {
            tileStatus.SetActive(false);
        }
    }

    public void UpdateTileStatusUI(List<Tile> tiles)
    {
        foreach (GameObject tileStatus in tileStatusObject)
        {
            tileStatus.SetActive(false);
        }

        if(tiles != null)
        {
            string player1Name = GameObject.Find("Board").GetComponent<Board>().players[0].playerName;
            string player2Name = GameObject.Find("Board").GetComponent<Board>().players[1].playerName;

            foreach (Tile tile in tiles)
            {
                int index = (tile.location[0] * (tileStatusObject.Count/3)) + tile.location[1];
                tileStatusObject[index].SetActive(true);
                tileStatusObject[index].GetComponent<TMP_Text>().text = tile.tileName;
                
                if(tile.playerName == player1Name)
                {
                    tileStatusObject[index].GetComponent<TMP_Text>().color = Color.red;
                }
                else if(tile.playerName == player2Name)
                {
                    tileStatusObject[index].GetComponent<TMP_Text>().color = Color.blue;
                }
            }
        }
    }

    public void UpdateCardUI(Player player, List<Card> cards)
    {
        while (cardContent.transform.childCount > 0) {
            DestroyImmediate(cardContent.transform.GetChild(0).gameObject);
        }

        foreach(Card card in cards)
        {
            bool isAbleToPlay = (card.mana <= player.mp);

            GameObject cardUI = Instantiate(cardPrefab);
            cardUI.GetComponent<CardUI>().SetCardDetail(card, isAbleToPlay);
            cardUI.transform.SetParent(cardContent.transform);
            cardUI.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void UpdateStatsUI(Board board, int currentPlayerIndex)
    {
        /* Turn */
        turn.text = "Turn " + board.turn + "/" + board.turnLimit;

        /* You First */
        if(board.turn % 2 == 1)
        {
            whoFirst.text = "P1 First";
        }
        else if(board.turn % 2 == 0)
        {
            whoFirst.text = "P2 First";
        }
        
        /* Current Player */
        if(currentPlayerIndex == 0)
        {
            p1CurrentTurn.SetActive(true);
            p2CurrentTurn.SetActive(false);
        }
        else if(currentPlayerIndex == 1)
        {
            p1CurrentTurn.SetActive(false);
            p2CurrentTurn.SetActive(true);
        }

        /* Mana */
        p1Mana.text = (board.players[0].mp).ToString();
        p2Mana.text = (board.players[1].mp).ToString();

        /* HP */
        for(int i = 0; i < 3; i++)
        {
            p1Health[i].text = (board.players[0].hp[i]).ToString();
            p2Health[i].text = (board.players[1].hp[i]).ToString();
        }
    }

    public void Log(string msg)
    {
        Debug.Log(DateTime.Now.ToString() + " " + msg);
        GameObject logMsg = Instantiate(logPrefab);
        logMsg.GetComponent<TMP_Text>().text = DateTime.Now.ToString() + " " + msg;
        logMsg.transform.SetParent(logContent.transform);
        logMsg.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void SetCommandText(string msg)
    {
        commandText.text = msg;
    }

    public void EndGame()
    {
        cardContent.SetActive(false);
        endBtnObj.SetActive(false);
        endGameObj.SetActive(true);
    }
}
