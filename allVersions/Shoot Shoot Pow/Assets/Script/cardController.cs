using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardController : MonoBehaviour
{
    [SerializeField] Material rock;
    [SerializeField] Material paper;
    [SerializeField] Material scissor;
    [SerializeField] static int posi = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckType());
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
        movementController.instance.moveCardToFrontOfPlayer(posi);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}