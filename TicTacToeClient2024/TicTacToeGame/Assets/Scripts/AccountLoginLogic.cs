using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountLoginLogic : GameLogic
{
     const char sepchar = ',';

    static GameObject usernameLoginInputField;
    static GameObject passwordLoginInputField;
    static GameObject loginButton;
    static GameObject registerButton;
    static GameObject refreshButton;
    static GameObject titleText;
    static GameObject subtitleText;

    public static AccountLoginStateSignifier currentState;

    // Start is called before the first frame update
    void Start()
    {   
        //NetworkClientProcessing.SetGameLogic(this);

        currentState = AccountLoginStateSignifier.RegisterState;

        Object[] GameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (Object obj in GameObjects)
        {
            
            if (obj.name == "UsernameLoginInput")
            {
                usernameLoginInputField = (GameObject)obj;
            }
            else if(obj.name == "PasswordLoginInput")
            {
                passwordLoginInputField = (GameObject)obj;
            }
            else if(obj.name == "LoginButton")
            {
                loginButton = (GameObject)obj;
            }
            else if(obj.name == "RegisterButton")
            {
                registerButton = (GameObject)obj;
            }
            else if(obj.name == "RefreshButton")
            {
                refreshButton = (GameObject)obj;
            }
            else if(obj.name == "TitleText")
            {
                titleText = (GameObject)obj;
            }
            else if(obj.name == "SubtitleText")
            {
                subtitleText = (GameObject)obj;
            }
        }

        loginButton.GetComponent<Button>().onClick.AddListener(LoginButtonPressed);
        registerButton.GetComponent<Button>().onClick.AddListener(RegisterButtonPressed);
        refreshButton.GetComponent<Button>().onClick.AddListener(RefreshButtonPressed);

        RefreshUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RefreshUI()
    {
        usernameLoginInputField.SetActive(false);
        passwordLoginInputField.SetActive(false);
        loginButton.SetActive(false);
        registerButton.SetActive(false);
        refreshButton.SetActive(true);
        titleText.SetActive(true);
        subtitleText.SetActive(true);

        if (currentState == AccountLoginStateSignifier.LoginState)
        {
            usernameLoginInputField.SetActive(true);
            passwordLoginInputField.SetActive(true);
            loginButton.SetActive(true);

            subtitleText.GetComponent<TextMeshProUGUI>().text = ChangeSubtitleText("Login to start playing!");
        }
        else if (currentState == AccountLoginStateSignifier.RegisterState)
        {
            usernameLoginInputField.SetActive(true);
            passwordLoginInputField.SetActive(true);
            registerButton.SetActive(true);

            subtitleText.GetComponent<TextMeshProUGUI>().text = ChangeSubtitleText("Create an account to play!");
        }
    }



    public void LoginButtonPressed()
    {
        Debug.Log("You have pressed me");

        string playerUsernameInput = GetUsernameFromInput();
        string playerPasswordInput = GetPasswordFromInput();
        string ClientAccountLoginInfo = conjoinStrings(ClientToServerSignifiers.LoginAccountInfo.ToString(), playerUsernameInput, playerPasswordInput);

        if(GetUsernameFromInput() != string.Empty && GetPasswordFromInput() != string.Empty)
        {
            Debug.Log("Login from client attempted!");
            NetworkClientProcessing.SendMessageToServer(ClientAccountLoginInfo, TransportPipeline.ReliableAndInOrder);
        }
        else
        {
            Debug.Log("Login Attempt Failed!");
        }
    }

    public void RegisterButtonPressed()
    {
        string playerUsernameInput = GetUsernameFromInput();
        string playerPasswordInput = GetPasswordFromInput();
        string ClientAccountRegisterInfo = conjoinStrings(ClientToServerSignifiers.RegisterAccountInfo.ToString(), playerUsernameInput, playerPasswordInput);

        if (GetUsernameFromInput() != string.Empty && GetPasswordFromInput() != string.Empty)
        {
            Debug.Log("Login from client attempted!");
            NetworkClientProcessing.SendMessageToServer(ClientAccountRegisterInfo, TransportPipeline.ReliableAndInOrder);
        }
        else
        {
            Debug.Log("Login Attempt Failed!");
            NetworkClientProcessing.SendMessageToServer("3, this is a failed login attempt.", TransportPipeline.ReliableAndInOrder);
        }
    }

    public void RefreshButtonPressed()
    {
        if(currentState == AccountLoginStateSignifier.LoginState)
        {
            currentState = AccountLoginStateSignifier.RegisterState;
            Debug.Log("going into register state");
            RefreshUI();
        }
        else if(currentState == AccountLoginStateSignifier.RegisterState)
        {
            currentState = AccountLoginStateSignifier.LoginState;
            Debug.Log("going into login state");
            RefreshUI();
        }   
    }



    public static string GetUsernameFromInput()
    {   
        string asdf = usernameLoginInputField.GetComponentsInChildren<TextMeshProUGUI>()[1].text;   

        return asdf;
    }
    public static string GetPasswordFromInput()
    {
        string asdf = passwordLoginInputField.GetComponentsInChildren<TextMeshProUGUI>()[1].text;

        return asdf;
    }

    public static string ChangeSubtitleText(string text)
    {   
        return text;
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

    public void LoginSuccessful()
    {

    }

    public override void ProcessMessageFromServer(string[] clientInstructions, TransportPipeline pipeline)
    {
        int signifier = int.Parse(clientInstructions[0].ToString());

        //send player to waiting room state
        if(signifier == ServerToClientSignifiers.LoginAttemptSuccessful)
        {
            
        }
        //show UI message to player and prompt them to try again
        else if(signifier == ServerToClientSignifiers.LoginAttemptFailed)
        {
            //RefreshUI();
        }
        //tell player that a new account has been created and send them to the login screen
        else if(signifier == ServerToClientSignifiers.RegisterAccountSuccessful)
        {
            currentState = AccountLoginStateSignifier.LoginState;
        }
        //refresh the register input fields and show UI message to player prompting them to retry
        else if(signifier == ServerToClientSignifiers.RegisterAccountFailed)
        {

        }
    }
}

