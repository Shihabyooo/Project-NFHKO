using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO consider having a TriggerableContainer class (inheriting from Triggerable) that you use as base for objects such as this.

public class TriggerableContainer : Triggerable
{
    public ItemSlot containedItem;

    public override bool Trigger()
    {
        if (isTriggered)
            return false;
        else if (pTriggerProcessCoroutine != null)
            Cancel();

        StartPlayerTrigger();
        return true;
    }

    //This coroutine is mostly similar to the base one, I copied it because the last part (where we set isTriggered = true) done conditionally and it would be problematic if we
    //called the base.coroutine from here and reset it to false after returning (consider what would happen if we cancelled action just after end of base and before resetting).
    protected override IEnumerator PlayerTriggerProcess()   
    {                                                       
        float timeSinceStart = 0.0f;

        while (timeSinceStart < triggerTime)
        {
            yield return new WaitForEndOfFrame();
            timeSinceStart += Time.deltaTime;
            
            Player.progressBar.SetProgress(timeSinceStart/triggerTime);
        }

        Player.progressBar.SetBarVisibility(false);
        Player.player.ClearActiveTask();

        if (GameManager.inventory.AddItem(containedItem)) //AddItem() returns true if we succeeded in adding the item.
            {
                //print ("SUCCEEDED IN ADDING ITEM");
                yield return isTriggered = true;
            }
        else //if adding the item failed (no inventory space remaining), we simply return without setting isTriggered, giving player possibility of attempting to retry triggering this.
            yield return null;
    }

}
