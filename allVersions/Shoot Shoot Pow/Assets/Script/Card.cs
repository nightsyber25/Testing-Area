using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//[CreateAssetMenu(fileName = "Card", menuName = "JKPTry/Card", order = 0)]
public class Card 
{
    // ID == 1 Scisscor
    // ID == 2 Paper
    // ID == 3 Rock
    // ID == (4-8) Special card
    public int cardID;
    public string cardName;
    public bool specialCard = false;

    private string[] cardNameArray = {"none","scissor","paper","rock","special1","special2","special3","special4","special5"};
    public Card(int id)
    {
        cardID = id;
        cardName = setCardName(id);
        if(id>3)
        {
            specialCard = true;
        }
        else
        {
            specialCard = false;
        }
    }

    public int getCardID()
    {
        return this.cardID;
    }

    private string setCardName(int id)
    {
        return cardNameArray[id];
    }

}
