using UnityEngine;
using System.Collections;

public class crosshair_gaze : MonoBehaviour
{
    public float offsetFromObjects = 0.1f;
    public float fixedDepth = 3.0f;
    public OVRCameraRig cameraController = null;

    private Transform thisTransform = null;
    private Material crosshairMaterial = null;

    /// <summary>
    /// Initialize the crosshair
    /// </summary>
    void Awake()
    {
        thisTransform = transform;
        if (cameraController == null)
        {
            Debug.LogError("ERROR: missing camera controller object on " + name);
            enabled = false;
            return;
        }
        // clone the crosshair material
        crosshairMaterial = GetComponent<Renderer>().material;
    }

    /// <summary>
    /// Cleans up the cloned material
    /// </summary>
    void OnDestroy()
    {
        if (crosshairMaterial != null)
        {
            Destroy(crosshairMaterial);
        }
    }

    /// <summary>
    /// Updates the position of the crosshair.
    /// </summary>
    void LateUpdate()
    {

        Ray ray;
        RaycastHit hit;

        // get the camera forward vector and position
        Vector3 cameraPosition = cameraController.centerEyeAnchor.position;
        Vector3 cameraForward = cameraController.centerEyeAnchor.forward;

        GetComponent<Renderer>().enabled = true;

        // cursor positions itself in 3D based on raycasts into the scene
        // trace to the spot that the player is looking at
        ray = new Ray(cameraPosition, cameraForward);
        if (Physics.Raycast(ray, out hit))
        {
            thisTransform.position = hit.point + (-cameraForward * offsetFromObjects);
            thisTransform.forward = -cameraForward;
        }
    }
}
