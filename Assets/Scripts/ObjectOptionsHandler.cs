using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class ObjectOptionsHandler : MonoBehaviour
{
    public TextMeshProUGUI optionText;
    public GameObject optionsMenuUI;
    public GameObject scalingGizmosPrefab;
    public InputActionReference leftControllerTrigger;
    
    [SerializeField] private Button scaleButton;
    [SerializeField] private Button rotateButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button duplicateButton;
    
    private GameObject targetObject;
    private GameObject spawnedGizmos;
    private bool isGizmosActive = false;
    
    private void OnEnable()
    {
        leftControllerTrigger.action.Enable();
        leftControllerTrigger.action.started += OnTriggerPressed;
    }
    
    private void OnDisable()
    {
        leftControllerTrigger.action.started -= OnTriggerPressed;
        // leftControllerTrigger.action.Disable();
    }

    public void ShowOptionsMenu(GameObject obj)
    {
        targetObject = obj;
        optionsMenuUI.SetActive(true);
        optionsMenuUI.transform.position = targetObject.transform.position + Vector3.up * 0.5f;
        optionsMenuUI.transform.LookAt(Camera.main.transform);
        AssignButtonActions();
    }

    private void AssignButtonActions()
    {
        if (scaleButton != null) scaleButton.onClick.AddListener(EnableScalingGizmos);
        if (rotateButton != null) rotateButton.onClick.AddListener(RotateObject);
        if (deleteButton != null) deleteButton.onClick.AddListener(DeleteObject);
        if (duplicateButton != null) duplicateButton.onClick.AddListener(DuplicateObject);
    }

    private void EnableScalingGizmos()
    {
        isGizmosActive = true;
        optionsMenuUI.SetActive(false);
        
        if (spawnedGizmos != null)
        {
            Destroy(spawnedGizmos);
        }
        
        spawnedGizmos = Instantiate(scalingGizmosPrefab, targetObject.transform.position, Quaternion.identity);
        spawnedGizmos.transform.SetParent(targetObject.transform);
    }

    private void RotateObject()
    {
        targetObject.transform.Rotate(Vector3.up, 45f);
    }
    
    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (isGizmosActive)
        {
            // Logic to scale object using gizmos
        }
    }
    
    private void ChangeObjectColor()
    {
        Renderer renderer = targetObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Random.ColorHSV();
        }
    }

    private void DeleteObject()
    {
        Destroy(targetObject);
        optionsMenuUI.SetActive(false);
    }

    private void DuplicateObject()
    {
        Instantiate(targetObject, targetObject.transform.position + Vector3.up * 2f, targetObject.transform.rotation);
    }
}
