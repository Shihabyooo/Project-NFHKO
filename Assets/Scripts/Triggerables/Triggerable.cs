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

    public Triggerable[] preRequiredTriggers; //in non-customized Triggers, every object in this list will be tested for isTriggered. If any fails, player cannot trigger this Trigger.
    public uint[] requiredItems; //listed by itemIDs.

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

    protected virtual bool CheckPreRequisites() //returns true if all prerequisites are satistified.
                                                //advanced prerequisites (e.g. trigger has multiple options for trigger) can override this method with their own tests.
    {
        if (preRequiredTriggers != null && preRequiredTriggers.Length > 0)
        {
            //loop over prerequiesites and check whether they are allready triggered
            foreach (Triggerable trigger in preRequiredTriggers)
            {
                if (!trigger.isTriggered)
                    return false;
            }
        }

        //check items
        if (!CheckRequiredItems())
            return false;
        
        //if we reached here, this means everything is set up and we can trigger this object
        return true;
    }

    protected virtual bool CheckRequiredItems()
    {
        if (requiredItems != null && requiredItems.Length > 0 )
        {
            //TODO finish this after implementing inventory system
        }

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
