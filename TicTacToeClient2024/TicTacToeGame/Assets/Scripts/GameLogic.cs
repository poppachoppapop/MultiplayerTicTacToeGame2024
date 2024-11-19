
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            NetworkClientProcessing.SendMessageToServer("2,Hello client's world, sincerely your network server",  TransportPipeline.ReliableAndInOrder);
    }

    public virtual void ProcessMessageFromClient(string[] clientInstructions, int clientID){}
}

public enum AccountLoginStateSignifier
{
    LoginState,
    RegisterState,
    FailedLoginState
}
