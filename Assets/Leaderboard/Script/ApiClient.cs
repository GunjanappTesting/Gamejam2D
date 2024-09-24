using System;
using System.Collections;
using UnityEngine.Networking;

public class ApiClient : IApiClient
{
    public IEnumerator Get(string url, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }

    public IEnumerator Post(string url, string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(url, jsonData))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
}

public interface IApiClient
{
    IEnumerator Get(string url, Action<string> onSuccess, Action<string> onError);
    IEnumerator Post(string url, string jsonData, Action<string> onSuccess, Action<string> onError);
}
