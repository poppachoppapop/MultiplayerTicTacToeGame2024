using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToeGameLogic : GameLogic
{
    const char sepchar = ',';

    //Account Login GameObjects
    static GameObject usernameLoginInputField;
    static GameObject passwordLoginInputField;
    static GameObject loginButton;
    static GameObject registerButton;
    static GameObject refreshButton;
    static GameObject titleText;
    static GameObject subtitleText;

    static GameObject accountLoginRegisterCanvas;
    static GameObject waitingRoomCanvas;
    static GameObject ticTacToeBoardCanvas;
    //Waiting Room GameObjects

    //TicTacToe GameObjects


    public static AccountLoginStateSignifier currentLoginState;
    public static GameStateManager currentGameState;

    // Start is called before the first frame update
    void Start()
    {
        NetworkClientProcessing.SetGameLogic(this);

        //currentState = AccountLoginStateSignifier.RegisterState;
        currentGameState = GameStateManager.AccountLoginLogic;

        Object[] GameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (Object obj in GameObjects)
        {
            #region Canvas
            if (obj.name == "LoginCanvas")
            {
                accountLoginRegisterCanvas = (GameObject)obj;
            }
            else if (obj.name == "WaitingRoomCanvas")
            {
                waitingRoomCanvas = (GameObject)obj;
            }
            else if (obj.name == "GameplayBoard")
            {
                ticTacToeBoardCanvas = (GameObject)obj;
            }
            #endregion

            #region account login stuff
            else if (obj.name == "UsernameLoginInput")
            {
                usernameLoginInputField = (GameObject)obj;
            }
            else if (obj.name == "PasswordLoginInput")
            {
                passwordLoginInputField = (GameObject)obj;
            }
            else if (obj.name == "LoginButton")
            {
                loginButton = (GameObject)obj;
            }
            else if (obj.name == "RegisterButton")
            {
                registerButton = (GameObject)obj;
            }
            else if (obj.name == "RefreshButton")
            {
                refreshButton = (GameObject)obj;
            }
            else if (obj.name == "TitleText")
            {
                titleText = (GameObject)obj;
            }
            else if (obj.name == "SubtitleText")
            {
                subtitleText = (GameObject)obj;
            }
            #endregion
        }

        loginButton.GetComponent<Button>().onClick.AddListener(LoginButtonPressed);
        registerButton.GetComponent<Button>().onClick.AddListener(RegisterButtonPressed);
        refreshButton.GetComponent<Button>().onClick.AddListener(RefreshButtonPressed);

        // accountLoginRegisterCanvas = GameObject.Find("LoginCanvas").GetComponent<Canvas>();
        // waitingRoomCanvas = GameObject.Find("WaitingRoomCanvas").GetComponent<Canvas>();
        // ticTacToeBoardCanvas = GameObject.Find("GameplayBoard").GetComponent<Canvas>();

        RefreshUI();
        SetActiveGameCanvas();
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

        accountLoginRegisterCanvas.gameObject.SetActive(false);
        waitingRoomCanvas.gameObject.SetActive(false);
        ticTacToeBoardCanvas.gameObject.SetActive(false);

        if (currentGameState == GameStateManager.AccountLoginLogic)
        {
            accountLoginRegisterCanvas.SetActive(true);

            if (currentLoginState == AccountLoginStateSignifier.LoginState)
            {
                usernameLoginInputField.SetActive(true);
                passwordLoginInputField.SetActive(true);
                loginButton.SetActive(true);

                subtitleText.GetComponent<TextMeshProUGUI>().text = ChangeSubtitleText("Login to start playing!");
            }
            else if (currentLoginState == AccountLoginStateSignifier.RegisterState)
            {
                usernameLoginInputField.SetActive(true);
                passwordLoginInputField.SetActive(true);
                registerButton.SetActive(true);

                subtitleText.GetComponent<TextMeshProUGUI>().text = ChangeSubtitleText("Create an account to play!");
            }
        }
        else if (currentGameState == GameStateManager.WaitingRoomLogic)
        {
            waitingRoomCanvas.SetActive(true);
        }
        else if (currentGameState == GameStateManager.TicTacToeBoardLogic)
        {
            ticTacToeBoardCanvas.SetActive(true);
        }

    }



    public void LoginButtonPressed()
    {
        Debug.Log("You have pressed me");

        string playerUsernameInput = GetUsernameFromInput();
        string playerPasswordInput = GetPasswordFromInput();
        string ClientAccountLoginInfo = conjoinStrings(ClientToServerSignifiers.LoginAccountInfo.ToString(), playerUsernameInput, playerPasswordInput);

        if (GetUsernameFromInput() != string.Empty && GetPasswordFromInput() != string.Empty)
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
            Debug.Log("Register from client attempted!");
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
        if (currentGameState == GameStateManager.AccountLoginLogic)
        {
            if (currentLoginState == AccountLoginStateSignifier.LoginState)
            {
                currentLoginState = AccountLoginStateSignifier.RegisterState;
                Debug.Log("going into register state");
                RefreshUI();
            }
            else if (currentLoginState == AccountLoginStateSignifier.RegisterState)
            {
                currentLoginState = AccountLoginStateSignifier.LoginState;
                Debug.Log("going into login state");
                RefreshUI();
            }
        }

    }

    public static string GetUsernameFromInput()
    {
        string usernameInput = usernameLoginInputField.GetComponentsInChildren<TextMeshProUGUI>()[1].text;

        return usernameInput;
    }
    public static string GetPasswordFromInput()
    {
        string passwordInput = passwordLoginInputField.GetComponentsInChildren<TextMeshProUGUI>()[1].text;

        return passwordInput;
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

    public void SetActiveGameCanvas()
    {
        if (currentGameState == GameStateManager.AccountLoginLogic)
        {
            //set login canvas to be active
            accountLoginRegisterCanvas.SetActive(true);
            waitingRoomCanvas.SetActive(false);
            ticTacToeBoardCanvas.SetActive(false);
        }
        else if (currentGameState == GameStateManager.WaitingRoomLogic)
        {
            //set waiting room canvas to be active
            waitingRoomCanvas.gameObject.SetActive(true);
            accountLoginRegisterCanvas.SetActive(false);
            ticTacToeBoardCanvas.SetActive(false);
        }
        else if (currentGameState == GameStateManager.TicTacToeBoardLogic)
        {
            //set tictactoe canvas to be active
            ticTacToeBoardCanvas.SetActive(true);
            waitingRoomCanvas.SetActive(false);
            accountLoginRegisterCanvas.SetActive(false);
        }
    }

    public void ChangeCurrentGameCanvas()
    {

    }

    public override void ProcessMessageFromServer(string[] clientInstructions, TransportPipeline pipeline)
    {
        int signifier = int.Parse(clientInstructions[0].ToString());

        #region login messages from server
        //send player to waiting room state
        if (currentGameState == GameStateManager.AccountLoginLogic)
        {
            if (currentLoginState == AccountLoginStateSignifier.LoginState)
            {
                if (signifier == ServerToClientSignifiers.LoginAttemptSuccessful)
                {
                    //send to waiting room
                    currentGameState = GameStateManager.WaitingRoomLogic;
                    SetActiveGameCanvas();
                }
                else if (signifier == ServerToClientSignifiers.AccountErrorMessage)
                {
                    Debug.Log("Failed Login Attempt");
                }
            }
            else if (currentLoginState == AccountLoginStateSignifier.RegisterState)
            {
                if (signifier == ServerToClientSignifiers.RegisterAccountSuccessful)
                {
                    //send player to login UI
                    currentLoginState = AccountLoginStateSignifier.LoginState;
                    RefreshUI();
                }
            }
        }
        #endregion
    }
}
