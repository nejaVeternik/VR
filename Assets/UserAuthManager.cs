using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class UserAuthManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button loginButton;

    private void Start()
    {
        registerButton.onClick.AddListener(RegisterUser);
        loginButton.onClick.AddListener(LoginUser);
    }

    private void RegisterUser()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        StartCoroutine(SendRegisterRequest(username, password));
    }

    private IEnumerator SendRegisterRequest(string username, string password)
    {
        UserProfile userProfile = new UserProfile { username = username, password = password };
        string jsonData = JsonUtility.ToJson(userProfile);

        UnityWebRequest request = new UnityWebRequest("http://localhost:3000/createProfile", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("User registered successfully: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error registering user: " + request.error);
        }
    }

    public void LoginUser()
    {
        Debug.Log("AAAAAAAAAA");
        //string username = usernameInputField.text;
        //string password = passwordInputField.text;

        string username = "test";
        string password = "test";

        StartCoroutine(SendLoginRequest(username, password));
    }

    private IEnumerator SendLoginRequest(string username, string password)
    {
        UserProfile userProfile = new UserProfile { username = username, password = password };
        string jsonData = JsonUtility.ToJson(userProfile);

        UnityWebRequest request = new UnityWebRequest("http://localhost:3000/login", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("User logged in successfully: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error logging in user: " + request.error);
        }
    }

    [System.Serializable]
    public class UserProfile
    {
        public string username;
        public string password;
    }
}
