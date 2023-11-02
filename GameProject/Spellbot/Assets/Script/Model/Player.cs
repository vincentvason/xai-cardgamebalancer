using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public string playerName;
    //Read Card from CSV
    [SerializeField] public List<Card> cards;
    //Player Stats
    [SerializeField] public int mp;
    [SerializeField] public int maxMp;
    [SerializeField] public int[] hp;
    [SerializeField] public int[] ctrl;
    //Organise Deck
    [SerializeField] public List<int> hand;
    [SerializeField] public List<int> deck;
    [SerializeField] public List<int> board;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < cards.Count; i++)
        {
            cards[i].cardIndex = i;
        }
    }

    public void SetupCard()
    {
        for(int i = 0; i < cards.Count; i++)
        {
            cards[i].cardIndex = i;
        }
    }

    public void SetMana(int mana)
    {
        mp = Mathf.Clamp(mana, 0, maxMp);
    }

    public void AddMana(int mana)
    {
        mp = Mathf.Clamp(mp + mana, 0, maxMp);
    }

    public void DrawCard(int n)
    {
        if(deck.Count >= n)
        {
            for(int i = 0; i < n; i++)
            {
                int index = Random.Range(0, deck.Count);
                hand.Add(deck[index]);
                deck.RemoveAt(index);
            }
        } 
    }

    public List<Card> GetAllCardsInDeck()
    {
        List<Card> selectedCards = new List<Card>();

        for(int i = 0; i < deck.Count; i++)
        {
            selectedCards.Add(cards[deck[i]]);
        }

        return selectedCards;
    }

    public List<Card> GetAllCardsInHand()
    {
        List<Card> selectedCards = new List<Card>();

        for(int i = 0; i < hand.Count; i++)
        {
            selectedCards.Add(cards[hand[i]]);
        }

        return selectedCards;
    }

    public Card GetCard(int cardIndex)
    {
        if(cardIndex < cards.Count)
        {
            return cards[cardIndex];
        }
        else
        {
            Debug.Log(cardIndex.ToString() + "is not existed.");
            return null;
        }
    }

    public Card GetCardFromHand(int cardIndex)
    {
        if(cardIndex < cards.Count && hand.IndexOf(cardIndex) >= 0)
        {
            return cards[cardIndex];
        }
        else
        {
            Debug.Log(cardIndex.ToString() + "is not existed.");
            return null;
        }
    }


    public void UsedCard(int cardIndex)
    {
        if(hand.IndexOf(cardIndex) >= 0)
        {
            board.Add(cardIndex);
            hand.Remove(cardIndex);
        }
        else
        {
            Debug.Log(cardIndex.ToString() + "is not existed.");
        }
    }

    public void ReturnCard(int cardIndex)
    {
        if(hand.IndexOf(cardIndex) >= 0)
        {
            deck.Add(cardIndex);
            hand.Remove(cardIndex);
        }
        else
        {
            Debug.Log(cardIndex.ToString() + "is not existed.");
        }
    }

    public void RecallCard(int cardIndex)
    {
        if(board.IndexOf(cardIndex) >= 0)
        {
            deck.Add(cardIndex);
            board.Remove(cardIndex);
        }
        else
        {
            Debug.Log(cardIndex.ToString() + "is not existed.");
        }
    }
}
