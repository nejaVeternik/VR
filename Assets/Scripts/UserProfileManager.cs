using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UserProfileManager : MonoBehaviour
{
    [System.Serializable]
    public class UserProfile
    {
        public string username;
        public string email;
        public int score;
    }

    public void CreateProfile(string username, string email)
    {
        UserProfile userProfile = new UserProfile();
        userProfile.username = username;
        userProfile.email = email;
        userProfile.score = 0;

        string jsonData = JsonUtility.ToJson(userProfile);
        StartCoroutine(SendCreateProfileRequest(jsonData));
    }

    private IEnumerator SendCreateProfileRequest(string jsonData)
    {
        UnityWebRequest request = new UnityWebRequest("http://localhost:3000/createProfile", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Profile created: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error: " + request.error);
        }
    }

    public void GetProfile(string username)
    {
        StartCoroutine(SendGetProfileRequest(username));
    }

    private IEnumerator SendGetProfileRequest(string username)
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/getProfile/" + username);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Profile retrieved: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error: " + request.error);
        }
    }
}
