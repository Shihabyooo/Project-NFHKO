using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(NavigationNode))]
public class Room : Region
{
    
    
    //public Transition[] transitions; //TODO change this to a node based solution abstracting transitions and rooms.

    BoxCollider collider;
   public uint roomID; 
   Color gizmoColor = new Color(0.0f,1.0f,1.0f,1.0f);

   
    void Start()
    {
        collider = this.gameObject.GetComponent<BoxCollider>();
        collider.center = localCentre;
        collider.size = new Vector3(sizeX, sizeY, 1.0f);
    }
}
