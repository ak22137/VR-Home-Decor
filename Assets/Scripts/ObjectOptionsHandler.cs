using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class ObjectOptionsHandler : MonoBehaviour
{
    public TextMeshProUGUI optionText;
    public GameObject optionsMenuUI;
    public InputActionReference leftControllerTrigger;
    
    // [SerializeField] private Button scaleButton;
    // [SerializeField] private Button rotateButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button duplicateButton;
    
    [SerializeField] private Button selectScaleXButton;
    [SerializeField] private Button selectScaleYButton;
    [SerializeField] private Button selectScaleZButton;
    [SerializeField] private Button scaleIncreaseButton;
    [SerializeField] private Button scaleDecreaseButton;
    
    [SerializeField] private Button selectRotateXButton;
    [SerializeField] private Button selectRotateYButton;
    [SerializeField] private Button selectRotateZButton;
    [SerializeField] private Button rotateIncreaseButton;
    [SerializeField] private Button rotateDecreaseButton;
    
    [SerializeField] private TextMeshProUGUI scaleText;
    [SerializeField] private TextMeshProUGUI rotationText;
    
    [SerializeField] private float scaleSensitivity = 0.1f;
    [SerializeField] private float rotateSensitivity = 15f;
    
    private GameObject targetObject;
    private Vector3 selectedScaleAxis = Vector3.one;
    private Vector3 selectedRotateAxis = Vector3.up;

    private bool duplicateOnce = false;
    
    private void OnEnable()
    {
        leftControllerTrigger.action.Enable();
        leftControllerTrigger.action.started += OnTriggerPressed;
    }
    
    private void OnDisable()
    {
        leftControllerTrigger.action.started -= OnTriggerPressed;
    }

    public void ShowOptionsMenu(GameObject obj)
    {
        targetObject = obj;
        optionsMenuUI.SetActive(true);
        optionsMenuUI.transform.position = targetObject.transform.position + Vector3.up * 0.5f;
        optionsMenuUI.transform.LookAt(Camera.main.transform);
        UpdateScaleText();
        UpdateRotationText();
        AssignButtonActions();
    }

    private void AssignButtonActions()
    {
        // if (scaleButton != null) scaleButton.onClick.AddListener(EnableScalingUI);
        // if (rotateButton != null) rotateButton.onClick.AddListener(EnableRotationUI);
        if (deleteButton != null) deleteButton.onClick.AddListener(DeleteObject);
        if (duplicateButton != null) duplicateButton.onClick.AddListener(DuplicateObject);
        
        if (selectScaleXButton != null) selectScaleXButton.onClick.AddListener(() => SelectScaleAxis(Vector3.right));
        if (selectScaleYButton != null) selectScaleYButton.onClick.AddListener(() => SelectScaleAxis(Vector3.up));
        if (selectScaleZButton != null) selectScaleZButton.onClick.AddListener(() => SelectScaleAxis(Vector3.forward));
        
        if (scaleIncreaseButton != null) scaleIncreaseButton.onClick.AddListener(() => ScaleObject(scaleSensitivity));
        if (scaleDecreaseButton != null) scaleDecreaseButton.onClick.AddListener(() => ScaleObject(-scaleSensitivity));
        
        if (selectRotateXButton != null) selectRotateXButton.onClick.AddListener(() => SelectRotateAxis(Vector3.right));
        if (selectRotateYButton != null) selectRotateYButton.onClick.AddListener(() => SelectRotateAxis(Vector3.up));
        if (selectRotateZButton != null) selectRotateZButton.onClick.AddListener(() => SelectRotateAxis(Vector3.forward));
        
        if (rotateIncreaseButton != null) rotateIncreaseButton.onClick.AddListener(() => RotateObject(rotateSensitivity));
        if (rotateDecreaseButton != null) rotateDecreaseButton.onClick.AddListener(() => RotateObject(-rotateSensitivity));
    }

    private void EnableScalingUI()
    {
        optionsMenuUI.SetActive(false);
    }
    
    private void EnableRotationUI()
    {
        optionsMenuUI.SetActive(false);
    }

    private void SelectScaleAxis(Vector3 axis)
    {
        selectedScaleAxis = axis;
    }
    
    private void SelectRotateAxis(Vector3 axis)
    {
        selectedRotateAxis = axis;
    }

    private void ScaleObject(float amount)
    {
        if (targetObject == null) return;
        Vector3 newScale = targetObject.transform.localScale + (selectedScaleAxis * amount);
        targetObject.transform.localScale = newScale;
        UpdateScaleText();
    }

    private void RotateObject(float amount)
    {
        if (targetObject == null) return;
        targetObject.transform.Rotate(selectedRotateAxis, amount);
        UpdateRotationText();
    }

    private void UpdateScaleText()
    {
        if (targetObject != null && scaleText != null)
        {
            scaleText.text = $"Scale: X={targetObject.transform.localScale.x:F2}, Y={targetObject.transform.localScale.y:F2}, Z={targetObject.transform.localScale.z:F2}";
        }
    }
    
    private void UpdateRotationText()
    {
        if (targetObject != null && rotationText != null)
        {
            Vector3 rotation = targetObject.transform.eulerAngles;
            rotationText.text = $"Rotation: X={rotation.x:F2}, Y={rotation.y:F2}, Z={rotation.z:F2}";
        }
    }
    
    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
    }
    
    private void DeleteObject()
    {
        Destroy(targetObject);
        optionsMenuUI.SetActive(false);
    }

    private void DuplicateObject()
    {
        if (duplicateOnce == false)
        {
            Instantiate(targetObject, targetObject.transform.position + Vector3.up * 2f, targetObject.transform.rotation);
        }
        else
        {

        }
        duplicateOnce = true;

    }
}