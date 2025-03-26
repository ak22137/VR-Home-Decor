using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Sigtrap;

public class VRScalingWithGizmos : MonoBehaviour
{
    public Vector3 scalingAxis = Vector3.one; // Default: Uniform scaling
    public InputActionReference triggerAction;
    private GameObject targetObject;
    private XRBaseInteractor activeInteractor;
    private bool isScaling = false;
    private Vector3 initialScale;
    private Vector3 initialHandPosition;

    private void Start()
    {
        triggerAction.action.Enable();
        triggerAction.action.started += OnTriggerPressed;
        triggerAction.action.canceled += OnTriggerReleased;
    }

    private void OnDestroy()
    {
        triggerAction.action.started -= OnTriggerPressed;
        triggerAction.action.canceled -= OnTriggerReleased;
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (activeInteractor != null)
        {
            targetObject = transform.parent.gameObject;
            if (targetObject != null)
            {
                initialScale = targetObject.transform.localScale;
                initialHandPosition = activeInteractor.transform.position;
                isScaling = true;
            }
        }
    }

    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        isScaling = false;
    }

    private void Update()
    {
        if (isScaling && targetObject != null && activeInteractor != null)
        {
            Vector3 currentHandPosition = activeInteractor.transform.position;
            float scaleChange = Vector3.Distance(currentHandPosition, initialHandPosition);
            targetObject.transform.localScale = initialScale + (scalingAxis * scaleChange);

            // Draw VR Gizmo Handles
            VrGizmos.DrawBox(targetObject.transform.position + Vector3.right * 0.5f, Quaternion.identity, Vector3.one * 0.1f, Color.red);
            VrGizmos.DrawBox(targetObject.transform.position + Vector3.up * 0.5f, Quaternion.identity, Vector3.one * 0.1f, Color.green);
            VrGizmos.DrawBox(targetObject.transform.position + Vector3.forward * 0.5f, Quaternion.identity, Vector3.one * 0.1f, Color.blue);
            VrGizmos.DrawSphere(activeInteractor.transform.position, 0.05f, Color.yellow);
        }
    }

    public void OnSelectEnter(XRBaseInteractor interactor)
    {
        activeInteractor = interactor;
    }

    public void OnSelectExit(XRBaseInteractor interactor)
    {
        if (interactor == activeInteractor)
        {
            isScaling = false;
            activeInteractor = null;
        }
    }
}
