using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoom
{
    public string roomName;

    public Player player1;
    public Player player2;

    public short [,] gameplayBoard = new short[3,3];

    public GameRoom(string roomName, Player player1, short[,] gameplayBoard)
    {
        this.roomName = roomName;
        this.player1 = player1;
        this.gameplayBoard = gameplayBoard;
    }

    public void AddPlayer2(Player player2)
    {
        this.player2 = player2;
    }
}
