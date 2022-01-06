using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeckController : MonoBehaviour
{
    public static DeckController instance;
    public List<Card> cardDeck = new List<Card>();
    public int[] numOfCardInDeck = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    private int[] cardProb = {1,1,1,1,1,1,2,2,2,2,2,2,3,3,3,3,3,3,4,4,5,5,6,6,7,7,8,8};
    [SerializeField] GameObject cardPrefab;
    GameObject card;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // GenerateDeck();
        //PrintDeck();
    }

    // public void PrintDeck()
    // {
    //     int i = 0;
    //      for (i = 0; i < 9; i++)
    //      {
    //          Debug.Log(numOfCardInDeck[i]);
    //      }

    //     foreach(Card card in cardDeck)
    //     {
    //         Debug.Log(i +": " + card.cardName);
    //         i++;
    //     }
    // }

    void Update()
    {

    }
    
    void CardTypeCount(int cardID)
    {
        switch (cardID)
        {
            case 0:
                numOfCardInDeck[0] += 1;
                break;
            case 1:
                numOfCardInDeck[1] += 1;
                break;
            case 2:
                numOfCardInDeck[2] += 1;
                break;
            case 3:
                numOfCardInDeck[3] += 1;
                break;
            case 4:
                numOfCardInDeck[4] += 1;
                break;
            case 5:
                numOfCardInDeck[5] += 1;
                break;
            case 6:
                numOfCardInDeck[6] += 1;
                break;
            case 7:
                numOfCardInDeck[7] += 1;
                break;
            case 8:
                numOfCardInDeck[8] += 1;
                break;
            default:
                Debug.LogError("Error, Invalid card ID was found");
                break;
        }
    }
    public List<Card> GenerateDeck()
    {
        int specialCardCount = 0;
        int cardRandom,cardCount;
        List<Card> cardDeckTemp = new List<Card>();
    
        for (cardCount = 0; cardCount < 40; cardCount++)
        {
            //Random ID in normal cards
            if (specialCardCount < 10)
            {
                cardRandom = cardProb[Random.Range(0, 28)];
                if (cardRandom > 3)
                {
                    specialCardCount++;
                }
            }
            else
            {
                cardRandom = Random.Range(1, 4);
            }
            Card temp = new Card(cardRandom);
            cardDeck.Add(temp);
            CardTypeCount(cardRandom);
            Debug.Log((cardCount + 1) + cardDeck[cardCount].cardName);
        }
        cardDeckTemp = cardDeck;
        
        return cardDeckTemp;
        // for (cardCount = 0; cardCount < 30; cardCount++)
        // {
        //     cardRandom = Random.Range(1, 4);
        //     Card temp = new Card(cardRandom);
        //     cardDeck.Add(temp);
        //     cardTypeCount(cardRandom);
        //     // Debug.Log((cardCount + 1) + cardDeck[cardCount].cardName);
        // }
    }

    public void SpawnDeck()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            card = PhotonNetwork.Instantiate(cardPrefab.name,new Vector3(1.294f,1.337f,-0.2f),Quaternion.identity);
            card = PhotonNetwork.Instantiate(cardPrefab.name,new Vector3(1.294f,1.316f,-0.2f),Quaternion.identity);
            card = PhotonNetwork.Instantiate(cardPrefab.name,new Vector3(1.294f,1.295f,-0.2f),Quaternion.identity);
        }
        else
        {
            card = PhotonNetwork.Instantiate(cardPrefab.name,new Vector3(1.294f,1.370f,-0.2f),Quaternion.identity);
            card = PhotonNetwork.Instantiate(cardPrefab.name,new Vector3(1.294f,1.355f,-0.2f),Quaternion.identity);
            card = PhotonNetwork.Instantiate(cardPrefab.name,new Vector3(1.294f,1.346f,-0.2f),Quaternion.identity);
        }
    }
    


}
