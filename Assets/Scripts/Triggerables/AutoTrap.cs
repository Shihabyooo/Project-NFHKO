//AutoTraps are pranks that are set by the player but don't require the AI to "trigger" them as part of the routine, rather they only need AI to "walk" overthem.
//Examples: Banana peels or oil on floor, electrecuted water puddles, etc.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoTrap : Triggerable
{
    public override void Cancel()
    {
        //print ("Called Cancel()");
        StopAllCoroutines();
        pTriggerProcessCoroutine = null;
        Player.player.progressBar.SetBarVisibility(false);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "AI_Main" && isTriggered)
        {
            AI.mainAI.ActivateNonRoutinePrankResponse();
            SwitchState(TriggerableState.aiTriggered);
        }
    }

    //TriggerAI() and StartAITrigger() are useless for this type of objects.
    public override void TriggerAI()
    {
    }

    protected override void StartAITrigger()
    {
    }

}
