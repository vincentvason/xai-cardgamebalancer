using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotUI : MonoBehaviour
{
    Controller controller;
    View view;

    [Header("Image Setup")]
    string player1Name = "P1";
    string player2Name = "P2";
    public GameObject player1Obj;
    public GameObject player2Obj;

    [Header("Detail")]
    public string playerName;
    public string botDesc;
    public string botSpecialEffect;
    public int[] botLocation;
    
    public TMP_Text botNameTMP;
    public TMP_Text botHealthTMP;
    public TMP_Text botAttackTMP;
    public Button botDescBtn;

    void Start()
    {
        botDescBtn.onClick.AddListener(Inspect);
    }

    public void SetBotDetail(Bot bot)
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        view = GameObject.Find("View").GetComponent<View>();
        player1Name = GameObject.Find("Board").GetComponent<Board>().players[0].playerName;
        player2Name = GameObject.Find("Board").GetComponent<Board>().players[1].playerName;

        /* Image */
        if(player1Name == bot.playerName)
        {
            player1Obj.SetActive(true);
            player2Obj.SetActive(false);
        }
        else if(player2Name == bot.playerName)
        {
            player1Obj.SetActive(false);
            player2Obj.SetActive(true);
        }

        /* Inspector */
        playerName = bot.playerName;
        botDesc = bot.desc;
        botLocation = bot.location;
        
        /* Details */
        botNameTMP.text = bot.botName;
        botHealthTMP.text = bot.health.ToString();
        botAttackTMP.text = bot.attack.ToString();

        if(bot.specialEffect == "NaN")
        {
            botSpecialEffect = "Normal";
        }
        else
        {
            botSpecialEffect = bot.specialEffect;
        }

    }

    void Inspect()
    {
        view.Log("[Inspection] " + playerName + "'s " + botNameTMP.text + " @ " + botLocation[0] + "," + botLocation[1] + " H/A:" + botHealthTMP.text + "/" + botAttackTMP.text + " ["+ botSpecialEffect +"] Special: " + botDesc);
    }
}
