﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public Transform[] nodesContainers;

    [SerializeField] NavigationNode[] sceneNodes;

    // Start is called before the first frame update
    void Start()
    {
        //I make the assumption (for ease of organizing things in the Unity editor) that nodes will be organized in multiple objects. e.g. doors in a Transitions transform, rooms in a Room transform.
        //So, we expose (to the editor) an array of Transforms to assign those parent transforms to, then we loop through first the parent transforms, then the childern of each parent.
        //Assuming C++ logic applies to C#, arrays have better performance than stl containers, so I'll only use the list in initialization and retain the frequently accessed sceneNodes as array.
        
        //The tempNodeHolder list will be used to store all transforms with NavigationNodes component in one place that we then copy to our sceneNodes array.
        List<Transform> tempNodeHolder = new List<Transform>();

        foreach (Transform container in nodesContainers) //loop through parent transforms.
        {
            foreach (Transform nodeTransform in container) //loop through childern transforms of current parent transform.
            {
                if (nodeTransform.gameObject.GetComponent<NavigationNode>() != null)    //just for safety, we first check that the child has a NavigationNode component.
                    tempNodeHolder.Add(nodeTransform); //add to our buffer list.
            }
        }

        sceneNodes = new NavigationNode[tempNodeHolder.Count]; //now that we know how many nodes we have, we can set our array's size and init it.
        
        for (uint i = 0; i < tempNodeHolder.Count; i++)
        {
            sceneNodes[i] = tempNodeHolder[(int)i].gameObject.GetComponent<NavigationNode>();
            sceneNodes[i].nodeID = i;
        }
    }

    public Path CalculatePath(NavigationNode currentNode, Vector3 targetPos, NavigationNode targetNode = null)
    {
        Path path = new Path();

        //first we figure out which node targetPos is at (if we didn't recieve it with the function call, i.e = null)
        if (targetNode == null)
            targetNode = FindNodeFromPosition(targetPos);

        //then we just use the recursive function FindPath to do what its name says...
        //but before that, we have to check whether we need to do so. If the target node is the same as the current node, we don't need to find a path through rooms but
        //head directly to target location since we are already in its room.
        if (currentNode.nodeID != targetNode.nodeID)
        {    
            //print ("calling FindPath with start " + currentNode.gameObject.name + " and target: " + targetNode.gameObject.name);
            List<NavigationNode> nodeChain = FindPath(currentNode, targetNode);
            // //test
            // print ("Pathfinder result: " + nodeChain.Count);
            // foreach (NavigationNode node in nodeChain)
            //     print (node.gameObject.name);
            // //end test
            nodeChain.RemoveAt(nodeChain.Count - 1); //last element is the node we are currently at, no need to keep it.
            //we fillout our path object.
            path.nodesChain = nodeChain;
        }
        else
        {
            path.nodesChain = new List<NavigationNode>();
        }
        
        //in both cases, the targetPos it the same
        path.exactPos = targetPos;

        return path;
    }

    List<NavigationNode> FindPath(NavigationNode start, NavigationNode end)
    {
        return FindPath(start, end, start, -1);
    }

    List<NavigationNode> FindPath(NavigationNode start, NavigationNode end, NavigationNode previousNode, int recurssionLevel)
    {
        int currentRecurssionLevel = recurssionLevel + 1; //infinite loop guard.
        if (currentRecurssionLevel > sceneNodes.Length) 
            {
                //print("Reached max traversal range" + recurssionLevel ); //test
                return null;
            }
        
        int results = 0;
        List<NavigationNode>[] resultPathsAtNode = new List<NavigationNode>[start.adjacentNodes.Length];    //there are max of n potential solutions per node; n = #of adjacent nodes.

        foreach (NavigationNode node in start.adjacentNodes)
        {
            List<NavigationNode> subPath = new List<NavigationNode>();
            
            if (node == end) //reached target
            {
                subPath.Add(node);
                subPath.Add(start);
                return subPath;
            }
            else if (currentRecurssionLevel > 0 && start.adjacentNodes.Length == 1) //a dead end, only one adjacent node which is the one we came from. The first argument tests that
            {                                                                       //we aren't at start point, else algorithm would fail if we started from a room with one exit.
                return null;
            }
            else if (node.nodeID != previousNode.nodeID)
            {
                subPath = FindPath(node, end, start, currentRecurssionLevel);
                if (subPath != null) //we have a solution for the current subnote, we add it to the solutions list (for current recussion-level) and increment results count.
                {   
                    subPath.Add(start);
                    resultPathsAtNode[results] = subPath;
                    results ++;
                }
            }
        }

        if (results < 1) //the loop above didn't return any valid path for all sub-nodes
        {
            return null;
        }
        else if (results == 1) //we have only one solution
        {
            return resultPathsAtNode[0];
        }
        else    //we have multiple solutions, so we compared to find the shortest one
        {       //TODO modify the test bellow to acount for distance between nodes as well (more computational cost?)
            List<NavigationNode> shortestPath = resultPathsAtNode[0];

            for (int i = 1; i < results; i ++)
            {
                if (resultPathsAtNode[i].Count < shortestPath.Count)
                shortestPath = resultPathsAtNode[i];
            }
            return shortestPath;
        }
    }

    public NavigationNode FindNodeFromPosition(Vector3 position)
    {
        foreach (NavigationNode node in sceneNodes)
        {
            if (node.nodeType == NavigationNode.NodeType.room)
            {
                Room room = node.gameObject.GetComponent<Room>();
                if (room.TestLocationInside(position))
                {
                    return node;
                }
            }
        }

        print ("WARNING! Could not find a node matching the provided position");
        return null;
    }
}

public class Path
{
    public Vector3 exactPos;
    public List<NavigationNode> nodesChain;

    public Path()
    {
        nodesChain = new List<NavigationNode>();
    }
}

