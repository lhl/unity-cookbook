using UnityEngine;
using System.Collections;
using Leap;

public class WindowManager : MonoBehaviour {

    public GameObject camera;
    public GameObject LeftEye;
    public GameObject RightEye;
    public GameObject overlay;
    public HandController hc;
    public GameObject rightCursor;

    private bool overlay_status;

    private Vector3 last_finger_position;

	// Use this for initialization
	void Start () {
        overlay_status = overlay.GetComponent<Renderer>().enabled;

        hc.GetLeapController().EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
        hc.GetLeapController().EnableGesture(Gesture.GestureType.TYPE_SWIPE);
    }

    // Update is called once per frame
    void Update () {

        /*** Look Down: Show Overlay ***/
        checkOverlay();

        /*** Keyboard ***/
        if (/*Input.GetKey(KeyCode.LeftControl) && */ Input.GetKeyDown("n"))
        {
            Instantiate(rightCursor, camera.transform.position, Quaternion.identity);

            // RaycastHit hit;
            // Ray ray = new Ray(camera.transform.position, camera.transform.forward);

            /*
            if (Physics.Raycast(ray, out hit))
            {
                thisTransform.position = hit.point + (-cameraForward * offsetFromObjects);
                thisTransform.forward = -cameraForward;
            }
            */



            Debug.Log("new window");
        }

        /*** WIP ***/
        foreach (HandModel hand_model in hc.GetAllGraphicsHands())
        {
  
            Hand hand = hand_model.GetLeapHand();

            // hand.isLeft
            // hand.isRight
            if (hand.Fingers[1].IsExtended) {
                rightCursor.transform.position = hand_model.fingers[1].GetTipPosition();
                last_finger_position = hand_model.fingers[1].GetTipPosition();
                // We now want to get the unity info...
                /*
                Debug.Log(hand.Fingers[1].TipPosition);
                UnityVectorExtension.ToUnityScaled(hand.Fingers[1].TipPosition, true);
                */
            }
        }


        // Hand firstHand = hands[0];

        /*
        HandModel hand_model = GetComponent<HandModel>();
        Hand leap_hand = hand_model.GetLeapHand();
        Debug.Log(leap_hand);

        
        HandList handsForGesture = gesture.Hands;

        */
    }

    void checkOverlay()
    {
        // Debug.Log(camera.transform.rotation[0]);
        if (camera.transform.rotation[0] <= -0.33)
        {
            overlay.GetComponent<Renderer>().enabled = true;
            LeftEye.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            RightEye.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        }
        else if (overlay_status == true)
        {
            overlay.GetComponent<Renderer>().enabled = false;
            LeftEye.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
            RightEye.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;

        }
    }
}
