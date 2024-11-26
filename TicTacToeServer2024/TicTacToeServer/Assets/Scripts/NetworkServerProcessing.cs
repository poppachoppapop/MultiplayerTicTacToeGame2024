using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkServerProcessing
{

    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received =  " + msg + ", from connection id = " + clientConnectionID + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(',');

        gameLogic.ProcessMessageFromClient(csv, clientConnectionID);
    }
    static public void SendMessageToClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        networkServer.SendMessageToClient(msg, clientConnectionID, pipeline);
    }

    #endregion

    #region Connection Events

    static public void ConnectionEvent(int clientConnectionID)
    {
        Debug.Log("Client connection, ID == " + clientConnectionID);
    }
    static public void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log("Client disconnection, ID == " + clientConnectionID);
    }

    #endregion

    #region Setup
    static NetworkServer networkServer;
    static GameLogic gameLogic;

    static public void SetNetworkServer(NetworkServer NetworkServer)
    {
        networkServer = NetworkServer;
    }
    static public NetworkServer GetNetworkServer()
    {
        return networkServer;
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
    public const int JoinRequestedRoom = 104;
    public const int StartGame = 105;
    public const int ExitGame = 106;
    public const int SendMove = 107;
    public const int ResetGame = 108;
}

static public class ServerToClientSignifiers
{
    public const int LoginAttemptSuccessful = 201;
    public const int RegisterAccountSuccessful = 202;
    public const int AccountErrorMessage = 203;
    public const int CreateAvailableRoom = 205;
    public const int JoinAvailableRoom = 206;
}

#endregion

