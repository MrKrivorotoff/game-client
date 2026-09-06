using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public sealed class RequestsController : MonoBehaviour
{
    [field: SerializeReference] public NetworkConfig NetworkConfig { get; set; }

    private byte[] _expectedCertBytes;
    private PanelRenderer _panelRenderer;
    private Label _responseBody;
    private Button _sendInventoryRequestButton;
    private Button _sendLoginRequestButton;
    private int _uiVersion;

    public void Start()
    {
        var certPath = Path.Combine(Application.streamingAssetsPath, "game-backend-cert.crt");
        _expectedCertBytes = File.ReadAllBytes(certPath);
    }

    public void OnEnable()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    public void OnDisable()
    {
        _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        UnbindUI();
    }

    private void OnUIReload(PanelRenderer panelRenderer,
        VisualElement rootElement,
        int version)
    {
        if (_uiVersion == version)
            return;
        _uiVersion = version;
        UnbindUI();
        BindUI(rootElement);
    }

    private void UnbindUI()
    {
        var sendInventoryRequestButton = _sendInventoryRequestButton;
        if (sendInventoryRequestButton != null)
        {
            sendInventoryRequestButton.clicked -= OnClickedSendInventoryRequest;
            _sendInventoryRequestButton = null;
        }

        var sendLoginRequestButton = _sendLoginRequestButton;
        if (sendLoginRequestButton != null)
        {
            sendLoginRequestButton.clicked -= OnClickedSendLoginRequest;
            _sendLoginRequestButton = null;
        }

        _responseBody = null;
    }

    private void BindUI(VisualElement rootElement)
    {
        var sendInventoryRequestButton = rootElement.Q<Button>("SendInventoryRequest");
        var sendLoginRequestButton = rootElement.Q<Button>("SendLoginRequest");
        sendInventoryRequestButton.clicked += OnClickedSendInventoryRequest;
        sendLoginRequestButton.clicked += OnClickedSendLoginRequest;
        _sendInventoryRequestButton = sendInventoryRequestButton;
        _sendLoginRequestButton = sendLoginRequestButton;
        _responseBody = rootElement.Q<Label>("ResponseBody");
    }

    private void OnClickedSendInventoryRequest()
    {
        StartCoroutine(SendInventoryRequest());
    }

    private IEnumerator SendInventoryRequest()
    {
        using var request = UnityWebRequest.Get(NetworkConfig.serverBaseUrl + "/inventory/user_currencies");
        request.certificateHandler = new PinnedCertificateHandler(_expectedCertBytes);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogErrorFormat("Request failed: responseCode={0} error={1}", request.responseCode, request.error);
            yield break;
        }

        SetResponseBodyText(request.downloadHandler.text);
    }

    private void OnClickedSendLoginRequest()
    {
        StartCoroutine(SendLoginRequest());
    }

    private IEnumerator SendLoginRequest()
    {
        using var request = UnityWebRequest.Post(NetworkConfig.serverBaseUrl + "/login", "", "");
        request.certificateHandler = new PinnedCertificateHandler(_expectedCertBytes);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogErrorFormat("Request failed: responseCode={0} error={1}", request.responseCode, request.error);
            yield break;
        }

        SetResponseBodyText(request.downloadHandler.text);
    }

    private void SetResponseBodyText(string text)
    {
        var responseBody = _responseBody;
        if (responseBody != null)
            responseBody.text = text;
    }
}