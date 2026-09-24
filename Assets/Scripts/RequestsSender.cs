using System;
using System.Collections;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.Networking;

public sealed class RequestsSender : MonoBehaviour
{
    [field: SerializeReference] public NetworkConfig NetworkConfig { get; set; }
    [field: SerializeReference] public CertificateHandlerProvider CertificateHandlerProvider { get; set; }

    private string _accessToken;

    private string _getUserCurrenciesUrl;
    private string _postLoginBasicUrl;
    private string _postLoginUrl;
    private string _postRegisterUrl;

    public void Start()
    {
        _getUserCurrenciesUrl = NetworkConfig.serverBaseUrl + "/inventory/user_currencies";
        _postLoginUrl = NetworkConfig.serverBaseUrl + "/auth/login";
        _postLoginBasicUrl = NetworkConfig.serverBaseUrl + "/auth/login_basic";
        _postRegisterUrl = NetworkConfig.serverBaseUrl + "/auth/register";
    }

    public IEnumerator SendInventoryRequest(Action<string> onComplete = null, Action onError = null)
    {
        var accessToken = _accessToken;
        if (accessToken is null)
        {
            Debug.LogErrorFormat("Login required");
            onError?.Invoke();
            yield break;
        }

        using var request = UnityWebRequest.Get(_getUserCurrenciesUrl);
        request.certificateHandler = CertificateHandlerProvider.CertificateHandler;
        request.disposeCertificateHandlerOnDispose = CertificateHandlerProvider.DisposeHandlerOnRequestDispose;
        request.SetRequestHeader("Authorization", AuthHeaders.CreateBearerValue(accessToken));
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogErrorFormat("Request failed: responseCode={0} error={1}", request.responseCode, request.error);
            onError?.Invoke();
            yield break;
        }

        var responseData = request.downloadHandler?.data;
        if (responseData is not null)
        {
            var responseMessage = GetUserCurrenciesResponse.Parser.ParseFrom(responseData);
            onComplete?.Invoke(responseMessage.ToString());
        }
        else
        {
            onComplete?.Invoke("Empty");
        }
    }

    public IEnumerator SendLoginRequest(string username, string password, Action<string> onComplete = null, Action onError = null)
    {
        var requestMessage = new LoginRequest { Username = username, Password = password };
        using var request = new UnityWebRequest(_postLoginUrl, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(requestMessage.ToByteArray());
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = CertificateHandlerProvider.CertificateHandler;
        request.disposeCertificateHandlerOnDispose = CertificateHandlerProvider.DisposeHandlerOnRequestDispose;
        request.SetRequestHeader("Content-Type", "application/x-protobuf");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogErrorFormat("Request failed: responseCode={0} error={1}", request.responseCode, request.error);
            onError?.Invoke();
            yield break;
        }

        var responseMessage = LoginResponse.Parser.ParseFrom(request.downloadHandler.data);
        onComplete?.Invoke(_accessToken = responseMessage.AccessToken);
    }

    public IEnumerator SendLoginBasicRequest(string username, string password, Action<string> onComplete = null, Action onError = null)
    {
        using var request = new UnityWebRequest(_postLoginBasicUrl, UnityWebRequest.kHttpVerbPOST);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = CertificateHandlerProvider.CertificateHandler;
        request.disposeCertificateHandlerOnDispose = CertificateHandlerProvider.DisposeHandlerOnRequestDispose;
        request.SetRequestHeader("Authorization", AuthHeaders.CreateBasicValue(username, password));
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogErrorFormat("Request failed: responseCode={0} error={1}", request.responseCode, request.error);
            onError?.Invoke();
            yield break;
        }

        var responseMessage = LoginResponse.Parser.ParseFrom(request.downloadHandler.data);
        onComplete?.Invoke(_accessToken = responseMessage.AccessToken);
    }

    public IEnumerator SendRegisterRequest(string username, string password, Action onComplete = null, Action onError = null)
    {
        var requestMessage = new RegisterRequest { Username = username, Password = password };
        using var request = new UnityWebRequest(_postRegisterUrl, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(requestMessage.ToByteArray());
        request.certificateHandler = CertificateHandlerProvider.CertificateHandler;
        request.disposeCertificateHandlerOnDispose = CertificateHandlerProvider.DisposeHandlerOnRequestDispose;
        request.SetRequestHeader("Content-Type", "application/x-protobuf");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogErrorFormat("Request failed: responseCode={0} error={1}", request.responseCode, request.error);
            onError?.Invoke();
            yield break;
        }

        onComplete?.Invoke();
    }
}