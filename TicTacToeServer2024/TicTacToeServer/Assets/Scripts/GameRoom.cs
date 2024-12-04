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


    bool winConditionMet;

    Player currentTurn;

    public GameRoom(string roomName, Player player1)
    {
        this.roomName = roomName;
        this.player1 = player1;
        player1.SetCurrentGameRoom(this);
    }

    public void AddPlayer2(Player player2)
    {
        this.player2 = player2;
        player2.SetCurrentGameRoom(this);
    }

    public void OnGameStart()
    {
        SetPlayerTurn(player1);
        ResetGameBoard();
    }

    public void PlayerSentMove(short column, short row)
    {

        string updateAllClientBoards = null;

        if (currentTurn == player1)
        {
            SetMoveOnBoard(column, row, player1Move);
            updateAllClientBoards = ServerGameLogic.conjoinStrings(ServerToClientSignifiers.UpdateClientBoards.ToString(),
            column.ToString(),
            row.ToString(),
            player1Move.ToString());
        }
        else
        {
            SetMoveOnBoard(column, row, player2Move);
            updateAllClientBoards = ServerGameLogic.conjoinStrings(ServerToClientSignifiers.UpdateClientBoards.ToString(),
            column.ToString(),
            row.ToString(),
            player2Move.ToString());
        }


        NetworkServerProcessing.SendMessageToClient(updateAllClientBoards, player1.clientId, TransportPipeline.ReliableAndInOrder);
        NetworkServerProcessing.SendMessageToClient(updateAllClientBoards, player2.clientId, TransportPipeline.ReliableAndInOrder);

        if (currentTurn == player1)
        {
            SetPlayerTurn(player2);
        }
        else
        {
            SetPlayerTurn(player1);
        }
    }

    public void CheckTicTacToeWinCondition(short column, short row, short playerMove)
    {
        if (CheckVerticalAxis(column, playerMove))
        {
            //player weens!
        }
        else if (CheckHorizontalAxis(row, playerMove))
        {
            //player weens!
        }
        else if (CheckDiagonalDownRight(playerMove))
        {
            //player weens!
        }
        else if (CheckDiagonalDownLeft(playerMove))
        {
            //player weens!
        }
        else if (CheckIfTieHappens())
        {
            //no players ween!
        }
        //no ween yet :3
    }

    public void ResetGameBoard()
    {
        for (int column = 0; column < 3; column++)
        {
            for (int row = 0; row < 3; row++)
            {
                this.gameplayBoard[column, row] = emptyBox;
            }
        }
    }

    public void SetMoveOnBoard(short column, short row, short playerMove)
    {
        gameplayBoard[column, row] = playerMove;
        CheckTicTacToeWinCondition(column, row, playerMove);
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

    public void GameOver()
    {

    }

    public bool CheckVerticalAxis(short column, short playerMove)
    {
        for (int row = 0; row < 3; row++)
        {
            if (gameplayBoard[column, row] != playerMove)
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckHorizontalAxis(short row, short playerMove)
    {
        for (int column = 0; column < 3; column++)
        {
            if (gameplayBoard[column, row] != playerMove)
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckDiagonalDownRight(short playerMove)
    {
        for (int i = 0; i < 3; i++)
        {
            if (gameplayBoard[i, i] != playerMove)
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckDiagonalDownLeft(short playerMove)
    {
        for (int row = 0; row < 3; row++)
        {
            if (gameplayBoard[2 - row, row] != playerMove)
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckIfTieHappens()
    {
        foreach (short move in gameplayBoard)
        {
            if (move == emptyBox)
            {
                return false;
            }

        }
        return true;
    }

}
