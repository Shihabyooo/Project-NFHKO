using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    public Region minZoomRegion;
    public Region maxZoomRegion;
    Region regionCurrent;
    public float zoomSpeed = 65.0f; 
    public float cameraSpeed = 5.0f;
    public float screenMovementThreshold = 10.0f; //the distance (in pixels) from the screen edges at which camera begins to move if mouse cursor reached.

    // Start is called before the first frame update
    void Start()
    {
        regionCurrent = this.gameObject.GetComponent<Region>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    void ProcessInput() //TODO move this to MouseInput class.
    {   
        //lock mouse cursor to game window
        Cursor.lockState = CursorLockMode.Confined;

        //process zoom
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta > 0.1f || scrollDelta < -0.1f)
        {
            scrollDelta = scrollDelta / Mathf.Abs(scrollDelta); //normalize the value
            Zoom(scrollDelta);
        }

        //process camera movement
        Vector3 mousePos = Input.mousePosition;
        Vector2 screenRes;
        if (Screen.fullScreen)
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
                Move(direction);
                Debug.DrawRay(this.transform.position, direction, Color.blue); //test
           }
    }

    void Zoom(float zoomRate)
    {
        float zLevel = this.transform.position.z;
        zLevel += zoomRate * zoomSpeed * Time.deltaTime;
        zLevel = Mathf.Clamp (zLevel, minZoomRegion.transform.position.z, maxZoomRegion.transform.position.z);

        Vector3 newPos = this.transform.position;
        newPos.z = zLevel;

        this.transform.position = newPos;
        Move(Vector3.zero); //Move() function has an OOB check and clamps camera location to within the regionCurrent. Since zooming out reduces the currentRegion, we call with
                            //a zero-valued Vector to force clamping. This avoids an unpleasant jump that would happen if one zooms out with camera at edge of region, then moves.
    }

    void Move(Vector3 direction)
    {
        Vector3 newPos = this.transform.position;
        newPos += direction * cameraSpeed * Time.deltaTime;

        //compute the region based on the current zLevel from the min/maxZoomRegion
        regionCurrent.Average(maxZoomRegion, minZoomRegion, this.transform.position.z);
        newPos.x = Mathf.Clamp(newPos.x, regionCurrent.cornerSW.x, regionCurrent.cornerNE.x);
        newPos.y = Mathf.Clamp(newPos.y, regionCurrent.cornerSW.y, regionCurrent.cornerNE.y);
        
        this.transform.position = newPos;
    }

}