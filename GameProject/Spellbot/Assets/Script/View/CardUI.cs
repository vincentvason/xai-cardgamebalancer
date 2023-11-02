using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{    
    [Header("Button Controller")]
    Controller controller;
    View view;
    public int cardIndex;
    
    [Header("Card View")]
    public TMP_Text botNameTMP;
    public TMP_Text botHealthTMP;
    public TMP_Text botAttackTMP;
    public TMP_Text botManaTMP;
    public TMP_Text botDescTMP;
    public Button botPlayBtn;

    void Start()
    {
        botPlayBtn.onClick.AddListener(delegate 
            {
                controller.SelectCardBtn(cardIndex);
                view.SetCommandText(botNameTMP.text + " is currently selected. Please select a location to land.");
            }
        );
    }

    public void SetCardDetail(Card card, bool isAbleToPlay)
    {
        /* Card Index */
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        view = GameObject.Find("View").GetComponent<View>();
        cardIndex = card.cardIndex;
        Debug.Log("Card Detail Index: " + card.cardIndex);
        
        /* Details */
        botNameTMP.text = card.cardName;
        botHealthTMP.text = card.health.ToString();
        botAttackTMP.text = card.attack.ToString();
        botManaTMP.text = card.mana.ToString();
        botDescTMP.text = card.desc;
        botPlayBtn.interactable = isAbleToPlay;
        
    }

}
