﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(PathFinder))] //pathfinder is moved to be a universal, GameManager component
public class Player : MonoBehaviour
{
    static public Player player;
    static public TaskProgressBarController progressBar;
    public enum PlayerState {moving, idle, onStairs, inAction}
    public PlayerState currentState;
    public NavigationNode currentNode;
    public float speed = 1.0f;

    Vector3 movementTarget;
    Vector3 ultimateMovementTarget;
    //bool isMoving = false;

    List<NavigationNode> pathPoints;
    Triggerable queuedTask = null;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            {
                player = this;
                progressBar = this.transform.Find("TaskProgressBar").gameObject.GetComponent<TaskProgressBarController>();
            }
        else
            Destroy(this.gameObject);

        progressBar.SetBarVisibility(false);
        currentState = PlayerState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        //Move();
    }

    public bool PlanAndExecuteMovement(Vector3 target)
    {
        if (isOnStairs) //don't want to interrupt stairs climbing/descending
            return false;

       //print ("Moving");

        Path path = GameManager.pathFinder.CalculatePath(Player.player.currentNode, target);
        
        if (path.nodesChain == null)
        {
            print ("Could not plot a path to target location");
            return false;   
        }

        if (movementCoroutine != null)
            {
                //StopCoroutine(movementCoroutine);
                StopAllCoroutines(); //since I have a corouting called from within movementCoroutine.
                movementCoroutine = null;
            }

        pathPoints = path.nodesChain;
        ultimateMovementTarget = path.exactPos;
        //need to figure a away to queue actions at the end of movement as well.
        //a bool (actionQueuedAfterMovement?) coupled with a reference to specialized "Interactable" class which we call the "Trigger" method of if actionQueuedAfterMovement == true, perhaps?

        //print ("Starting movement coroutine");
        movementCoroutine = StartCoroutine(Movement());
        return true;
    }

    Coroutine movementCoroutine = null;
    bool isOnStairs = false;
    IEnumerator Movement()
    {

        Vector3 currentTarget;
        bool hasArrived = false;
        if (pathPoints.Count > 1)
        {
            currentTarget = pathPoints[pathPoints.Count - 1].transform.position;
            //print ("Setting target to position of node " + pathPoints[pathPoints.Count - 1].name);
        }
        else
            currentTarget = ultimateMovementTarget;

        while (!hasArrived)
        {
            Vector3 pos = this.transform.position;
            float direction = currentTarget.x - pos.x;
            direction = direction / Mathf.Abs(direction); //normalize direction
            
            pos.x += direction * speed * Time.deltaTime;

            this.transform.position = pos;

            //if (Mathf.Abs(pos.x - currentTarget.x) <= 0.005f)
            if (Mathf.Sign(direction) * (pos.x - currentTarget.x) > 0.0f) //this actually translates to "we've passed the destination," but its better than the previous check.
            {                
                if (pathPoints.Count > 0) //remember that PathFinder can send a path with an empty List<NavigationPoints>, if we don't do this check, will trigger OOB write.
                    {
                        if (pathPoints[pathPoints.Count - 1].nodeType == NavigationNode.NodeType.stairs)
                        {
                            isOnStairs = true;
                            StartCoroutine(ClimbStairs());

                            float helperTimer = 0.0f; //test
                            while (isOnStairs)
                            {
                                yield return new WaitForEndOfFrame();
                                helperTimer += Time.deltaTime;
                                if (helperTimer > 5.0f) //test, infinite loop guard
                                {
                                    print ("Warning! Waited too long for stairs coroutine");
                                    break;
                                }
                            }
                        }
                        else
                        {
                            currentNode = pathPoints[pathPoints.Count -1];
                            pathPoints.RemoveAt(pathPoints.Count - 1);
                        }
                    }

                if (pathPoints.Count > 1)
                {
                    currentTarget = pathPoints[pathPoints.Count - 1].transform.position;
                    //print ("Setting target to position of node " + pathPoints[pathPoints.Count - 1].name);
                }
                else if (pathPoints.Count == 1)
                {
                    currentTarget = ultimateMovementTarget;
                    //print ("Reached room, heading to exact position in room");
                }
                else
                    {
                        //print ("Arrived at destination");
                        hasArrived = true;
                    }
            }
            yield return new WaitForEndOfFrame();
        }
        if (queuedTask != null)
            ProcessTrigger();

        yield return movementCoroutine = null;
    }

    IEnumerator ClimbStairs()
    {
        //What's bellow is a testing implementation.

        //print ("started ascending/descending stairs");
        yield return new WaitForSeconds(1.0f);
         //there is an assumption here that stairs always come in -at least- pairs, and due to the fact that paths end in rooms, this means the first stair hit always ends up 
         //no less than 2 elements from the end of the path (worst case: stairBegin --> stairEnd --> targetRoom).
         
        //print("from " + pathPoints[pathPoints.Count - 1].gameObject.name + " to " + pathPoints[pathPoints.Count - 2].gameObject.name );

        float yDelta = pathPoints[pathPoints.Count - 1].transform.position.y - pathPoints[pathPoints.Count - 2].transform.position.y;
         
        Vector3 teleportPos =  new Vector3(
            pathPoints[pathPoints.Count - 2].transform.position.x, 
            this.transform.position.y - yDelta,
            0.0f
         );
        //print ("Teleport Target: " + teleportPos);
        this.gameObject.transform.position = teleportPos; //teleport to other end of stairs.
        
        pathPoints.RemoveAt(pathPoints.Count - 1); //remove first stair node
        pathPoints.RemoveAt(pathPoints.Count - 1); //remote the two stair nodes

        yield return new WaitForSeconds(1.0f);
        currentNode = pathPoints[pathPoints.Count -1];
        //print ("finished ascending/descending stairs");
        //print ("Next node: " + pathPoints[pathPoints.Count - 1].gameObject.name);
        yield return isOnStairs = false;
    }

    public void SetTask(Triggerable task)
    {
        //print ("Called SetTask() with " + (task == null? "null" : task.gameObject.name)); //test
        if (task != null)
            CancelTask();
        
        queuedTask = task;
    }

    void CancelTask()
    {
        //print ("Called CancelTask()");
        if (queuedTask != null)
            queuedTask.Cancel();        

        queuedTask= null;
    }

    bool ProcessTrigger()
    {
        bool result = queuedTask.Trigger();
        queuedTask = null;
        return result;
    }

    void OnDrawGizmos()
    {
        if (movementCoroutine != null)
        {
            Gizmos.color = Color.red;
            if (pathPoints.Count > 0)
            {
                Gizmos.DrawLine(pathPoints[pathPoints.Count - 1].transform.position, this.transform.position);
                Gizmos.DrawLine(pathPoints[0].transform.position, ultimateMovementTarget);
            }
            
            if (pathPoints.Count > 1)
            {
                for (int i = 0; i < pathPoints.Count - 1; i++)
                {
                    Gizmos.DrawLine(pathPoints[i].transform.position, pathPoints[i+1].transform.position);
                }
            }

            Gizmos.DrawSphere(ultimateMovementTarget, 0.5f);
        }
    }
}