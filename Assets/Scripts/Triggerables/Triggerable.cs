using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//each trigger has to aspects of condsiderations: 
//      1- pre-requirements, which could be other triggerables (with states saved in a global parameter), or items used (drag and drop? select then select?) or nothing.
//      2- triggering time, which can take near instantaniously (item pickup), or take moderate or long time (to add difficulty, must time with enemy routine).


public class Triggerable : MonoBehaviour
{
    public enum TriggerableState {untriggered, playerTriggered, aiTriggered};

    public TriggerableState state {get; private set;}

    public float triggerTime = 5.0f;
    public bool isTriggered = false; //TODO remove uses of this bool, switch to using TriggerableState state.

    public Triggerable[] preRequiredTriggers; //in non-customized Triggers, every object in this list will be tested for isTriggered. If any fails, player cannot trigger this Trigger.
    public ItemSlot[] requiredItems; 
    public TriggerableScript script;

    void Awake()
    {
        state = TriggerableState.untriggered;
    }

    public virtual bool Trigger()
    {
        if (isTriggered)
            return false;
        else if (pTriggerProcessCoroutine != null)
            Cancel();

        if (!CheckPreRequisites())
            return false;

        StartPlayerTrigger();
        return true;
    }

    public virtual void TriggerAI()
    {
        StartAITrigger();
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
            foreach (ItemSlot item in requiredItems)
            {
                if (!GameManager.inventory.CheckItem(item))
                {
                    return false;
                }
            }
        }
        
        return true;
    }

    public virtual void Cancel()
    {
        //print ("Called Cancel()");
        StopAllCoroutines();
        pTriggerProcessCoroutine = null;
        Player.player.progressBar.SetBarVisibility(false);
    }

    protected virtual void StartPlayerTrigger()
    {
        //print ("Inside StartPlayerTrigger()");
        pTriggerProcessCoroutine = StartCoroutine(PlayerTriggerProcess());
    } 

    protected virtual void StartAITrigger()
    {
        //check if triggered by player
        if (isTriggered)
        {
            AI.mainAI.ActivatePrankResponse(); //TODO when implementing prank animation, which should be controlled by a coroutine, move this call to the end of coroutine.
            SwitchState(TriggerableState.aiTriggered);
        }
        else
        {
            AI.mainAI.NormalAIAction();
        }
    }

    protected Coroutine pTriggerProcessCoroutine = null;
    protected virtual IEnumerator PlayerTriggerProcess()
    {
        //print ("Started PlayerTriggerProcess");
        float timeSinceStart = 0.0f;

        while (timeSinceStart < triggerTime)
        {
            yield return new WaitForEndOfFrame();
            timeSinceStart += Time.deltaTime;
            
            Player.player.progressBar.SetProgress(timeSinceStart/triggerTime);
        }

        Player.player.progressBar.SetBarVisibility(false);
        Player.player.ClearActiveTask();
        
        //print ("Finished PlayerTriggerProcess");
        SwitchState(TriggerableState.playerTriggered);
        isTriggered = true;
        RemoveRquiredItems();
        yield return null;
    }

    public virtual void SwitchState(TriggerableState newstate)
    {
        state = newstate;
    }

    protected virtual void RemoveRquiredItems()
    {
        if (requiredItems.Length < 1)
            return;

        foreach(ItemSlot item in requiredItems)
        {
            GameManager.inventory.RemoveItem(item);
        }
    }

}

[System.Serializable]
public class TriggerableScript //and when I say "Script," I mean so in a narrative way. This is a container for text/messages related to the trigger
{
    [TextArea] public string inspectionMessageUntriggered;
    [TextArea] public string duringTriggeringMessage;
    [TextArea] public string inspectionMessageTriggeredPlayer;
    [TextArea] public string inspectionMessageTriggeredAI;
    [TextArea] public string hint1;
    [TextArea] public string hint2;
}