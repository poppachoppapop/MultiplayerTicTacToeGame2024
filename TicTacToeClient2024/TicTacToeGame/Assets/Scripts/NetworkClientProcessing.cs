using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkClientProcessing
{

    #region Send and Receive Data Functions
    static public void ReceivedMessageFromServer(string msg, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received =  " + msg + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(',');

        gameLogic.ProcessMessageFromServer(csv, pipeline);

    }

    static public void SendMessageToServer(string msg, TransportPipeline pipeline)
    {
        networkClient.SendMessageToServer(msg, pipeline);
    }

    #endregion

    #region Connection Related Functions and Events
    static public void ConnectionEvent()
    {
        Debug.Log("Network Connection Event!");
    }
    static public void DisconnectionEvent()
    {
        Debug.Log("Network Disconnection Event!");
    }
    static public bool IsConnectedToServer()
    {
        return networkClient.IsConnected();
    }
    static public void ConnectToServer()
    {
        networkClient.Connect();
    }
    static public void DisconnectFromServer()
    {
        networkClient.Disconnect();
    }

    #endregion

    #region Setup
    static NetworkClient networkClient;
    static GameLogic gameLogic;

    static public void SetNetworkedClient(NetworkClient NetworkClient)
    {
        networkClient = NetworkClient;
    }
    static public NetworkClient GetNetworkedClient()
    {
        return networkClient;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion

}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int LoginAccountInfo = 101;
    public const int RegisterAccountInfo = 102;
    public const int CheckIfRoomAvailable = 103;
    public const int SendMove = 104;
    public const int ExitGame = 105;
    public const int ResetGame = 106;
    public const int SendMessage = 107;
    public const int ReturnToLoginScreen = 108;
}

static public class ServerToClientSignifiers
{
    public const int LoginAttemptSuccessful = 201;
    public const int RegisterAccountSuccessful = 202;
    public const int AccountErrorMessage = 203;
    public const int WaitingForNewPlayer = 204;
    public const int StartGame = 205;
    public const int CurrentPlayerTurn = 206;
    public const int NotCurrentTurn = 207;
    public const int UpdateClientBoards = 208;
    public const int ResetGame = 209;
    public const int SendMessageToOtherPlayer = 210;
    public const int WinnerSignifier = 211;
    public const int LoserSignifier = 212;
    public const int TieGameSignifier = 213;
}
#endregion

