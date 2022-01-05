using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{

    public static CardController instance;
    [SerializeField] Material rock;
    [SerializeField] Material paper;
    [SerializeField] Material scissor;
    [SerializeField] static int posi = 0;
    
    void Start()
    {
        // StartCoroutine(CheckType());
        // MoveCardToFrontOfPlayer(posi);
    }

    IEnumerator CheckType()
    {
        switch (DeckController.instance.cardDeck[0].cardID)
        {
            case 1:
                GetComponent<Renderer>().material = scissor;
                DeckController.instance.cardDeck.RemoveAt(0);
                posi++;
                Debug.Log(posi);
                yield return new WaitForSeconds(0.5f);
                break;
            case 2:
                GetComponent<Renderer>().material = paper;
                DeckController.instance.cardDeck.RemoveAt(0);
                posi++;
                Debug.Log(posi);
                yield return new WaitForSeconds(0.5f);
                break;
            case 3:
                GetComponent<Renderer>().material = rock;
                DeckController.instance.cardDeck.RemoveAt(0);
                posi++;
                Debug.Log(posi);
                yield return new WaitForSeconds(0.5f);
                break;    
        }
    }
    public void MoveCardToFrontOfPlayer (int posi)
    {
        switch (posi)
        {
            case 1:
                gameObject.GetComponent<Animator>().Play("firstPosiDraw");
                break;
            case 2:
                gameObject.GetComponent<Animator>().Play("secondPosiDraw");
                break;
            case 3:
                gameObject.GetComponent<Animator>().Play("thirdPositionDraw");
                break; 
        }
    }
}
