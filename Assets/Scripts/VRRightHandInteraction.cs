using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRRightHandInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public LayerMask interactableLayer; // Layer for interactable objects
    public Material highlightMaterial;  // Highlight material

    [Header("Input Actions")]
    public InputActionReference triggerButtonAction; // Trigger button input action

    private Transform controllerTransform;
    private GameObject currentlyHighlightedObject;
    private Material originalMaterial;
    private bool isTriggerPressed;

    private GameObject grabbedObject;
    private bool isGrabbing = false;

    private void Awake()
    {
        // Find the right-hand XR controller (Ray Interactor or Direct Interactor)
        XRBaseInteractor controller = FindAnyObjectByType<XRDirectInteractor>();
        if (controller == null)
        {
            controller = FindAnyObjectByType<XRRayInteractor>();
        }

        if (controller != null)
        {
            controllerTransform = controller.transform;
        }
        else
        {
            Debug.LogError("No XR Controller (Direct or Ray) found for right hand!");
        }
    }

    private void OnEnable()
    {
        triggerButtonAction.action.Enable();
        triggerButtonAction.action.started += OnTriggerPressed;
        triggerButtonAction.action.canceled += OnTriggerReleased;
    }

    private void OnDisable()
    {
        triggerButtonAction.action.started -= OnTriggerPressed;
        triggerButtonAction.action.canceled -= OnTriggerReleased;
        triggerButtonAction.action.Disable();
    }

    void Update()
    {
        PerformRaycast();
    }

    private void PerformRaycast()
    {
        if (controllerTransform == null) return;

        RaycastHit hit;
        if (Physics.Raycast(controllerTransform.position, controllerTransform.forward, out hit, 10f, interactableLayer))
        {
            HighlightObject(hit.collider.gameObject);
        }
        else
        {
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
                originalMaterial = renderer.material;
                Material[] materials = renderer.materials;
                System.Array.Resize(ref materials, materials.Length + 1);
                materials[materials.Length - 1] = highlightMaterial;
                renderer.materials = materials;
            }
        }
    }

    private void RemoveHighlight()
    {
        if (currentlyHighlightedObject != null)
        {
            var renderer = currentlyHighlightedObject.GetComponent<Renderer>();
            if (renderer != null && originalMaterial != null)
            {
                renderer.materials = new Material[] { originalMaterial };
            }
            currentlyHighlightedObject = null;
        }
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        isTriggerPressed = true;
        if (!isGrabbing && currentlyHighlightedObject != null)
        {
            StartGrabbing(currentlyHighlightedObject);
        }
    }

    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        isTriggerPressed = false;
        if (isGrabbing)
        {
            StopGrabbing();
        }
    }

    private void StartGrabbing(GameObject obj)
    {
        isGrabbing = true;
        grabbedObject = obj;

        // Disable Rigidbody physics to move the object smoothly
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Attach object to controller
        grabbedObject.transform.SetParent(controllerTransform);
    }

    private void StopGrabbing()
    {
        if (grabbedObject != null)
        {
            // Re-enable physics when dropping
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            // Detach from controller
            grabbedObject.transform.SetParent(null);
            grabbedObject = null;
        }

        isGrabbing = false;
    }
}