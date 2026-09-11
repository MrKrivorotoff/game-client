using System;
using System.Collections;
using System.IO;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.Networking;

public sealed class RequestsSender : MonoBehaviour
{
    [field: SerializeReference] public NetworkConfig NetworkConfig { get; set; }

    private byte[] _expectedCertBytes;
    private string _getUserCurrenciesUrl;
    private string _postLoginUrl;

    public void Start()
    {
        var certPath = Path.Combine(Application.streamingAssetsPath, "game-backend-cert.crt");
        _expectedCertBytes = File.ReadAllBytes(certPath);
        _getUserCurrenciesUrl = NetworkConfig.serverBaseUrl + "/inventory/user_currencies";
        _postLoginUrl = NetworkConfig.serverBaseUrl + "/auth/login";
    }

    public IEnumerator SendInventoryRequest(Action<string> onComplete)
    {
        using var request = UnityWebRequest.Get(_getUserCurrenciesUrl);
        request.certificateHandler = new PinnedCertificateHandler(_expectedCertBytes);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogErrorFormat("Request failed: responseCode={0} error={1}", request.responseCode, request.error);
            yield break;
        }

        onComplete(request.downloadHandler.text);
    }

    public IEnumerator SendLoginRequest(string username, string password, Action<string> onComplete)
    {
        var requestMessage = new LoginRequest { Username = username, Password = password };
        using var request = new UnityWebRequest(_postLoginUrl, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(requestMessage.ToByteArray());
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new PinnedCertificateHandler(_expectedCertBytes);
        request.SetRequestHeader("Content-Type", "application/x-protobuf");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogErrorFormat("Request failed: responseCode={0} error={1}", request.responseCode, request.error);
            yield break;
        }

        var responseMessage = LoginResponse.Parser.ParseFrom(request.downloadHandler.data);
        onComplete(responseMessage.Token);
    }
}