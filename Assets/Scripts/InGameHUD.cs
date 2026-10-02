using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class InGameHUD : MonoBehaviour
{
    private Label joinCodeLabel;
    private const string PlaceholderText = "Server Code..."; // Match whatever your UXML placeholder text is

    void OnEnable()
    {
        var panelRenderer = GetComponent<PanelRenderer>();
        if (panelRenderer == null)
        {
            Debug.LogError("PanelRenderer missing on InGameHUD!");
            return;
        }
        
        panelRenderer.RegisterUIReloadCallback((renderer, root, version) =>
        {
            var mainContainer = root.Q<VisualElement>("Main");
            if (mainContainer != null)
            {
                joinCodeLabel = mainContainer.Q<Label>("ServerCodeLabel");
            }

            UpdateJoinCodeDisplay();
        });
    }

    void Update()
    {
        // Keep checking until the label no longer shows the placeholder text
        if (joinCodeLabel != null && joinCodeLabel.text == PlaceholderText)
        {
            UpdateJoinCodeDisplay();
        }
    }

    private void UpdateJoinCodeDisplay()
    {
        if (joinCodeLabel == null) return;

        var session = GameSessionManager.CurrentSession;
        if (session != null && !string.IsNullOrEmpty(session.Code))
        {
            joinCodeLabel.text = $"Code: {session.Code}";
            joinCodeLabel.style.display = DisplayStyle.Flex;
        }
    }
}