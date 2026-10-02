using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    private Button hostButton;
    private Button joinButton;
    private TextField joinCodeTextField;
    
    private ISession currentSession;
    private PanelRenderer panelRenderer;

    async void Awake()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        // Find the PanelRenderer component and register the UI load callback
        panelRenderer = GetComponent<PanelRenderer>();
        if (panelRenderer != null)
        {
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
        }
        else
        {
            Debug.LogError("PanelRenderer component missing from this GameObject!");
        }
    }

    void OnDestroy()
    {
        if (panelRenderer != null)
        {
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
    {
        // Unregister old events to prevent duplicate triggers on reload
        if (hostButton != null) hostButton.clicked -= OnHostClicked;
        if (joinButton != null) joinButton.clicked -= OnJoinClicked;

        // Query elements from the PanelRenderer's root element
        var hostContainer = root.Q<VisualElement>("Host");
        if (hostContainer != null)
            hostButton = hostContainer.Q<Button>();

        var joinContainer = root.Q<VisualElement>("Join");
        if (joinContainer != null)
        {
            joinButton = joinContainer.Q<Button>();
            joinCodeTextField = joinContainer.Q<TextField>();
        }

        if (joinCodeTextField != null)
            joinCodeTextField.value = "";

        if (hostButton != null)
        {
            hostButton.clicked += OnHostClicked;
            Debug.Log("PanelRenderer: Host button successfully hooked up!");
        }

        if (joinButton != null)
        {
            joinButton.clicked += OnJoinClicked;
            Debug.Log("PanelRenderer: Join button successfully hooked up!");
        }
    }

    private async void OnHostClicked()
    {
        Debug.Log("Host Clicked");
        try
        {
            var options = new SessionOptions
            {
                Name = "SurvivalGameSession",
                MaxPlayers = 4,
                IsPrivate = false
            }.WithRelayNetwork();

            // Create session once and assign both locally and to the static manager
            currentSession = await MultiplayerService.Instance.CreateSessionAsync(options);
            GameSessionManager.CurrentSession = currentSession;
            
            Debug.Log($"Session Created! Join Code: {GameSessionManager.CurrentSession.Code}");
            
            gameObject.SetActive(false);
        }
        catch (SessionException e)
        {
            Debug.LogError($"Failed to create session: {e.Message}");
        }
    }

    private async void OnJoinClicked()
    {
        try
        {
            string joinCode = joinCodeTextField != null ? joinCodeTextField.value.Trim().ToUpper() : "";
            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.LogWarning("Please enter a valid join code into the text field.");
                return;
            }

            Debug.Log($"Attempting to join session with code: {joinCode}");
            
            // Join session once and assign both locally and to the static manager
            currentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
            GameSessionManager.CurrentSession = currentSession;
            
            Debug.Log("Successfully joined session!");

            gameObject.SetActive(false);
        }
        catch (SessionException e)
        {
            Debug.LogError($"Failed to join session: {e.Message}");
        }
    }
}