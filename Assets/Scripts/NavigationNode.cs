using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationNode : MonoBehaviour
{
    public enum NodeType {door, stairs, room};
    public NodeType nodeType;

    public uint nodeID;

    public NavigationNode[] adjacentNodes;

     void OnDrawGizmos()
    {

        if (adjacentNodes.Length < 1)
            return;
        foreach (NavigationNode node in adjacentNodes)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, node.transform.position);
        }
    }

}
