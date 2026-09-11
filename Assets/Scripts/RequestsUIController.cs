using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(RequestsSender))]
[RequireComponent(typeof(PanelRenderer))]
public sealed class RequestsUIController : MonoBehaviour
{
    private RequestsSender _requestsSender;
    private PanelRenderer _panelRenderer;
    private Label _responseBodyLabel;
    private TextField _usernameTextField;
    private TextField _passwordTextField;
    private Button _sendInventoryRequestButton;
    private Button _sendLoginRequestButton;
    private Button _sendRegisterRequestButton;
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
        
        var sendRegisterRequestButton = _sendRegisterRequestButton;
        if (sendRegisterRequestButton != null)
        {
            sendRegisterRequestButton.clicked -= OnClickedSendLoginRequest;
            _sendRegisterRequestButton = null;
        }

        _responseBodyLabel = null;
        _usernameTextField = null;
        _passwordTextField = null;
    }

    private void BindUI(VisualElement rootElement)
    {
        var sendInventoryRequestButton = rootElement.Q<Button>("SendInventoryRequest");
        var sendLoginRequestButton = rootElement.Q<Button>("SendLoginRequest");
        var sendRegisterRequestButton = rootElement.Q<Button>("SendRegistrationRequest");
        sendInventoryRequestButton.clicked += OnClickedSendInventoryRequest;
        sendLoginRequestButton.clicked += OnClickedSendLoginRequest;
        sendRegisterRequestButton.clicked += OnClickedSendRegisterRequest;
        _sendInventoryRequestButton = sendInventoryRequestButton;
        _sendLoginRequestButton = sendLoginRequestButton;
        _sendRegisterRequestButton = sendRegisterRequestButton;
        _responseBodyLabel = rootElement.Q<Label>("ResponseBody");
        _usernameTextField = rootElement.Q<TextField>("Username");
        _passwordTextField = rootElement.Q<TextField>("Password");
    }

    private void OnClickedSendInventoryRequest()
    {
        StartCoroutine(_requestsSender.SendInventoryRequest(SetResponseBodyText));
    }

    private void OnClickedSendLoginRequest()
    {
        StartCoroutine(_requestsSender.SendLoginRequest(_usernameTextField.text, _passwordTextField.text, SetResponseBodyText));
    }
    
    private void OnClickedSendRegisterRequest()
    {
        StartCoroutine(_requestsSender.SendRegisterRequest(_usernameTextField.text, _passwordTextField.text, () => Debug.Log("Registration Done")));
    }

    private void SetResponseBodyText(string text)
    {
        var responseBody = _responseBodyLabel;
        if (responseBody != null)
            responseBody.text = text;
    }
}