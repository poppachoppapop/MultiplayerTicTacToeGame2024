
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class AccountInformation
{
    public string accountUsername;

    public string accountPassword;
}
public class GameLogic : MonoBehaviour
{
    void Start()
    {
        NetworkClientProcessing.SetGameLogic(this);
        //NetworkClientProcessing.SetGameLogic(AccountLoginLogic);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            NetworkClientProcessing.SendMessageToServer("2,Hello client's world, sincerely your network server",  TransportPipeline.ReliableAndInOrder);
    }

    public virtual void ProcessMessageFromServer(string[] clientInstructions, TransportPipeline pipeline){}

    //public virtual void StateManager(){}
}


public enum AccountLoginStateSignifier
{
    LoginState,
    RegisterState
}