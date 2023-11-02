using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    /* Step for each player */
    // step 0: select avaliable card
    // step 1: select avaliable place
    // step 2: proceed
    
    public Model model;
    
    public int selectCardIndex = -1;
    public int[] selectCardLocation = new int[] {-1, -1};

    public void Update()
    {
        if(selectCardIndex >= 0 && selectCardLocation[0] >= 0 && selectCardLocation[0] >= 0)
        {
            model.PlaceBot(selectCardIndex, selectCardLocation);
            Init();
        }
    }

    public void Init()
    {
        selectCardIndex = -1;
        selectCardLocation = new int[] {-1, -1};
    }

    public void SelectCardBtn(int cardIndex)
    {
        selectCardIndex = cardIndex;
    }

    public void SelectLaneBtn(int lane)
    {
        selectCardLocation[0] = lane;
    }

    public void SelectColBtn(int col)
    {
        selectCardLocation[1] = col;
    }

    public void EndTurnBtn()
    {
        model.EndTurn();
        Init();
    }

    public void ResetBtn()
    {
        SceneManager.LoadScene(0);
    }

}
