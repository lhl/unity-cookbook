using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Leap;

public class WindowManager : MonoBehaviour
{
    public GameObject center;
    public GameObject leftEye;
    public GameObject rightEye;
    public GameObject overlay;
    public HandController handController;
    public GameObject gazeCursor;
    public GameObject mouseCursor;
    public GameObject rightCursor;
    public GameObject leftCursor;
    public Transform windows;
    public GameObject browser;
    public GameObject addressBar;

    private bool overlay_status;
    private Vector3 last_finger_position;
    private Ray last_finger_ray;
    private GameObject focused;
    private GameObject selected;
    private string last_input;

    // Use this for initialization
    void Start()
    {
        UnityEngine.VR.InputTracking.Recenter();

        overlay_status = overlay.GetComponent<Renderer>().enabled;

        // handController.GetLeapController().EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
        handController.GetLeapController().EnableGesture(Gesture.GestureType.TYPE_SWIPE);
        handController.GetLeapController().EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
    }

    // Update is called once per frame
    void Update()
    {

        /*** Look Down: Show Overlay ***/
        checkOverlay();

        /*** Hand Cursors ***/
        drawHandCursors();

        /*** Gaze Cursor ***/
        drawGazeCursor();

        /*** Check if we are selecting windows ***/
        windowSelection();



        /*** Keyboard ***/
        keyboardCommands();





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
        // Debug.Log(center.transform.rotation[0]);
        if (center.transform.rotation[0] <= -0.33)
        {
            overlay.GetComponent<Renderer>().enabled = true;
            leftEye.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            rightEye.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        }
        else if (overlay_status == true)
        {
            overlay.GetComponent<Renderer>().enabled = false;
            leftEye.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
            rightEye.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;

        }
    }

    void drawHandCursors()
    {
        GameObject currentCursor;
        rightCursor.GetComponent<Renderer>().enabled = false;
        leftCursor.GetComponent<Renderer>().enabled = false;
        foreach (HandModel hand_model in handController.GetAllGraphicsHands())
        {
            Hand hand = hand_model.GetLeapHand();

            if (hand.IsLeft)
            {
                currentCursor = leftCursor;
            }
            else
            {
                currentCursor = rightCursor;
            }

            if (hand.Fingers[1].IsExtended && !(hand.Fingers[0].IsExtended || hand.Fingers[3].IsExtended))
            {
                currentCursor.transform.position = hand_model.fingers[1].GetTipPosition();
                currentCursor.transform.forward = -center.transform.forward;
                currentCursor.GetComponent<Renderer>().enabled = true;
                last_finger_position = hand_model.fingers[1].GetTipPosition();
                last_finger_ray = hand_model.fingers[1].GetRay();

                if (selected)
                {
                    // Debug.Log(last_finger_position);
                    Vector3 direction = (last_finger_position - center.transform.position).normalized;
                    Ray ray = new Ray(center.transform.position, direction);
                    selected.transform.position = ray.GetPoint(1.0f);
                    selected.transform.forward = center.transform.forward;

                }
            }
            else
            {
                selected = null;
            }
        }

    }

    void drawGazeCursor()
    {
        // See https://github.com/lhl/unity-cookbook/blob/master/Assets/_Objects/crosshair_gaze/crosshair_gaze.cs
        float offsetFromObjects = 0.01f;
        float fixedDepth = 1.2f; // Actual DK2 focal distance

        Ray ray;
        RaycastHit hit;

        // Let's use a layer mask to only interact w/ windows
        // http://answers.unity3d.com/questions/416919/making-raycast-ignore-multiple-layers.html
        LayerMask layerMask = 1 << LayerMask.NameToLayer("Windows"); // only check for collisions with layerX
        // layerMask = ~(1 << LayerMask.NameToLayer("layerX")); // ignore collisions with layerX
        // LayerMask layerMask = ~(1 << LayerMask.NameToLayer("layerX") | 1 << LayerMask.NameToLayer("layerY")); // ignore both layerX and layerY

        ray = new Ray(center.transform.position, center.transform.forward);
        if (Physics.Raycast(ray, out hit, 20.0f, layerMask))
        {
            // Debug.Log(hit.collider.name);
            gazeCursor.transform.position = hit.point + (-center.transform.forward * offsetFromObjects);
            gazeCursor.transform.forward = -center.transform.forward;

            // This is a hack but WTH - if nothing focused, focus on the window you're looking at
            if(!focused)
            {
                focused = hit.collider.gameObject;
            }
        }
        else
        {
            gazeCursor.transform.position = center.transform.position + (center.transform.forward * fixedDepth);
            gazeCursor.transform.forward = -center.transform.forward;

        }
    }


    void windowSelection()
    {
        foreach (HandModel hand_model in handController.GetAllGraphicsHands())
        {
            Hand hand = hand_model.GetLeapHand();

            if (hand.Fingers[1].IsExtended)
            {
                RaycastHit hit;
                Ray ray = hand_model.fingers[1].GetRay();
                LayerMask layerMask = 1 << LayerMask.NameToLayer("Windows");
                if (Physics.Raycast(ray, out hit, 20.0f, layerMask))
                {
                    // Debug.Log(hit.collider.name);
                    selected = hit.collider.gameObject;

                    // focused window is always last selected or created
                    focused = selected;
                    focusWindow(focused);

                    // selected.GetComponent<ParticleSystem>().enableEmission = true;
                }
                else
                {
                    if (selected)
                    {

                        // selected.GetComponent<ParticleSystem>().enableEmission = false;
                        selected = null;
                    }
                }
            }
        }

    }

    void keyboardCommands()
    {

        // Recenter
        if (Input.GetKeyUp("r") && Input.GetKey(KeyCode.LeftControl))
        {
            UnityEngine.VR.InputTracking.Recenter();
        }

        // New Browser
        else if (Input.GetKeyUp("n") && Input.GetKey(KeyCode.LeftControl))
        {
            Ray ray = new Ray(center.transform.position, center.transform.forward);
            GameObject w = (GameObject)Instantiate(browser, ray.GetPoint(1.0f), Quaternion.identity);

            focusWindow(w);

            w.transform.forward = center.transform.forward;
            w.transform.parent = windows;
        }

        // New Terminal
        else if (Input.GetKeyUp("t") && Input.GetKey(KeyCode.LeftControl))
        {
            Ray ray = new Ray(center.transform.position, center.transform.forward);
            GameObject w = (GameObject)Instantiate(browser, ray.GetPoint(1.0f), Quaternion.identity);

            w.GetComponent<UWKWebView>().URL = "http://localhost:8765/";

            focusWindow(w);

            w.transform.forward = center.transform.forward;
            w.transform.parent = windows;
        }

        // Address Bar
        else if ((Input.GetKeyUp("l") && Input.GetKey(KeyCode.LeftControl)) || (Input.GetKeyUp("space") && Input.GetKey(KeyCode.LeftControl)))
        {
            if(addressBar.GetComponent<Canvas>().enabled)
            {
                addressBar.GetComponent<Canvas>().enabled = false;
                addressBar.GetComponentInChildren<InputField>().DeactivateInputField();
                addressBar.GetComponentInChildren<InputField>().enabled = false;
                focusWindow(focused);

            } else { 
                addressBar.GetComponent<Canvas>().enabled = true;
                addressBar.GetComponentInChildren<InputField>().enabled = true;
                addressBar.GetComponentInChildren<InputField>().ActivateInputField();
                focusWindow();
            }
        }

        // Close Window
        else if (Input.GetKeyUp("w") && Input.GetKey(KeyCode.LeftControl))
        {
            if(focused)
            {
                Destroy(focused);
                focused = null;
                selected = null;
            }
        }

        // Quit
        else if (Input.GetKeyUp("q") && Input.GetKey(KeyCode.LeftAlt))
        {
            Application.Quit();
        }


        // RaycastHit hit;
        // Ray ray = new Ray(center.transform.position, center.transform.forward);

        /*
        if (Physics.Raycast(ray, out hit))
        {
            thisTransform.position = hit.point + (-cameraForward * offsetFromObjects);
            thisTransform.forward = -cameraForward;
        }
        */




    }


    void focusWindow(GameObject w=null)
    {
        // Unfocus the rest - in theory, w/ focused you should only need to unfocus the currently focused...
        GameObject[] ws = GameObject.FindGameObjectsWithTag("Window");
        foreach (GameObject wx in ws)
        {
            wx.GetComponent<WindowBrowser>().KeyboardEnabled = false;
            wx.GetComponent<WindowBrowser>().MouseEnabled = false;
            wx.GetComponent<WindowBrowser>().HasFocus = false;
        }

        // Focus 1
        if(w) {         
            w.GetComponent<WindowBrowser>().KeyboardEnabled = true;
            w.GetComponent<WindowBrowser>().MouseEnabled = true;
            w.GetComponent<WindowBrowser>().HasFocus = true;
        }
    }


    public void loadURL(string url)
    {
        url = url.Trim();
        Debug.Log(url);
        if((url != last_input) && (url != ""))
        {
            // Debug.Log(focused);

            // If no Windows, lets make one and focus it - hacky
            if(GameObject.FindGameObjectsWithTag("Window").Length == 0)
            {
                Ray ray = new Ray(center.transform.position, center.transform.forward);
                GameObject w = (GameObject)Instantiate(browser, ray.GetPoint(1.0f), Quaternion.identity);

                focusWindow(w);

                w.transform.forward = center.transform.forward;
                w.transform.parent = windows;
            }

            // Load in Focused Window
            if (focused)
            {
                // No local file browsing w/ this...
                if (!url.StartsWith("http"))
                {
                    // Debug.Log("switching...");
                    switch (url.ToLower())
                    {
                        // Commands
                        case "t":
                        case "shell":
                        case "terminal":
                            url = "http://localhost:8765/";
                            break;

                        case "help":
                        case "?":
                        case "about":
                            url = "http://lhl.github.io/vrwm/help.html";
                            break;

                        // Common sites
                        case "google":
                            url = "http://google.com/";
                            break;

                        case "twitter":
                            url = "http://twitter.com/";
                            break;

                        case "fb":
                        case "facebook":
                            url = "https://facebook.com/";
                            break;

                        case "hn":
                            url = "https://news.ycombinator.com/";
                            break;

                        // Easter Eggs
                        case "lhl":
                            url = "https://twitter.com/lhl";
                            break;

                        case "vr":
                            url = "https://twitter.com/lhl/lists/vr";
                            break;

                        case "oculus":
                        case "bored":
                            url = "https://www.reddit.com/r/oculus";
                            break;

                        case "leap":
                        case "leapmotion":
                        case "leap motion":
                        case "gamejam":
                        case "game jam":
                            url = "https://itch.io/jam/leapmotion3djam";
                            break;

                        // Otherwise you probably wanted to add http to your URL...
                        default:
                            url = "http://" + url;
                            break;
                    }

                }
                Debug.Log(url);

                focused.GetComponent<UWKWebView>().LoadURL(url);
                last_input = url;
            }
        }
        addressBar.GetComponent<Canvas>().enabled = false;
        addressBar.GetComponentInChildren<InputField>().DeactivateInputField();
        addressBar.GetComponentInChildren<InputField>().enabled = false;
        focusWindow(focused);
    }
}