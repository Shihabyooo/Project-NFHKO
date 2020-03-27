using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public LayerMask clickableLayers;
 
    public float screenMovementThreshold = 10.0f; //the distance (in pixels) from the screen edges at which camera begins to move if mouse cursor reached.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    Vector3 lastClick = new Vector3(100.0f, 100.0f, 100.0f); //test
    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(lastClick, 0.5f);
    }

    float holdTimerMouseButton0 = 0.0f;
    float holdTimerMouseButton1 = 0.0f;
    public float minLeftClickHoldTimeToMove = 0.025f;
    public float maxLeftClickHoldTimeToMove = 0.5f;
    public float minRightClickHoldTimeView = 0.025f; //for future use of secondary action using right click.
    public float maxRightClickHoldTimeView = 0.5f; //for future use of secondary action using right click.
    //TODO I'm making the assumption here that the future use for right click is a "view" action, if that isn't the case, change the name of the two variables above.
    Vector3 lastRightClickPosition; //for use when moving camera by dragging.

    void ProcessInput()
    {
        //lock mouse cursor to game window
        Cursor.lockState = CursorLockMode.Confined;

        //update timers
        holdTimerMouseButton0 += Time.deltaTime;
        holdTimerMouseButton1 += Time.deltaTime;

        //Left Click tests
        if(Input.GetMouseButtonDown(0))
        {
           holdTimerMouseButton0 = 0.0f;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (holdTimerMouseButton0 >= minLeftClickHoldTimeToMove && holdTimerMouseButton0 <= maxLeftClickHoldTimeToMove)
            {
                ProcessNormalLeftClick();
            }
        }
    

        //right click tests
        if (Input.GetMouseButtonDown(1))
        {
            lastRightClickPosition = Input.mousePosition;
            holdTimerMouseButton1 = 0.0f;
        }
        else if (Input.GetMouseButton(1))
        {
            if (holdTimerMouseButton1 > maxRightClickHoldTimeView)
            {
                ProcessCameraDrag();
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (holdTimerMouseButton1 >= minRightClickHoldTimeView && holdTimerMouseButton1 <= maxRightClickHoldTimeView)
            {
                //process normal secondary action
            }
        }

        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))   //zooming and moving by position mouse at screen edge only should work if no button is pressed down, else
        {                                                           //it would conflict with moving by right click hold...
            //process zoom
            ProcessScrollZoom();

            //process camera movement
            ProcessScreenEdgeMovement();
        }
    }

    void ProcessNormalLeftClick()
    {
        Vector3 modMousePos = Input.mousePosition;
        modMousePos.z = 1.0f;
        Vector3 rawClickPos = Camera.main.ScreenToWorldPoint(modMousePos);
            
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 clickDir = rawClickPos - cameraPos;
        Ray ray = new Ray (cameraPos, clickDir);
        RaycastHit hit;
            
        if (Physics.Raycast(ray, out hit, 1000.0f, clickableLayers))
        {
            //print ("Hit Name: " + hit.collider.gameObject.name);
            if (hit.collider.gameObject.GetComponent<Triggerable>() != null && !hit.collider.gameObject.GetComponent<Triggerable>().isTriggered)
                Player.player.SetTask(hit.collider.gameObject.GetComponent<Triggerable>());
            else
                Player.player.SetTask(null);

            Player.player.PlanAndExecuteMovement(hit.point);
        }
    }

    void ProcessCameraDrag()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 direction = mousePos - lastRightClickPosition;

        //to have the movement speed varrying with the distance from the right click, we custom-normalize the vector by -a percentage of- half the width/height of the screen.
        float deadZonePercent = 0.025f; //the region where any movement smaller than which would not cause camera movement (i.e. minimum radius).
        float limitZonePercent = 0.5f; //the limit after which movement will be at constant (max) speed regardless of how farther the mouse goes (i.e. maximum radius).

        float halfWidth = Screen.width / 2.0f;
        float halfHeight = Screen.height / 2.0f;

        //check that we are outside deadzone else zero out the direction. For X-axis:
        if (direction.x > deadZonePercent * halfWidth || direction.x < -1.0f * deadZonePercent * halfWidth)
                direction.x = Mathf.Clamp( direction.x / (limitZonePercent * halfWidth), -1.0f, 1.0f);  //normalize direction
        else
            direction.x = 0.0f;
        
        //For Y-axis:
        if (direction.y > deadZonePercent * halfHeight || direction.y < -1.0f * deadZonePercent * halfHeight)
            direction.y = Mathf.Clamp( direction.y / (limitZonePercent * halfHeight), -1.0f, 1.0f); //normalize direction
        else
            direction.y = 0.0f;

        CameraControl.mainCam.Move(direction);
        //print (direction);//test
    }

    void ProcessScrollZoom()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta > 0.1f || scrollDelta < -0.1f)
        {
            scrollDelta = scrollDelta / Mathf.Abs(scrollDelta); //normalize the value
            CameraControl.mainCam.Zoom(scrollDelta);
        }
    }

    void ProcessScreenEdgeMovement()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 screenRes;
        if (Screen.fullScreen) //TODO research whether Screen.width and Screen.height return the correct values regardless of windowed/fullscreen mode. 
        {
            screenRes = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        }
        else
        {
            screenRes = new Vector2(Screen.width, Screen.height);
        }
        
        if (mousePos.x > screenRes.x - screenMovementThreshold ||
            mousePos.x < screenMovementThreshold ||
            mousePos.y > screenRes.y - screenMovementThreshold ||
            mousePos.y < screenMovementThreshold
            )
        {
            Vector3 screenCentre = new Vector3(screenRes.x / 2.0f, screenRes.y / 2.0f, 0.0f);
            Vector3 direction = mousePos - screenCentre;
            direction.z = 0.0f;
            direction.Normalize();
            CameraControl.mainCam.Move(direction);
            Debug.DrawRay(this.transform.position, direction, Color.blue); //test
        }
    }

}
