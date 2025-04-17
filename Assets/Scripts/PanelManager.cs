using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject homePanel;
    [SerializeField] private List<PanelData> panels = new List<PanelData>();

    private GameObject currentActivePanel;
    [SerializeField] private Button quitButton;
    private void Start()
    {

        homePanel.SetActive(true);
        currentActivePanel = homePanel;
        quitButton.onClick.AddListener(ExitGame);

        foreach (var panelData in panels)
        {
            panelData.panel.SetActive(false);
        }


        SetupButtonListeners();
    }

    private void SetupButtonListeners()
    {

        foreach (var panelData in panels)
        {

            if (panelData.openButton != null)
            {
                panelData.openButton.onClick.AddListener(() => OpenPanel(panelData.panel));
            }

            if (panelData.backButton != null)
            {
                panelData.backButton.onClick.AddListener(() => ReturnToHomePanel());
            }
        }
    }


    private void OpenPanel(GameObject panelToOpen)
    {

        homePanel.SetActive(false);


        if (currentActivePanel != null && currentActivePanel != homePanel)
        {
            currentActivePanel.SetActive(false);
        }


        panelToOpen.SetActive(true);
        currentActivePanel = panelToOpen;
    }

    private void ReturnToHomePanel()
    {

        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
        }


        homePanel.SetActive(true);
        currentActivePanel = homePanel;
    }
    public void ExitGame()
    {
        Debug.Log("Quitting application...");

        #if UNITY_EDITOR
        
        UnityEditor.EditorApplication.isPlaying = false;
        
        #else
        
        Application.Quit();
        #endif
    }

    [System.Serializable]
    public class PanelData
    {
        public string panelName;
        public GameObject panel;
        public Button openButton;
        public Button backButton;
    }
}