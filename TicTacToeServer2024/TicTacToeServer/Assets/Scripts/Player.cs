using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int clientId;
    public string clientPlayerName;
    public GameRoom currentGameRoom;

    public Player(int clientId, string clientPlayerName) 
    {
        this.clientId = clientId;
        this.clientPlayerName = clientPlayerName;
    }

    public void SetCurrentGameRoom(GameRoom currentGameRoom)
    {
        this.currentGameRoom = currentGameRoom;
    }
}
