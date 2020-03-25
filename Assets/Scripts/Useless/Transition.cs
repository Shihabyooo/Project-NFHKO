using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(NavigationNode))]
public class Transition : MonoBehaviour
{
    // Start is called before the first frame update
  public uint transitionID;
  public Room[] linkedRooms;
}
