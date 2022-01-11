using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public string username;
    public int actor;
    public int selectedCard;
    public int countWin;
    public int inHandCard1, inHandCard2, inHandCard3, inHandCardCount;


    public PlayerInfo(string _username, int _actor)
    {
        username = _username;
        actor = _actor;
        countWin = 0;
        selectedCard = 0;
        inHandCard1 = 0;
        inHandCard2 = 0;
        inHandCard3 = 0;
        inHandCardCount = 0;
    }

    public static int checkInhandCard(PlayerInfo player)
    {
        if (player.inHandCard1 == 0)
        {
            return 1;
        }
        else if (player.inHandCard2 == 0)
        {
            return 2;
        }
        else if (player.inHandCard3 == 0)
        {
            return 3;
        }
        else
        {
            return 0;
        }
    }
}
