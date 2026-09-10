using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(RequestsSender))]
[RequireComponent(typeof(PanelRenderer))]
public sealed class RequestsUIController : MonoBehaviour
{
    private RequestsSender _requestsSender;
    private PanelRenderer _panelRenderer;
    private Label _responseBody;
    private Button _sendInventoryRequestButton;
    private Button _sendLoginRequestButton;
    private int _uiVersion;

    public void Awake()
    {
        _requestsSender = GetComponent<RequestsSender>();
        _panelRenderer = GetComponent<PanelRenderer>();
    }

    public void OnEnable()
    {
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
        StartCoroutine(_requestsSender.SendInventoryRequest(SetResponseBodyText));
    }

    private void OnClickedSendLoginRequest()
    {
        StartCoroutine(_requestsSender.SendLoginRequest("placeholder_username", "placeholder_password", SetResponseBodyText));
    }

    private void SetResponseBodyText(string text)
    {
        var responseBody = _responseBody;
        if (responseBody != null)
            responseBody.text = text;
    }
}