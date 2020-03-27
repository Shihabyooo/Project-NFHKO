using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public static CameraControl mainCam;
    public Region minZoomRegion;
    public Region maxZoomRegion;
    Region regionCurrent;
    public float zoomSpeed = 65.0f; 
    public float cameraSpeed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (mainCam == null)
            mainCam = this;
        else
            Destroy(this.gameObject);

        regionCurrent = this.gameObject.GetComponent<Region>();
    }

    public void Zoom(float zoomRate)
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

    public void Move(Vector3 direction)
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