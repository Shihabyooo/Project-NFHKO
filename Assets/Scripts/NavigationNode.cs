using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationNode : MonoBehaviour
{
    public enum NodeType {door, stairs, room};
    public NodeType nodeType;

    public uint nodeID;

    public NavigationNode[] adjacentNodes;

    void Awake()
    {
        //This code bellow is due to check and workaround mistakes when setting node relations in editor. e.g. forgot to set adjacent node or 
        //deleted one without readjusting adjacent nodes no.)
        int counter = 0;

        foreach (NavigationNode node in adjacentNodes)
        {
            if (node != null)
                counter++;
        }
        
        if (counter != adjacentNodes.Length)
        {
            NavigationNode[] tempNodesHolder = new NavigationNode[counter];
            
            counter = 0;
            foreach(NavigationNode node in adjacentNodes)
            {
                if (node != null)
                {
                    tempNodesHolder[counter] = node;
                    counter++;
                }
            }

            adjacentNodes = tempNodesHolder;
        }
    }

     void OnDrawGizmos()
    {

        if (adjacentNodes.Length < 1)
            return;
        foreach (NavigationNode node in adjacentNodes)
        {
            Gizmos.color = Color.green;
            if (node != null)
                Gizmos.DrawLine(this.transform.position, node.transform.position);
        }
    }

}
