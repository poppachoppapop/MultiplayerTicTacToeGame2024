using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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

    //different state canvas
    static GameObject accountLoginRegisterCanvas;
    static GameObject waitingRoomCanvas;
    static GameObject ticTacToeBoardCanvas;

    //Waiting Room GameObjects
    static GameObject roomNameInputField;
    static GameObject waitingRoomBackButton;
    static GameObject createRoomButton;
    static GameObject memeText;
    static GameObject memeImage;
    static GameObject waitingForPlayer2Text;


    //TicTacToe GameObjects

    static GameObject listOfPlaySpaces;
    static Button[] ticTacToePlaySpaces;

    public static AccountLoginStateSignifier currentLoginState;
    public static WaitingRoomLogicState currentWaitingRoomState;
    public static TicTacToeGameState currentTicTacToeGameState;
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

            #region account login game objects
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

            #region waiting room game objects

            else if (obj.name == "RoomNameCreator")
            {
                roomNameInputField = (GameObject)obj;
            }
            else if (obj.name == "WaitingRoomBackButton")
            {
                waitingRoomBackButton = (GameObject)obj;
            }
            else if (obj.name == "CreateRoomButton")
            {
                createRoomButton = (GameObject)obj;
            }
            else if (obj.name == "MemeImage")
            {
                memeImage = (GameObject)obj;
            }
            else if (obj.name == "MemeText")
            {
                memeText = (GameObject)obj;
            }
            else if (obj.name == "WaitingForPlayer2Text")
            {
                waitingForPlayer2Text = (GameObject)obj;
            }
            #endregion

            #region tic tac toe board objects

            else if (obj.name == "ListOfPlaySpaces")
            {
                listOfPlaySpaces = (GameObject)obj;
            }


            #endregion
        }

        //login canvas buttons
        loginButton.GetComponent<Button>().onClick.AddListener(LoginButtonPressed);
        registerButton.GetComponent<Button>().onClick.AddListener(RegisterButtonPressed);
        refreshButton.GetComponent<Button>().onClick.AddListener(RefreshButtonPressed);

        //waiting room buttons
        createRoomButton.GetComponent<Button>().onClick.AddListener(CreateRoomButtonPressed);
        waitingRoomBackButton.GetComponent<Button>().onClick.AddListener(WaitingRoomBackButtonPressed);

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
            if (currentWaitingRoomState == WaitingRoomLogicState.WaitingForNewPlayerState)
            {
                waitingForPlayer2Text.SetActive(true);

                roomNameInputField.SetActive(false);
                waitingRoomBackButton.SetActive(false);
                createRoomButton.SetActive(false);
                memeImage.SetActive(false);
                memeText.SetActive(false);
            }
        }
        else if (currentGameState == GameStateManager.TicTacToeBoardLogic)
        {
            ticTacToeBoardCanvas.SetActive(true);

            if (currentTicTacToeGameState == TicTacToeGameState.WaitingForTurnState)
            {
                listOfPlaySpaces.SetActive(true);
                waitingForPlayer2Text.SetActive(false);
            }
            else if (currentTicTacToeGameState == TicTacToeGameState.SelectingSquareTurn)
            {
                listOfPlaySpaces.SetActive(true);
                waitingForPlayer2Text.SetActive(false);
            }
        }

    }


    #region Login / Register Logics
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

    #endregion
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

    #region Waiting Room Logics
    public static string GetRoomNameFromInput()
    {
        string roomNameInput = roomNameInputField.GetComponentsInChildren<TextMeshProUGUI>()[1].text;

        return roomNameInput;
    }

    public void CreateRoomButtonPressed()
    {
        string clientRoomInput = GetRoomNameFromInput();
        string clientRoomNameInfo = conjoinStrings(ClientToServerSignifiers.CheckIfRoomAvailable.ToString(), clientRoomInput);

        if (GetRoomNameFromInput() != string.Empty)
        {
            Debug.Log("attempting to create room with name: " + GetRoomNameFromInput());
            NetworkClientProcessing.SendMessageToServer(clientRoomNameInfo, TransportPipeline.ReliableAndInOrder);
        }
    }

    public void WaitingRoomBackButtonPressed()
    {
        currentGameState = GameStateManager.AccountLoginLogic;
        SetActiveGameCanvas();
    }

    #endregion

    #region TicTacToeGame Logics

    #endregion

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

        #region Waiting Room messages from server

        else if (currentGameState == GameStateManager.WaitingRoomLogic)
        {
            if (signifier == ServerToClientSignifiers.waitingForNewPlayer)
            {
                currentWaitingRoomState = WaitingRoomLogicState.WaitingForNewPlayerState;
                SetActiveGameCanvas();
                RefreshUI();
            }
            else if (signifier == ServerToClientSignifiers.StartGame)
            {
                currentGameState = GameStateManager.TicTacToeBoardLogic;
                currentTicTacToeGameState = TicTacToeGameState.WaitingForTurnState;
                SetActiveGameCanvas();
                RefreshUI();
            }
        }

        #endregion

        #region Tic Tac Toe Messages From Server



        #endregion
    }
}

