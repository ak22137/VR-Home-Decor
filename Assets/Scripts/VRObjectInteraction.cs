using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class VRCombinedInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public LayerMask interactableLayer;
    public Material highlightMaterial;
    public GameObject optionsMenuUI;

    [Header("Input Actions")]
    public InputActionReference leftTriggerAction;
    public InputActionReference rightTriggerAction;

    [Header("Controller References")]
    public Transform leftControllerTransform;
    public Transform rightControllerTransform;

    private GameObject currentlyHighlightedObject;
    private Material[] originalMaterials;
    private bool isTriggerPressed;
    private GameObject grabbedObject;
    private bool isGrabbing = false;
    private Quaternion originalRotation;

    private bool leftTriggerPressed = false;
    private bool rightTriggerPressed = false;

    private GameObject leftHitObject;
    private GameObject rightHitObject;



    private void OnEnable()
    {
        leftTriggerAction.action.Enable();
        rightTriggerAction.action.Enable();
        leftTriggerAction.action.started += OnLeftTriggerPressed;
        rightTriggerAction.action.started += OnRightTriggerPressed;
        leftTriggerAction.action.canceled += OnTriggerReleased;
        rightTriggerAction.action.canceled += OnTriggerReleased;
    }

    private void OnDisable()
    {
        leftTriggerAction.action.started -= OnLeftTriggerPressed;
        rightTriggerAction.action.started -= OnRightTriggerPressed;
        leftTriggerAction.action.canceled -= OnTriggerReleased;
        rightTriggerAction.action.canceled -= OnTriggerReleased;
        // leftTriggerAction.action.Disable();
        // rightTriggerAction.action.Disable();
    }



    void Update()
    {
        PerformRaycast(leftControllerTransform);
        PerformRaycast(rightControllerTransform);

        // Allow rotation only if both triggers are held and both controllers point to the same object
        if (isGrabbing && grabbedObject != null)
        {
            if (leftTriggerPressed && rightTriggerPressed &&
                leftHitObject == grabbedObject && rightHitObject == grabbedObject)
            {
                // Allow rotation (do nothing here, keep grabbedObject's current rotation)
            }
            else
            {
                // Lock rotation
                grabbedObject.transform.rotation = originalRotation;
            }
        }
    }



    private void PerformRaycast(Transform controller)
    {
        if (controller == null) return;

        RaycastHit hit;
        if (Physics.Raycast(controller.position, controller.forward, out hit, 10f, interactableLayer))
        {
            HighlightObject(hit.collider.gameObject);

            if (controller == leftControllerTransform)
                leftHitObject = hit.collider.gameObject;
            else if (controller == rightControllerTransform)
                rightHitObject = hit.collider.gameObject;
        }
        else
        {
            if (controller == leftControllerTransform)
                leftHitObject = null;
            else if (controller == rightControllerTransform)
                rightHitObject = null;

            RemoveHighlight();
        }
    }


    private void HighlightObject(GameObject obj)
    {
        if (obj != currentlyHighlightedObject)
        {
            RemoveHighlight();
            currentlyHighlightedObject = obj;
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                originalMaterials = renderer.materials;
                Material[] newMaterials = new Material[originalMaterials.Length + 1];
                originalMaterials.CopyTo(newMaterials, 0);
                newMaterials[newMaterials.Length - 1] = highlightMaterial;
                renderer.materials = newMaterials;
            }
        }
    }

    private void RemoveHighlight()
    {
        if (currentlyHighlightedObject != null)
        {
            var renderer = currentlyHighlightedObject.GetComponent<Renderer>();
            if (renderer != null && originalMaterials != null)
            {
                renderer.materials = originalMaterials;
            }
            currentlyHighlightedObject = null;
            originalMaterials = null;
        }
    }

    private void OnLeftTriggerPressed(InputAction.CallbackContext context)
    {
        leftTriggerPressed = true;

        if (leftHitObject != null && !(leftHitObject == rightHitObject && rightTriggerPressed))
        {
            ShowObjectOptions(leftHitObject);
        }
    }

    private void OnRightTriggerPressed(InputAction.CallbackContext context)
    {
        rightTriggerPressed = true;

        if (!isGrabbing && rightHitObject != null)
        {
            StartGrabbing(rightHitObject);
        }
    }


    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        if (context.action == leftTriggerAction.action)
            leftTriggerPressed = false;

        if (context.action == rightTriggerAction.action)
            rightTriggerPressed = false;

        isTriggerPressed = false;

        if (isGrabbing)
        {
            StopGrabbing();
        }
    }


    private void ShowObjectOptions(GameObject obj)
    {
        optionsMenuUI.SetActive(true);
        optionsMenuUI.transform.position = obj.transform.position + Vector3.up * 5f;
        //optionsMenuUI.transform.LookAt(Camera.main.transform);

        var optionsHandler = optionsMenuUI.GetComponent<ObjectOptionsHandler>();
        if (optionsHandler != null)
        {
            optionsHandler.ShowOptionsMenu(obj);
        }
    }

   

    private void StartGrabbing(GameObject obj)
    {
        isGrabbing = true;
        grabbedObject = obj;

        originalRotation = grabbedObject.transform.rotation;

        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        grabbedObject.transform.SetParent(rightControllerTransform);
    }


    private void StopGrabbing()
    {
        if (grabbedObject != null)
        {
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            grabbedObject.transform.SetParent(null);
            grabbedObject = null;
        }
        isGrabbing = false;
    }
}