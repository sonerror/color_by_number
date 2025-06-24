using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickCounter : MonoBehaviour, IPointerClickHandler
{
    private int clickCount = 0;
    public Button boosterButton;

    public void OnPointerClick(PointerEventData eventData)
    {
        clickCount++;
        if(clickCount == 3) 
        {
            if (boosterButton != null)
            {
                boosterButton.gameObject.SetActive(true);
            }
        }
        if (clickCount == 5)
        {
            ShowBoosterButton();
            clickCount = 0;
        }
    }
    void ShowBoosterButton()
    {
       // UIManager.Ins.OpenUI<Cheat>();
    }
    public void CloseCheatButton()
    {
        boosterButton.gameObject.SetActive(false);
    }
}