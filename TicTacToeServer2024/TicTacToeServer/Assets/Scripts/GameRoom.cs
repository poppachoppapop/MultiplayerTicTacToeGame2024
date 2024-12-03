using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoom
{
    public string roomName;

    public Player player1;
    public Player player2;

    public short[,] gameplayBoard = new short[3, 3];

    //tic tac toe moves
    const short emptyBox = 0;
    const short player1Move = 1;
    const short player2Move = 2;

    Player currentTurn;

    public GameRoom(string roomName, Player player1)
    {
        this.roomName = roomName;
        this.player1 = player1;
    }

    public void AddPlayer2(Player player2)
    {
        this.player2 = player2;
    }

    public void OnGameStart()
    {
        SetPlayerTurn(player1);
        ResetGameBoard();
    }

    public void UpdateGameBoard()
    {
        
    }

    public void CheckTicTacToeWinCondition()
    {

    }

    public void ResetGameBoard()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                this.gameplayBoard[x, y] = emptyBox;
            }
        }
    }

    public void SetPlayerTurn(Player player)
    {
        if (currentTurn != null)
        {
            NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.NotCurrentTurn.ToString(), currentTurn.clientId, TransportPipeline.ReliableAndInOrder);
        }

        currentTurn = player;
        NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.CurrentPlayerTurn.ToString(), currentTurn.clientId, TransportPipeline.ReliableAndInOrder);
    }


}
