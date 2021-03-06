﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(NavigationNode))]
public class Room : Region
{
    BoxCollider roomCollider;
    public uint roomID; 
    Color gizmoColor = new Color(0.0f,1.0f,1.0f,1.0f);

    void Awake()
    {
        roomCollider = this.gameObject.GetComponent<BoxCollider>();
        roomCollider.center = localCentre;
        roomCollider.size = new Vector3(sizeX, sizeY, 1.0f);
    }
}
