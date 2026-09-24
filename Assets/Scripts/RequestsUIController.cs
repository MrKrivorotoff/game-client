using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(RequestsSender))]
[RequireComponent(typeof(PanelRenderer))]
public sealed class RequestsUIController : MonoBehaviour
{
    private const string PlayerNamePrefKey = "PlayerName";

    private RequestsSender _requestsSender;
    private PanelRenderer _panelRenderer;
    
    private Label _responseBodyLabel;
    private TextField _usernameTextField;
    private TextField _passwordTextField;
    private Button _sendInventoryRequestButton;
    private Button _sendLoginRequestButton;
    private Button _sendLoginBasicRequestButton;
    private Button _sendRegisterRequestButton;

    private int _uiVersion;
    private bool _isDataInitialized;

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
        InitializeUIData();
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

        var sendLoginBasicRequestButton = _sendLoginBasicRequestButton;
        if (sendLoginBasicRequestButton != null)
        {
            sendLoginBasicRequestButton.clicked -= OnClickedSendLoginBasicRequest;
            _sendLoginBasicRequestButton = null;
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
        var sendLoginBasicRequestButton = rootElement.Q<Button>("SendLoginBasicRequest");
        var sendRegisterRequestButton = rootElement.Q<Button>("SendRegistrationRequest");
        sendInventoryRequestButton.clicked += OnClickedSendInventoryRequest;
        sendLoginRequestButton.clicked += OnClickedSendLoginRequest;
        sendLoginBasicRequestButton.clicked += OnClickedSendLoginBasicRequest;
        sendRegisterRequestButton.clicked += OnClickedSendRegisterRequest;
        _sendInventoryRequestButton = sendInventoryRequestButton;
        _sendLoginRequestButton = sendLoginRequestButton;
        _sendLoginBasicRequestButton = sendLoginBasicRequestButton;
        _sendRegisterRequestButton = sendRegisterRequestButton;
        _responseBodyLabel = rootElement.Q<Label>("ResponseBody");
        _usernameTextField = rootElement.Q<TextField>("Username");
        _passwordTextField = rootElement.Q<TextField>("Password");
    }

    private void OnClickedSendInventoryRequest()
    {
        StartCoroutine(_requestsSender.SendInventoryRequest(
            onComplete: text =>
            {
                SetResponseBodyText(text);
                _sendInventoryRequestButton.SetEnabled(true);
            },
            onError: () => _sendInventoryRequestButton.SetEnabled(true))
        );
        _sendInventoryRequestButton.SetEnabled(false);
    }

    private void OnClickedSendLoginRequest()
    {
        StartCoroutine(_requestsSender.SendLoginRequest(_usernameTextField.value, _passwordTextField.value,
            onComplete: text =>
            {
                SetResponseBodyText(text);
                _sendLoginRequestButton.SetEnabled(true);
                PlayerPrefs.SetString(PlayerNamePrefKey, _usernameTextField.value);
            },
            onError: () => _sendLoginRequestButton.SetEnabled(true))
        );
        _sendLoginRequestButton.SetEnabled(false);
    }

    private void OnClickedSendLoginBasicRequest()
    {
        StartCoroutine(_requestsSender.SendLoginBasicRequest(_usernameTextField.value, _passwordTextField.value,
            onComplete: text =>
            {
                SetResponseBodyText(text);
                _sendLoginBasicRequestButton.SetEnabled(true);
                PlayerPrefs.SetString(PlayerNamePrefKey, _usernameTextField.value);
            },
            onError: () => _sendLoginBasicRequestButton.SetEnabled(true))
        );
        _sendLoginBasicRequestButton.SetEnabled(false);
    }

    private void OnClickedSendRegisterRequest()
    {
        StartCoroutine(_requestsSender.SendRegisterRequest(_usernameTextField.value, _passwordTextField.value,
            onComplete: () =>
            {
                Debug.Log("Registration Done");
                _sendRegisterRequestButton.SetEnabled(true);
            }, onError: () => _sendRegisterRequestButton.SetEnabled(true))
        );
        _sendRegisterRequestButton.SetEnabled(false);
    }

    private void SetResponseBodyText(string text)
    {
        var responseBody = _responseBodyLabel;
        if (responseBody != null)
            responseBody.text = text;
    }

    private void InitializeUIData()
    {
        if (_isDataInitialized) return;
        if (PlayerPrefs.HasKey(PlayerNamePrefKey))
            _usernameTextField.value = PlayerPrefs.GetString(PlayerNamePrefKey);
        _isDataInitialized = true;
    }
}