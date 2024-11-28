using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int clientId;
    public string clientPlayerName;

    public Player(int clientId, string clientPlayerName) 
    {
        this.clientId = clientId;
        this.clientPlayerName = clientPlayerName;
    }
}
