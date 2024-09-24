using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    private IApiClient apiClient;


    void Start()
    {
        apiClient = new ApiClient();

        // Get Data
        //StartCoroutine(apiClient.Get(Constant.LEADERBOARD, OnGetSuccess, OnError));
        //StartCoroutine(apiClient.Get(Constant.FIRSTTHREE, OnGetSuccess, OnError));

        // Post Data
       // string postData = "{\"cunique_id\": \"cfa60923-1a43-4d48-ae87-e03\"}";
       // StartCoroutine(apiClient.Post(Constant.NEARESTRANK, postData, OnPostSuccess, OnError)); 
        
        //string postData = "{\"unique_id\": \"cfa60923-1a43-4d48-ae87-e5\",\"name\": \"sourav\",\"email\": \"souravsatpati18@gmail.com\",\"apps_data\": \"Ballon Merge\"}";
       // StartCoroutine(apiClient.Post(Constant.SIGNIN, postData, OnPostSuccess, OnError));
        
        string postData = "{\"unique_id\": \"cfa60923-1a43-4d48-ae87-e5\",\"otp\": \"347803\"}";
        StartCoroutine(apiClient.Post(Constant.OTPVERIFY, postData, OnPostSuccess, OnError));
    }

    private void OnGetSuccess(string data)
    {
        LeaderboardResponse leaderboardData = JsonUtility.FromJson<LeaderboardResponse>(data);
        Debug.Log("GET Response: " + leaderboardData.message);

        // Access leaderboard data
        foreach (var entry in leaderboardData.leaderboard)
        {
            Debug.Log($"Name: {entry.name}, Rank: {entry.rank}, Points: {entry.points}");
        }
    }

    private void OnPostSuccess(string data)
    {
        Debug.Log("GET Response: " + data);
    }

    private void OnError(string error)
    {
        Debug.LogError("Error: " + error);
    }
}
