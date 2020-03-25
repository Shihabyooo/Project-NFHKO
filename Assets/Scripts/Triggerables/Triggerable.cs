using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//each trigger has to aspects of condsiderations: 
//      1- pre-requirements, which could be other triggerables (with states saved in a global parameter), or items used (drag and drop? select then select?) or nothing.
//      2- triggering time, which can take near instantaniously (item pickup), or take moderate or long time (to add difficulty, must time with enemy routine).

public class Triggerable : MonoBehaviour
{
    public float triggerTime = 5.0f;
    public bool isTriggered = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual bool Trigger()
    {
        if (isTriggered)
            return false;
        else if (pTriggerProcessCoroutine != null)
            Cancel();

        StartPlayerTrigger();
        return true;
    }

    public virtual void Cancel()
    {
        //print ("Called Cancel()");
        StopAllCoroutines();
        pTriggerProcessCoroutine = null;
    }

    protected virtual void StartPlayerTrigger()
    {
        //print ("Inside StartPlayerTrigger()");
        pTriggerProcessCoroutine = StartCoroutine(PlayerTriggerProcess());
    } 

    Coroutine pTriggerProcessCoroutine = null;
    protected virtual IEnumerator PlayerTriggerProcess()
    {
        //print ("Started PlayerTriggerProcess");
        float timeSinceStart = 0.0f;

        while (timeSinceStart < triggerTime)
        {
            yield return new WaitForEndOfFrame();
            timeSinceStart += Time.deltaTime;
            
            Player.progressBar.SetProgress(timeSinceStart/triggerTime);
        }

        Player.progressBar.SetBarVisibility(false);
        //print ("Finished PlayerTriggerProcess");
        yield return isTriggered = true;

    }

}
