using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Region : MonoBehaviour
{
    public Vector3 cornerNE {get; private set;}
    public Vector3 cornerSW {get; private set;}
    public Vector3 cornerNW {get; private set;}
    public Vector3 cornerSE {get; private set;}
    public Vector3 localCentre;
    public Vector3 globalCentre {get; private set;}
    public float sizeX;
    public float sizeY;
    public Color gizmoColour = Color.red;
    
     void OnValidate()
    {
        UpdateCorners();
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        UpdateCorners(); //this is expensive, but otherwise Unity won't update corners in editors unless exposed values are modified.
        Gizmos.color = gizmoColour;
        Gizmos.DrawLine(cornerSW, cornerNW);
        Gizmos.DrawLine(cornerNW, cornerNE);
        Gizmos.DrawLine(cornerNE, cornerSE);
        Gizmos.DrawLine(cornerSE, cornerSW);
    }
#endif
    public void UpdateCorners()
    {
        globalCentre = this.transform.position + localCentre;
        cornerSW = new Vector3(globalCentre.x - (sizeX / 2.0f), globalCentre.y - (sizeY / 2.0f), globalCentre.z);
        cornerNW = new Vector3(globalCentre.x - (sizeX / 2.0f), globalCentre.y + (sizeY / 2.0f), globalCentre.z);
        cornerNE = new Vector3(globalCentre.x + (sizeX / 2.0f), globalCentre.y + (sizeY / 2.0f), globalCentre.z);
        cornerSE = new Vector3(globalCentre.x + (sizeX / 2.0f), globalCentre.y - (sizeY / 2.0f), globalCentre.z);
    }

    public void UpdateCorners (Vector3 overrideGlobalCentre)
    {
        globalCentre = overrideGlobalCentre;
        cornerSW = new Vector3(globalCentre.x - (sizeX / 2.0f), globalCentre.y - (sizeY / 2.0f), globalCentre.z);
        cornerNW = new Vector3(globalCentre.x - (sizeX / 2.0f), globalCentre.y + (sizeY / 2.0f), globalCentre.z);
        cornerNE = new Vector3(globalCentre.x + (sizeX / 2.0f), globalCentre.y + (sizeY / 2.0f), globalCentre.z);
        cornerSE = new Vector3(globalCentre.x + (sizeX / 2.0f), globalCentre.y - (sizeY / 2.0f), globalCentre.z);
    }

    public bool TestLocationInside(Vector3 location)
    {
        if (location.x < cornerNE.x &&
        location.x > cornerSW.x &&
        location.y < cornerNW.y &&
        location.y > cornerSW.y)
            return true;
        else
            return false;   
    }

    public void Average(Region region1, Region region2, float zLevel) //helpful mostly in Camera controls, where I have to compute the region at an arbitrary slice of a furstrum defined by two regions.
    {
        //simple linear interpolation using Equation of Slopes
        float ratio = (zLevel - region1.globalCentre.z) / (region2.globalCentre.z - region1.globalCentre.z); //since this part is common between all three equations, let's cache to reuse!
        
        sizeX = ratio * (region2.sizeX - region1.sizeX) + region1.sizeX;
        sizeY = ratio * (region2.sizeY - region1.sizeY) + region1.sizeY;

        globalCentre = ratio * (region2.globalCentre - region1.globalCentre) + region1.globalCentre; //since scalar multiplication, vector additions and vector subtractions work on each member of a vector, we can save time and do the interpolation on the entire vector.
        UpdateCorners(globalCentre);
    }
}
