using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;



public class AccountLoginLogic : GameLogic
{
    const char sepchar = ',';

    string accountDirectoryPath;

    //string[] listOfAccounts;

    public void Awake()
    {
        accountDirectoryPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Accounts";

        // if (Directory.Exists(accountDirectoryPath))
        // {   
        //     string[]listOfAccounts = Directory.GetFiles(accountDirectoryPath).ToArray();
        // }
        // else
        // {
        //     return;
        // }
    }

    public void Start()
    {
        NetworkServerProcessing.SetGameLogic(this);
    }
    //function for received login attempt
    public void CheckIfAccountInformationAlreadyExistsForLogin(string clientMessage, int currentConnectionID)
    {
        string[] csv = clientMessage.Split(sepchar);
        // int signifier = 
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
        }

        else if (signifier == ClientToServerSignifiers.RegisterAccountInfo)
        {
            string registerUsernameInput = clientInstructions[1];
            string registerPasswordInput = clientInstructions[2];

            if (!AccountAlreadyExists(registerUsernameInput))
            {
                RegisterClientAsNewAccount(registerUsernameInput, registerPasswordInput);
            }
            else if(AccountAlreadyExists(registerUsernameInput))
            {
                NetworkServerProcessing.SendMessageToClient(
                ServerToClientSignifiers.RegisterAccountFailed.ToString()
                + ",register attempt failed, account already exists, please try again.",
                clientID,
                TransportPipeline.ReliableAndInOrder);

                Debug.Log("account exists");
            }
        }
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
