using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;



public class ServerGameLogic : GameLogic
{
    const char sepchar = ',';

    //list of players within the entire server
    Dictionary<int, Player> playersInServer = new Dictionary<int, Player>();

    Dictionary<string, GameRoom> roomDictionary = new Dictionary<string, GameRoom>();

    //2d array to represent tictactoe board
    short[,] gameplayBoard = new short[3, 3];

    //tic tac toe moves
    const short emptyBox = 0;
    const short player1Move = 1;
    const short player2Move = 2;

    string accountDirectoryPath;

    public void Awake()
    {
        accountDirectoryPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Accounts";
    }

    public void Start()
    {
        NetworkServerProcessing.SetGameLogic(this);
    }


    public override void ProcessMessageFromClient(string[] clientInstructions, int clientID)
    {
        int signifier = int.Parse(clientInstructions[0]);
        #region Login logic

        if (signifier == ClientToServerSignifiers.LoginAccountInfo)
        {
            //client is asking if login worked
            //message format (1,username,password)
            string clientUsernameInput = clientInstructions[1];
            string clientPasswordInput = clientInstructions[2];

            string failedLoginMessage = conjoinStrings(
                ServerToClientSignifiers.AccountErrorMessage.ToString(),
                "login attempt failed, account already exists, please try again."
                );

            if (AccountAlreadyExists(clientUsernameInput))
            {
                LoginClientWithCorrectCredentials(clientID, clientUsernameInput);
            }

            else if (!AccountAlreadyExists(clientUsernameInput))
            {
                NetworkServerProcessing.SendMessageToClient(
                    failedLoginMessage,
                    clientID,
                    TransportPipeline.ReliableAndInOrder
                );
            }
        }

        else if (signifier == ClientToServerSignifiers.RegisterAccountInfo)
        {
            string registerUsernameInput = clientInstructions[1];
            string registerPasswordInput = clientInstructions[2];

            if (AccountAlreadyExists(registerUsernameInput))
            {
                NetworkServerProcessing.SendMessageToClient(
                ServerToClientSignifiers.AccountErrorMessage.ToString()
                + ",register attempt failed, account already exists, please try again.",
                clientID,
                TransportPipeline.ReliableAndInOrder);

                Debug.Log("account exists");
            }
            if (!AccountAlreadyExists(registerUsernameInput))
            {
                RegisterClientAsNewAccount(registerUsernameInput, registerPasswordInput);
            }
        }

        #endregion

        else if (signifier == ClientToServerSignifiers.CheckIfRoomAvailable)
        {
            string clientRoomName = clientInstructions[1];

            if (!RoomAlreadyExists(clientRoomName))
            {
                CreateGameRoomForClients(clientID, clientRoomName);
            }
            else if (RoomAlreadyExists(clientRoomName))
            {
                SendPlayerToExistingGameRoom(clientID, clientRoomName);
            }
        }
    }

    #region Login Stuff
    public void LoginClientWithCorrectCredentials(int clientID, string clientUsername)
    {
        Debug.Log("login successful!");

        NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.LoginAttemptSuccessful.ToString(), clientID, TransportPipeline.ReliableAndInOrder);

        Player papoy = new Player(clientID, clientUsername);

        playersInServer.Add(clientID, papoy);
    }

    public void RegisterClientAsNewAccount(string clientUsername, string clientPassword)
    {

        //here, take the clientInstructions and use them to create an account using StreamWriter
        string accountSaveFile = clientUsername + ".txt";
        string accountUsernameAndPassword = conjoinStrings(ClientToServerSignifiers.LoginAccountInfo.ToString(), clientUsername, clientPassword);
        string pathToSaveNewAccount = accountDirectoryPath + Path.DirectorySeparatorChar + accountSaveFile;

        if (!Directory.Exists(accountDirectoryPath))
        {
            Debug.Log("Creating Directory Path for All Accounts. Path is: " + accountDirectoryPath);
            Directory.CreateDirectory(accountDirectoryPath);
        }
        using (StreamWriter writer = new StreamWriter(pathToSaveNewAccount))
        {
            writer.WriteLine(accountUsernameAndPassword);
        }
    }

    public bool AccountAlreadyExists(string accountUsername)
    {
        string accountSaveFile = accountUsername + ".txt";

        foreach (string line in GetAccountNames())
        {
            string accountFileCurrent = Path.GetFileName(line);

            if (accountFileCurrent == accountSaveFile)
            {
                return true;
            }
        }


        return false;
    }

    static private string conjoinStrings(params string[] strings)
    {
        string conjoinedString = "";

        foreach (string s in strings)
        {
            if (conjoinedString != "")
            {
                conjoinedString += sepchar;
            }
            conjoinedString += s;
        }

        return conjoinedString;
    }

    public string[] GetAccountNames()
    {
        if (Directory.Exists(accountDirectoryPath))
        {
            return Directory.GetFiles(accountDirectoryPath).ToArray();
        }
        return null;
    }
    #endregion

    #region Waiting Room Stuff

    public bool RoomAlreadyExists(string roomName)
    {

        if (roomDictionary.ContainsKey(roomName))
        {
            return true;
        }

        return false;
    }

    public void CreateGameRoomForClients(int clientID, string roomName)
    {

        GameRoom newlyCreatedRoom = new GameRoom(roomName, playersInServer[clientID], gameplayBoard);

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                newlyCreatedRoom.gameplayBoard[x, y] = emptyBox;
            }
        }

        roomDictionary.Add(roomName, newlyCreatedRoom);

        Debug.Log(clientID + roomName);

        Debug.Log("Created new room!");
    }

    public void SendPlayerToExistingGameRoom(int clientID, string roomName)
    {
        roomDictionary[roomName].AddPlayer2(playersInServer[clientID]);

        //start game 
        Debug.Log("starting game now.");

    }

    #endregion

    //requirements for a room:
    //player 1
    //player 2
    //game board

}
