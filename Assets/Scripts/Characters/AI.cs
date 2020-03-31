using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Character
{
    public AIRoutine routine;

    public static AI mainAI; //calling it main because there *might* be secondary AIs later on

    public override void Awake()
    {
        if (mainAI == null)        
            mainAI = this;
        else
            Destroy(this.gameObject);

        base.Awake();
    }

    public override void CustomStart()
    {
        routine.InitializeRoutine();
        base.CustomStart();
    }

    public override void FixedUpdate()
    {
        if (GameManager.gameMan.isGameplayActive && movementCoroutine == null && activeTask == null)
            ProgressRoutine();

        base.FixedUpdate();
    }

    void ProgressRoutine()
    {
        AIRoutine.WayPoint nextWaypoint = routine.GetNextWaypoint();
        SetTask(nextWaypoint.triggerable);
        //the line bellow will throw errors if both triggerable and rawPosition are not set. Leaving it as it is for now until I implement a better system to manage routines.
        PlanAndExecuteMovement(nextWaypoint.triggerable == null? nextWaypoint.rawPosition.position : nextWaypoint.triggerable.transform.position); 
    }

    protected override bool ProcessTrigger()
    {
        activeTask = queuedTask;
        queuedTask = null;
        activeTask.TriggerAI();
        
        return true;
    }

    public void NormalAIAction()
    {
        StartCoroutine(AIAction()); //TODO modify this line after implementing StartAITrigger() in Triggerable. 
    }
    
    public void ActivatePrankResponse()
    {
        print("Activating prank response"); //test
        //TODO remember to:
            //make accommodation for animation triggers (future)
            //check whether there are any triggerables remaining in routine (use routine.noOfTriggerableWaypoints);
            //clear active tasks, so the routine can progress.

            routine.RemoveWayPointFromRoutine(routine.currentWaypoint);
            CheckRemainingRoutineTasks();
            ClearActiveTask();
    }

    void CheckRemainingRoutineTasks()
    {
        if (routine.noOfTriggerableWaypoints < 1)
        {
            print ("No more tasks left for AI!");
        }
    }

    float timer = 0.0f;
    IEnumerator AIAction()
    {
        float taskTime = routine.GetCurrentWaypointActionDuration();

        while (timer < taskTime)
        {
            yield return new WaitForEndOfFrame();
            progressBar.SetProgress(timer / taskTime);
            timer += Time.deltaTime;
        }
        
        timer = 0.0f;
        
        progressBar.SetBarVisibility(false);
        yield return activeTask = null;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos(); //draw nav path from Character.cs.
        
        //draw links between waypoints.
        Gizmos.color = new Color(0.5f, 0.75f, 0.0f, 1.0f);
        if (routine != null && routine.wayPoints != null && routine.wayPoints.Length > 1)
        {
            for (int i = 0; i < routine.wayPoints.Length; i++)
            {
                int iNext = i == routine.wayPoints.Length - 1? 0 : i + 1;   //basically, when we reach the last elemen of the array, the second point to draw the line to would be the first 
                                                                            //element of the array, drawing a looped path.
                //Yes, the two lines bellow are ugly, but until I figure out a better way to do the routine system, the only alternative are even uglier nested if-statements.
                Vector3 point_1 = routine.wayPoints[i].triggerable == null? (routine.wayPoints[i].rawPosition == null? Vector3.zero : routine.wayPoints[i].rawPosition.position) : routine.wayPoints[i].triggerable.transform.position;
                Vector3 point_2 = routine.wayPoints[iNext].triggerable == null? (routine.wayPoints[iNext].rawPosition == null? Vector3.zero : routine.wayPoints[iNext].rawPosition.position) : routine.wayPoints[iNext].triggerable.transform.position;

                Debug.DrawLine(point_1, point_2);
            }
        }
    }
    
    void OnGUI()
    {
        //test
        //list routine waypoints on GUI
        float rectWidth = 400.0f;
        float xPosition = Screen.width - rectWidth;
        
        Rect rect = new Rect(xPosition, 10, rectWidth, 30);

        string line = "<size=20> --- AI ROUTINE --- </size>";
        GUI.Label(rect, line);
        
        for (int i = 0; i < routine.wayPoints.Length; i++)
        {
            AIRoutine.WayPoint waypoint = routine.wayPoints[i];
            rect.y += rect.height + 5;
            
            if (waypoint.triggerable != null)
                line = "<size=15> Triggerable: " + waypoint.triggerable.gameObject.name + ".</size>";
            else if (waypoint.rawPosition != null)
                line = "<size=15> RawPosition: " + waypoint.rawPosition.gameObject.name + ".</size>";
            else
                line = "<size=15> --Empty waypoint-- </size>";

            GUI.Label(rect, line);
        }
    }

}

[System.Serializable]
public class AIRoutine
{
    [System.Serializable]
    public struct WayPoint //a waypoint can hold both a transform and a triggerable, but only one will be used. Priority will given to triggerable, if it is null, then the point position.
    {   public Triggerable triggerable;
        public Transform rawPosition; //only matters if triggerable == null.
        public float timeAtPoint;
    }

    public WayPoint[] wayPoints;

    public int currentWaypoint {get; private set;}
    public int noOfTriggerableWaypoints {get; private set;}

    public void InitializeRoutine()
    {
        currentWaypoint = 0;
        noOfTriggerableWaypoints = GetNumberOfTriggerableWaypoints();
    }

    public WayPoint GetNextWaypoint()
    {
        int counter = 0;
        while (counter < wayPoints.Length)
        {
            currentWaypoint++;
            if (currentWaypoint >= wayPoints.Length)
                currentWaypoint = 0;
            
            WayPoint wayPoint = wayPoints[currentWaypoint];

            if (wayPoint.triggerable != null || wayPoint.rawPosition != null)
                return wayPoints[currentWaypoint];
            else
                counter++;
        }

        MonoBehaviour.print ("WANRING! No next set waypoint is found for AI routine"); //test
        return new WayPoint();
    }

    public float GetCurrentWaypointActionDuration()
    {
        return wayPoints[currentWaypoint].timeAtPoint;
    }

    public void RemoveWayPointFromRoutine(int wayPointID)
    {
        if (wayPoints[wayPointID].triggerable != null)
            noOfTriggerableWaypoints--;

        //setting the content of the waypoints to null is probably faster than clearing
        wayPoints[wayPointID].triggerable = null;
        wayPoints[wayPointID].rawPosition = null;
    }

    int GetNumberOfTriggerableWaypoints()
    {
        int counter = 0;

        foreach (WayPoint wayPoint in wayPoints)
        {
            if (wayPoint.triggerable != null)
                counter++;
        }

        if (counter == 0)
            MonoBehaviour.print("WARNING! No triggerable behaviour is set in AI routine"); //test

        return counter;
    }

}