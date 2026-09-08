using System;
using System.Collections;
using System.IO;
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
        _postLoginUrl = NetworkConfig.serverBaseUrl + "/login";
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

    public IEnumerator SendLoginRequest(Action<string> onComplete)
    {
        using var request = UnityWebRequest.Post(_postLoginUrl, "", "");
        request.certificateHandler = new PinnedCertificateHandler(_expectedCertBytes);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogErrorFormat("Request failed: responseCode={0} error={1}", request.responseCode, request.error);
            yield break;
        }

        onComplete(request.downloadHandler.text);
    }
}