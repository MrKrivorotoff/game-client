using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public sealed class RequestsController : MonoBehaviour
{
    private PanelRenderer _panelRenderer;
    private Button _sendInventoryRequestButton;
    private Button _sendLoginRequestButton;
    private Label _responseBody;
    private int _uiVersion;

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
        using var request = UnityWebRequest.Get("http://localhost:8081/inventory/user_currencies");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            yield break;
        SetResponseBodyText(request.downloadHandler.text);
    }

    private void OnClickedSendLoginRequest()
    {
        StartCoroutine(SendLoginRequest());
    }

    private IEnumerator SendLoginRequest()
    {
        using var request = UnityWebRequest.Post("http://localhost:8082/login", "", "");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            yield break;
        SetResponseBodyText(request.downloadHandler.text);
    }

    private void SetResponseBodyText(string text)
    {
        var responseBody = _responseBody;
        if (responseBody != null)
            responseBody.text = text;
    }
}