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

            if(AccountAlreadyExists(clientUsernameInput))
            {
                LoginClientWithCorrectCredentials(clientID);
            }

            else if(!AccountAlreadyExists(clientUsernameInput))
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

            if(AccountAlreadyExists(registerUsernameInput))
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
    }

    public void LoginClientWithCorrectCredentials(int clientID)
    {
        Debug.Log("login successful!");

        NetworkServerProcessing.SendMessageToClient(ServerToClientSignifiers.LoginAttemptSuccessful.ToString(), clientID, TransportPipeline.ReliableAndInOrder);
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
}
