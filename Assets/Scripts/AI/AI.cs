using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : Character
{
    public AIRoutine routine;

    public static AI mainAI; //calling it main because there *might* be secondary AIs later on
    static public TaskProgressBarController progressBar;

    // Start is called before the first frame update
    void Start()
    {
        if (mainAI == null)        
        {
            mainAI = this;
            progressBar = this.transform.Find("TaskProgressBar").gameObject.GetComponent<TaskProgressBarController>();
        }
        else
        {
            Destroy(this.gameObject);
        }

        progressBar.SetBarVisibility(false);
    }
    void FixedUpdate()
    {
        if (movementCoroutine == null && activeTask == null)
            ProgressRoutine();
    }

    void ProgressRoutine()
    {
        AIRoutine.WayPoint nextWaypoint = routine.GetNextWaypoint();
        SetTask(nextWaypoint.triggerable);
        //the line bellow will throw errors if both triggerable and rawPosition are not set. Leaving it as it is for now untill I implement a better system to manage routines.
        PlanAndExecuteMovement(nextWaypoint.triggerable == null? nextWaypoint.rawPosition.position : nextWaypoint.triggerable.transform.position); 
    }

    protected override bool ProcessTrigger()
    {
        queuedTask.TriggerAI();
        activeTask = queuedTask;
        queuedTask = null;


        StartCoroutine(AIAction()); //TODO modify this line after implementing StartAITrigger() in Triggerable. 
        return true;
    }

    float timer = 0.0f;
    IEnumerator AIAction()
    {
        float taskTime = routine.GetCurrentWaypointActionDuration();
        //yield return new WaitForSeconds(routine.GetCurrentWaypointActionDuration());
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
        base.OnDrawGizmos();
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

    public int currentWaypoint = 0;

    public WayPoint GetNextWaypoint()
    {
        currentWaypoint++;
        if (currentWaypoint >= wayPoints.Length)
            currentWaypoint = 0;

        return wayPoints[currentWaypoint];
    }

    public float GetCurrentWaypointActionDuration()
    {
        return wayPoints[currentWaypoint].timeAtPoint;
    }
}