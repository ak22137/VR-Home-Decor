using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VRInitialMenu : MonoBehaviour
{
    public GameObject[] spawnableObjects;
    public Transform menuSpawnPoint;
    public GameObject menuUIPrefab;
    public GameObject buttonPrefab;
    [Header("Input Actions")]
    public InputActionReference primaryButtonAction;

    private GameObject currentMenuInstance;
    private bool isPrimaryButtonPressed;

    private void OnEnable()
    {
        primaryButtonAction.action.Enable();
        primaryButtonAction.action.started += OnPrimaryButtonPressed;
        primaryButtonAction.action.canceled += OnPrimaryButtonReleased;
    }

    private void OnDisable()
    {
        primaryButtonAction.action.started -= OnPrimaryButtonPressed;
        primaryButtonAction.action.canceled -= OnPrimaryButtonReleased;
        // primaryButtonAction.action.Disable();
    }

    private void OnPrimaryButtonPressed(InputAction.CallbackContext context)
    {
        if (!isPrimaryButtonPressed)
        {
            isPrimaryButtonPressed = true;
            ToggleMenu();
        }
        Debug.Log("Primary trigger is pressed");
    }

    private void OnPrimaryButtonReleased(InputAction.CallbackContext context)
    {
        isPrimaryButtonPressed = false;
    }

    private void ToggleMenu()
    {
        if (currentMenuInstance == null)
        {
            currentMenuInstance = Instantiate(menuUIPrefab, menuSpawnPoint.position, menuSpawnPoint.rotation);
            PopulateMenu(currentMenuInstance);
        }
        else
        {
            Destroy(currentMenuInstance);
            currentMenuInstance = null;
        }
    }

    private void PopulateMenu(GameObject menuInstance)
    {
        // GameObject buttonPrefab = Resources.Load<GameObject>("UI/ButtonPrefab");

        // if (buttonPrefab == null)
        // {
        //     Debug.LogError("ButtonPrefab could not be loaded! Ensure it is placed in 'Assets/Resources/UI/'");
        //     return;
        // }

        // Find the Panel inside the menuInstance where buttons should be placed
        Transform menuPanel = FindDeepChild(menuInstance.transform, "Panel");
        if (menuPanel == null)
        {
            Debug.LogError("Menu instance is missing a 'Panel' child object!");
            return;
        }

        foreach (var obj in spawnableObjects)
        {
            GameObject button = Instantiate(buttonPrefab, menuPanel); // Spawn inside the panel

            // Set button text
            TMPro.TextMeshProUGUI buttonText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = obj.name;
            }
            else
            {
                Debug.LogError("ButtonPrefab is missing a TextMeshProUGUI component.");
            }

            // Add click event
            UnityEngine.UI.Button uiButton = button.GetComponent<UnityEngine.UI.Button>();
            if (uiButton != null)
            {
                uiButton.onClick.AddListener(() => SpawnObject(obj));
            }
            else
            {
                Debug.LogError("ButtonPrefab is missing a Button component.");
            }
        }
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindDeepChild(child, name);
            if (found != null) return found;
        }
        return null;
    }
    private void SpawnObject(GameObject obj)
    {
        
        var spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 3f;

       
        GameObject spawnedObject = Instantiate(obj, spawnPosition, Quaternion.identity);

        
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = spawnedObject.AddComponent<Rigidbody>();
        }

       
        rb.isKinematic = false;
    }
}
