using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO consider having a TriggerableContainer class (inheriting from Triggerable) that you use as base for objects such as this.

public class DiscRack : Triggerable
{
    public uint containedItemID;

    public override bool Trigger()
    {
        if (isTriggered)
            return false;
        else if (pTriggerProcessCoroutine != null)
            Cancel();

        StartPlayerTrigger();
        return true;
    }
    // protected override IEnumerator PlayerTriggerProcess()
    // {
    //     base.PlayerTriggerProcess();

    //     //TODO after implementing inventory system, add the item contained within this script to it
    //     yield return null;
    // }


}
