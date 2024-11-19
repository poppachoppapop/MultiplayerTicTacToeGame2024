using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountLoginLogic : MonoBehaviour
{
     const char sepchar = ',';

    static GameObject usernameLoginInputField;
    static GameObject passwordLoginInputField;
    static GameObject loginButton;
    static GameObject registerButton;
    static GameObject refreshButton;

    public static AccountLoginStateSignifier currentState;

    // Start is called before the first frame update
    void Start()
    {
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

        if (currentState == AccountLoginStateSignifier.LoginState)
        {
            usernameLoginInputField.SetActive(true);
            passwordLoginInputField.SetActive(true);
            loginButton.SetActive(true);
        }
        else if (currentState == AccountLoginStateSignifier.RegisterState)
        {
            usernameLoginInputField.SetActive(true);
            passwordLoginInputField.SetActive(true);
            registerButton.SetActive(true);
        }
    }



    public void LoginButtonPressed()
    {
        // string playerUsernameInput = conjoinStrings(ClientToServerSignifiers.UsernameInput.ToString(), GetUsernameFromInput());
        // string playerPasswordInput = conjoinStrings(ClientToServerSignifiers.PasswordInput.ToString(), GetPasswordFromInput());

        // if(GetUsernameFromInput() != string.Empty && GetPasswordFromInput() != string.Empty)
        // {
        //     Debug.Log("Login from client attempted!");
        //     NetworkClientProcessing.SendMessageToServer(playerUsernameInput, TransportPipeline.ReliableAndInOrder);
        //     NetworkClientProcessing.SendMessageToServer(playerPasswordInput, TransportPipeline.ReliableAndInOrder);
        // }
        // else
        // {
        //     Debug.Log("Login Attempt Failed!");
        //     NetworkClientProcessing.SendMessageToServer("3, this is a failed login attempt.", TransportPipeline.ReliableAndInOrder);
        // }
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
        //RefreshUI();

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
        //string asdf = usernameLoginInputField.GetComponentsInChildren<TextMeshPro>()[0].text;   
        
        string asdf = usernameLoginInputField.GetComponentsInChildren<TextMeshProUGUI>()[1].text;   
        
        if (asdf == null)
        {
            Debug.Log("ya mama");
        }

        Debug.Log(asdf);

        return asdf;
    }
    public static string GetPasswordFromInput()
    {
        string asdf = passwordLoginInputField.GetComponentsInChildren<TextMeshProUGUI>()[1].text;

        if(asdf == null)
        {
            Debug.Log("You fool. You utter incompetent buffoon");
        }

        Debug.Log(asdf);

        return asdf;
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
}
