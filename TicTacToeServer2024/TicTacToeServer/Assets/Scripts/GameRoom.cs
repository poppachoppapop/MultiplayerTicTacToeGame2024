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

    public void RemovePlayerFromRoom(int clientID)
    {
        if (clientID == this.player1.clientId)
        {
            this.player1 = null;
        }
        else if (clientID == this.player2.clientId)
        {
            this.player2 = null;
        }

        //CheckIfGameRoomEmpty(this.roomName);
    }

    public void OnGameStart()
    {
        SetPlayerTurn(player1);
        ResetGameBoard();
    }

    public void PlayerSentMove(short column, short row)
    {
        bool stopGame = false;

        string updateAllClientBoards = null;

        if (currentTurn == player1)
        {
            SetMoveOnBoard(column, row, player1Move);
            updateAllClientBoards = ServerGameLogic.conjoinStrings(ServerToClientSignifiers.UpdateClientBoards.ToString(),
            column.ToString(),
            row.ToString(),
            player1Move.ToString());
            stopGame = CheckTicTacToeWinCondition(column, row, player1Move);
        }
        else
        {
            SetMoveOnBoard(column, row, player2Move);
            updateAllClientBoards = ServerGameLogic.conjoinStrings(ServerToClientSignifiers.UpdateClientBoards.ToString(),
            column.ToString(),
            row.ToString(),
            player2Move.ToString());
            stopGame = CheckTicTacToeWinCondition(column, row, player2Move);
        }

        NetworkServerProcessing.SendMessageToClient(updateAllClientBoards, player1.clientId, TransportPipeline.ReliableAndInOrder);
        NetworkServerProcessing.SendMessageToClient(updateAllClientBoards, player2.clientId, TransportPipeline.ReliableAndInOrder);

        if (!stopGame)
        {
            if (currentTurn == player1)
            {
                SetPlayerTurn(player2);
            }
            else
            {
                SetPlayerTurn(player1);
            }
        }

    }

    public bool CheckTicTacToeWinCondition(short column, short row, short playerMove)
    {
        if (CheckVerticalAxis(column, playerMove))
        {
            if (playerMove == player1Move)
            {
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.WinnerSignifier.ToString(), player1.clientId, TransportPipeline.ReliableAndInOrder);
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.LoserSignifier.ToString(), player2.clientId, TransportPipeline.ReliableAndInOrder);
            }
            else if (playerMove == player2Move)
            {
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.WinnerSignifier.ToString(), player2.clientId, TransportPipeline.ReliableAndInOrder);
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.LoserSignifier.ToString(), player1.clientId, TransportPipeline.ReliableAndInOrder);
            }
            return true;
        }
        else if (CheckHorizontalAxis(row, playerMove))
        {
            if (playerMove == player1Move)
            {
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.WinnerSignifier.ToString(), player1.clientId, TransportPipeline.ReliableAndInOrder);
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.LoserSignifier.ToString(), player2.clientId, TransportPipeline.ReliableAndInOrder);
            }
            else if (playerMove == player2Move)
            {
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.WinnerSignifier.ToString(), player2.clientId, TransportPipeline.ReliableAndInOrder);
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.LoserSignifier.ToString(), player1.clientId, TransportPipeline.ReliableAndInOrder);
            }
            return true;
        }
        else if (CheckDiagonalDownRight(playerMove))
        {
            if (playerMove == player1Move)
            {
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.WinnerSignifier.ToString(), player1.clientId, TransportPipeline.ReliableAndInOrder);
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.LoserSignifier.ToString(), player2.clientId, TransportPipeline.ReliableAndInOrder);
            }
            else if (playerMove == player2Move)
            {
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.WinnerSignifier.ToString(), player2.clientId, TransportPipeline.ReliableAndInOrder);
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.LoserSignifier.ToString(), player1.clientId, TransportPipeline.ReliableAndInOrder);
            }
            return true;
        }
        else if (CheckDiagonalDownLeft(playerMove))
        {
            if (playerMove == player1Move)
            {
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.WinnerSignifier.ToString(), player1.clientId, TransportPipeline.ReliableAndInOrder);
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.LoserSignifier.ToString(), player2.clientId, TransportPipeline.ReliableAndInOrder);
            }
            else if (playerMove == player2Move)
            {
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.WinnerSignifier.ToString(), player2.clientId, TransportPipeline.ReliableAndInOrder);
                NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.LoserSignifier.ToString(), player1.clientId, TransportPipeline.ReliableAndInOrder);
            }
            return true;
        }
        else if (CheckIfTieHappens())
        {
            //no players ween!
            NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.TieGameSignifier.ToString(), player1.clientId, TransportPipeline.ReliableAndInOrder);
            NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.TieGameSignifier.ToString(), player2.clientId, TransportPipeline.ReliableAndInOrder);
            return true;
        }
        //no ween yet :3
        return false;
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

    #region win condition checks

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

    #endregion

    public bool CheckIfGameRoomEmpty()
    {
        if (this.player1 != null && this.player2 != null)
        {
            return false;
        }

        return true;
    }

    // public void DeleteGameRoom()
    // {
    //     this.roomName = null;
    //     this.player1 = null;
    //     this.player2 = null;
    //     this.gameplayBoard = null;
    // }



}

/*
PRIORITY
- send win/lose condition to client
- stop game
- reset game on server/client
- leaving game on server side
- delete empty rooms

BACKBURNER
- send message to other player
- observer
*/
