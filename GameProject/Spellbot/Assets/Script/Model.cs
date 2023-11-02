using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    [Header("MVC")]
    [SerializeField] Controller controller;
    [SerializeField] View view;
    
    [Header("Board Stats")]
    [SerializeField] Board board;
    [SerializeField] int startCard;
    [SerializeField] int currentOrder = 0;
    [SerializeField] int[] turnOrder = new int[] {0, 1, 2};
    [SerializeField] int result = -1;


    // Start is called before the first frame update
    void Start()
    {
        // Start the game
        board.players[0].SetupCard();
        board.players[1].SetupCard();
        board.players[0].DrawCard(startCard);
        board.players[1].DrawCard(startCard);
        
        view.CleanPlaceButtonUI();
        view.CleanBotUI();
        view.CleanTileStatusUI();
        
        view.UpdateCardUI(board.players[0],board.players[0].GetAllCardsInHand());
    }

    // Update is called once per frame
    void Update()
    {
        // Always Update
        view.UpdateTurfUI(board);
        view.UpdateStatsUI(board,turnOrder[currentOrder]);

        if(controller.selectCardIndex >= 0)
        {
            view.UpdatePlaceButtonUI(board,turnOrder[currentOrder]);
        }
        else
        {
            view.SetCommandText("Player "+ (turnOrder[currentOrder] + 1) +"'s turn. Please select a card or end your turn.");
            view.CleanPlaceButtonUI();
        }
    }

    public void PlaceBot(int cardIndex, int[] location)
    {
        board.PlaceBot(turnOrder[currentOrder], cardIndex, location);
        view.UpdateCardUI(board.players[turnOrder[currentOrder]],board.players[turnOrder[currentOrder]].GetAllCardsInHand());
        view.UpdateBotUI(board.spaces);
        view.UpdateTileStatusUI(board.tiles);
    }

    public void EndTurn()
    {
        currentOrder++;

        if(currentOrder == 2)
        {
            turnOrder[0] = 1 - turnOrder[0];
            turnOrder[1] = 1 - turnOrder[0];
            result = board.Proceed();

            if(result >= 0)
            {
                view.EndGame();
            }
            
            currentOrder = 0;
        }
        
        view.UpdateCardUI(board.players[turnOrder[currentOrder]],board.players[turnOrder[currentOrder]].GetAllCardsInHand());
        view.UpdateBotUI(board.spaces);
        view.UpdateTileStatusUI(board.tiles);    
    }
}
