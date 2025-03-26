using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectScaler : MonoBehaviour
{
    public Transform targetObject;
    public Transform xGizmo, yGizmo, zGizmo;
    public InputActionReference leftControllerTrigger;
    private Transform activeGizmo;
    private Vector3 initialScale;
    private float scaleSpeed = 0.01f;

    private void OnEnable()
    {
        leftControllerTrigger.action.Enable();
        leftControllerTrigger.action.performed += OnTriggerPressed;
    }

    private void OnDisable()
    {
        leftControllerTrigger.action.performed -= OnTriggerPressed;
        leftControllerTrigger.action.Disable();
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (activeGizmo == null) return;
        
        float input = context.ReadValue<float>();
        Vector3 scaleChange = Vector3.zero;

        if (activeGizmo == xGizmo) scaleChange = new Vector3(input * scaleSpeed, 0, 0);
        else if (activeGizmo == yGizmo) scaleChange = new Vector3(0, input * scaleSpeed, 0);
        else if (activeGizmo == zGizmo) scaleChange = new Vector3(0, 0, input * scaleSpeed);

        targetObject.localScale += scaleChange;
    }

    public void SetActiveGizmo(Transform gizmo)
    {
        activeGizmo = gizmo;
    }
}
