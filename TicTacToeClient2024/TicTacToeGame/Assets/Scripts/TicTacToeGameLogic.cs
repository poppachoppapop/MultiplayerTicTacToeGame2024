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

    short emptyBox = 0;
    short player1 = 1;
    short player2 = 2;


    //Account Login GameObjects
    GameObject usernameLoginInputField;
    GameObject passwordLoginInputField;
    GameObject loginButton;
    GameObject registerButton;
    GameObject refreshButton;
    GameObject titleText;
    GameObject subtitleText;

    //different state canvas
    GameObject accountLoginRegisterCanvas;
    GameObject waitingRoomCanvas;
    GameObject ticTacToeBoardCanvas;

    //Waiting Room GameObjects
    GameObject roomNameInputField;
    GameObject waitingRoomBackButton;
    GameObject createRoomButton;
    GameObject memeText;
    GameObject memeImage;
    GameObject waitingForPlayer2Text;


    //TicTacToe GameObjects
    public GameObject listOfPlaySpaces;
    GameObject[,] ticTacToePlaySpaces = new GameObject[3, 3];
    GameObject gameplayBackButton;
    GameObject chatInputField;
    GameObject sendChatButton;
    GameObject waitingForTurnText;
    GameObject winLoseTitle;
    GameObject resetGameButton;

    //game state handlers
    public AccountLoginStateSignifier currentLoginState;
    public WaitingRoomLogicState currentWaitingRoomState;
    public TicTacToeGameState currentTicTacToeGameState;
    public GameStateManager currentGameState;

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
            else if (obj.name == "GameplayBackButton")
            {
                gameplayBackButton = (GameObject)obj;
            }
            else if (obj.name == "ChatInput")
            {
                chatInputField = (GameObject)obj;
            }
            else if (obj.name == "SendChatButton")
            {
                sendChatButton = (GameObject)obj;
            }
            else if (obj.name == "WaitingForTurnText")
            {
                waitingForTurnText = (GameObject)obj;
            }
            else if (obj.name == "WinLoseTitle")
            {
                winLoseTitle = (GameObject)obj;
            }
            else if (obj.name == "ResetGameButton")
            {
                resetGameButton = (GameObject)obj;
            }

            #endregion

            #region setting button play spaces

            if (obj.name == "TopLeftBox")
            {
                ticTacToePlaySpaces[0, 0] = (GameObject)obj;
                obj.GetComponent<Button>().onClick.AddListener(delegate { SendMove(0, 0); });
            }
            else if (obj.name == "TopMiddleBox")
            {
                ticTacToePlaySpaces[0, 1] = (GameObject)obj;
                obj.GetComponent<Button>().onClick.AddListener(delegate { SendMove(0, 1); });
            }
            else if (obj.name == "TopRightBox")
            {
                ticTacToePlaySpaces[0, 2] = (GameObject)obj;
                obj.GetComponent<Button>().onClick.AddListener(delegate { SendMove(0, 2); });
            }
            else if (obj.name == "MiddleLeftBox")
            {
                ticTacToePlaySpaces[1, 0] = (GameObject)obj;
                obj.GetComponent<Button>().onClick.AddListener(delegate { SendMove(1, 0); });
            }
            else if (obj.name == "MiddleBox")
            {
                ticTacToePlaySpaces[1, 1] = (GameObject)obj;
                obj.GetComponent<Button>().onClick.AddListener(delegate { SendMove(1, 1); });
            }
            else if (obj.name == "MiddleRightBox")
            {
                ticTacToePlaySpaces[1, 2] = (GameObject)obj;
                obj.GetComponent<Button>().onClick.AddListener(delegate { SendMove(1, 2); });
            }
            else if (obj.name == "BottomLeftBox")
            {
                ticTacToePlaySpaces[2, 0] = (GameObject)obj;
                obj.GetComponent<Button>().onClick.AddListener(delegate { SendMove(2, 0); });
            }
            else if (obj.name == "BottomMiddleBox")
            {
                ticTacToePlaySpaces[2, 1] = (GameObject)obj;
                obj.GetComponent<Button>().onClick.AddListener(delegate { SendMove(2, 1); });
            }
            else if (obj.name == "BottomRightBox")
            {
                ticTacToePlaySpaces[2, 2] = (GameObject)obj;
                obj.GetComponent<Button>().onClick.AddListener(delegate { SendMove(2, 2); });
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

        //gameplay board buttons
        gameplayBackButton.GetComponent<Button>().onClick.AddListener(GameplayBackButtonPressed);
        sendChatButton.GetComponent<Button>().onClick.AddListener(SendChatMessageToServer);
        resetGameButton.GetComponent<Button>().onClick.AddListener(ResetGameButtonPressed);


        RefreshUI();
        SetActiveGameCanvas();
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

                subtitleText.GetComponent<TextMeshProUGUI>().text = ChangeText("Login to start playing!");
            }
            else if (currentLoginState == AccountLoginStateSignifier.RegisterState)
            {
                usernameLoginInputField.SetActive(true);
                passwordLoginInputField.SetActive(true);
                registerButton.SetActive(true);

                subtitleText.GetComponent<TextMeshProUGUI>().text = ChangeText("Create an account to play!");
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
            else if (currentWaitingRoomState == WaitingRoomLogicState.BasicWaitingRoomState)
            {
                roomNameInputField.SetActive(true);
                waitingRoomBackButton.SetActive(true);
                createRoomButton.SetActive(true);
                memeImage.SetActive(true);
                memeText.SetActive(true);

                waitingForPlayer2Text.SetActive(false);
            }
        }
        else if (currentGameState == GameStateManager.TicTacToeBoardLogic)
        {
            ticTacToeBoardCanvas.SetActive(true);

            if (currentTicTacToeGameState == TicTacToeGameState.WaitingForTurnState)
            {
                winLoseTitle.SetActive(false);
                listOfPlaySpaces.SetActive(true);
                waitingForPlayer2Text.SetActive(false);

                waitingForTurnText.SetActive(true);
                resetGameButton.SetActive(false);
            }
            else if (currentTicTacToeGameState == TicTacToeGameState.CurrentPlayerTurn)
            {
                winLoseTitle.SetActive(false);
                listOfPlaySpaces.SetActive(true);
                waitingForPlayer2Text.SetActive(false);

                waitingForTurnText.SetActive(false);
                resetGameButton.SetActive(false);
            }
            else if (currentTicTacToeGameState == TicTacToeGameState.WinnerState)
            {
                winLoseTitle.SetActive(true);
                winLoseTitle.GetComponent<TextMeshProUGUI>().text = ChangeText("You win!");
                DisablePlayButtons();

                resetGameButton.SetActive(true);
                waitingForTurnText.SetActive(false);
            }
            else if (currentTicTacToeGameState == TicTacToeGameState.LoserState)
            {
                winLoseTitle.SetActive(true);
                winLoseTitle.GetComponent<TextMeshProUGUI>().text = ChangeText("You Lose!");
                DisablePlayButtons();

                resetGameButton.SetActive(true);
                waitingForTurnText.SetActive(false);
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

    public string GetUsernameFromInput()
    {
        string usernameInput = usernameLoginInputField.GetComponentsInChildren<TextMeshProUGUI>()[1].text;

        return usernameInput;
    }
    public string GetPasswordFromInput()
    {
        string passwordInput = passwordLoginInputField.GetComponentsInChildren<TextMeshProUGUI>()[1].text;

        return passwordInput;
    }

    public string ChangeText(string text)
    {
        return text;
    }

    #endregion

    #region Waiting Room Logics
    public string GetRoomNameFromInput()
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
        currentLoginState = AccountLoginStateSignifier.LoginState;
        SetActiveGameCanvas();
    }

    #endregion

    #region TicTacToeGame Logics

    public void SendChatMessageToServer()
    {
        string chatMessageToSend = chatInputField.GetComponentsInChildren<TextMeshProUGUI>()[1].text;

        string chatMessageWithSignifier = conjoinStrings(ClientToServerSignifiers.SendMessage.ToString(), chatMessageToSend);

        NetworkClientProcessing.SendMessageToServer(chatMessageWithSignifier, TransportPipeline.ReliableAndInOrder);
    }

    public void EnablePlayButtons()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                string boxTicks = ticTacToePlaySpaces[x, y].GetComponentInChildren<TextMeshProUGUI>().text;

                if (boxTicks != "X" && boxTicks != "O")
                {
                    ticTacToePlaySpaces[x, y].GetComponent<Button>().enabled = true;
                }
            }
        }
    }

    public void DisablePlayButtons()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                ticTacToePlaySpaces[x, y].GetComponent<Button>().enabled = false;
            }
        }
    }

    public void SendMove(short x, short y)
    {
        string moveToSend = conjoinStrings(ClientToServerSignifiers.SendMove.ToString(), x.ToString(), y.ToString());
        NetworkClientProcessing.SendMessageToServer(moveToSend, TransportPipeline.ReliableAndInOrder);
        DisablePlayButtons();
    }

    public void UpdateGameBoard(short x, short y, short playerMove)
    {
        if (playerMove.ToString() == player1.ToString())
        {
            ticTacToePlaySpaces[x, y].GetComponentInChildren<TextMeshProUGUI>().text = "X";
        }
        else if (playerMove.ToString() == player2.ToString())
        {
            ticTacToePlaySpaces[x, y].GetComponentInChildren<TextMeshProUGUI>().text = "O";
        }
        else if (playerMove.ToString() == emptyBox.ToString())
        {
            for (int column = 0; column < 3; column++)
            {
                for (int row = 0; row < 3; row++)
                {
                    ticTacToePlaySpaces[column, row].GetComponentInChildren<TextMeshProUGUI>().text = null;
                }
            }
        }
    }

    public void GameplayBackButtonPressed()
    {
        currentGameState = GameStateManager.WaitingRoomLogic;
        currentWaitingRoomState = WaitingRoomLogicState.BasicWaitingRoomState;
        SetActiveGameCanvas();
        RefreshUI();

        NetworkClientProcessing.SendMessageToServer(ClientToServerSignifiers.ExitGame.ToString(), TransportPipeline.ReliableAndInOrder);
    }

    public void ResetGameButtonPressed()
    {
        NetworkClientProcessing.SendMessageToServer(ClientToServerSignifiers.ResetGame.ToString(), TransportPipeline.ReliableAndInOrder);
    }

    #endregion


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
                DisablePlayButtons();
                RefreshUI();
            }
        }

        #endregion

        #region Tic Tac Toe Messages From Server


        else if (currentGameState == GameStateManager.TicTacToeBoardLogic)
        {
            if (signifier == ServerToClientSignifiers.CurrentPlayerTurn)
            {
                EnablePlayButtons();
                currentTicTacToeGameState = TicTacToeGameState.CurrentPlayerTurn;
                RefreshUI();
            }
            else if (signifier == ServerToClientSignifiers.NotCurrentTurn)
            {
                DisablePlayButtons();
                currentTicTacToeGameState = TicTacToeGameState.WaitingForTurnState;
                RefreshUI();
            }
            //updating client side boards according to servers current state
            else if (signifier == ServerToClientSignifiers.UpdateClientBoards)
            {
                UpdateGameBoard(short.Parse(clientInstructions[1]), short.Parse(clientInstructions[2]), short.Parse(clientInstructions[3]));
            }
            else if (signifier == ServerToClientSignifiers.ResetGame)
            {
                UpdateGameBoard(emptyBox, emptyBox, emptyBox);
            }
            else if (signifier == ServerToClientSignifiers.SendMessageToOtherPlayer)
            {

            }
            else if (signifier == ServerToClientSignifiers.WinnerSignifier)
            {
                currentTicTacToeGameState = TicTacToeGameState.WinnerState;
                RefreshUI();
            }
            else if (signifier == ServerToClientSignifiers.LoserSignifier)
            {
                currentTicTacToeGameState = TicTacToeGameState.LoserState;
                RefreshUI();
            }
            else if (signifier == ServerToClientSignifiers.TieGameSignifier)
            {
                currentTicTacToeGameState = TicTacToeGameState.TieGameState;
                RefreshUI();
            }
        }


        #endregion
    }

    private string conjoinStrings(params string[] strings)
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
}

