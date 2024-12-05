using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoom
{
    public string roomName;

    public int resetGameCounter;

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
            this.player1 = player2;
            player2 = null;
        }
        else if (clientID == this.player2.clientId)
        {
            this.player2 = null;
        }

        if(this.player1 == null)
        {
            //delete room
            NetworkServerProcessing.gameLogic.roomDictionary.Remove(this.roomName);
        }
        else
        {
            NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.waitingForNewPlayer.ToString(), this.player1.clientId, TransportPipeline.ReliableAndInOrder);
        }
    }

    public void OnGameStart()
    {
        ResetGameBoard();
        SetPlayerTurn(player1);
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

}

/*
PRIORITY
-̶ s̶e̶n̶d̶ w̶i̶n̶/̶l̶o̶s̶e̶ c̶o̶n̶d̶i̶t̶i̶o̶n̶ t̶o̶ c̶l̶i̶e̶n̶t̶
-̶ s̶t̶o̶p̶ g̶a̶m̶e̶
-̶ r̶e̶s̶e̶t̶ g̶a̶m̶e̶ o̶n̶ s̶e̶r̶v̶e̶r̶/̶c̶l̶i̶e̶n̶t̶
-̶ l̶e̶a̶v̶i̶n̶g̶ g̶a̶m̶e̶ o̶n̶ s̶e̶r̶v̶e̶r̶ s̶i̶d̶e̶
    1̶.̶ p̶e̶r̶s̶o̶n̶ w̶h̶o̶ l̶e̶a̶v̶e̶s̶ n̶e̶e̶d̶ g̶e̶t̶ r̶e̶m̶o̶v̶e̶d̶ f̶r̶o̶m̶ g̶a̶m̶e̶ r̶o̶o̶m̶
    2̶.̶ p̶e̶r̶s̶o̶n̶ w̶h̶o̶ d̶i̶d̶n̶t̶ l̶e̶a̶v̶e̶ n̶e̶e̶d̶s̶ t̶o̶ g̶e̶t̶ s̶e̶n̶t̶ t̶o̶ w̶a̶i̶t̶i̶n̶g̶ r̶o̶o̶m̶
    3̶.̶ i̶f̶ b̶o̶t̶h̶ p̶e̶o̶p̶l̶e̶ l̶e̶a̶v̶e̶,̶ d̶e̶l̶e̶t̶e̶ r̶o̶o̶m̶
-̶ d̶e̶l̶e̶t̶e̶ e̶m̶p̶t̶y̶ r̶o̶o̶m̶s̶

BACKBURNER
- send message to other player
- observer
*/
