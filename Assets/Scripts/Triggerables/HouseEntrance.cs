//Common sense tells me that managing the house entrance (which controls the "good"/non-failure ending of the stage), should have its own script. But for now, let's
//avoid messing up the code elsewhere and hack a Triggerable class for this. Saves us some clicking checks and if-statements.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HouseEntrance : Triggerable
{
    public override bool Trigger()
    {
        if (!CheckPreRequisites())
        {
            //print ("Hasn't reached minimum required pranks yet");
            GameManager.narrativeMan.ShowMessage("I can't go home just yet.");
            return false;
        }

        StartPlayerTrigger();
        return true;
    }

    public override void TriggerAI() //AI shouldn't ever trigger this.
    {
        //StartAITrigger();
    }

    protected override bool CheckPreRequisites() 
    {
        //TODO After implementing a stage requirement handler (one that contains, for eg. the minimum tricks need to be triggered before stage can be ended), do the checking here.
        //Consider having this check be done in GameStateManager.
        print ("Checking prerequisites for level end");
        return false;
        
    }

    protected override void StartPlayerTrigger() //Here we call a pop up menu (and, in case the player hasn't set all traps, a reminder of such) asking whether the player wants to
    {                                            //end the stage and progress to the next one. If not, simply close the popup. If yes, end stage.
        //print ("Inside StartPlayerTrigger()");
        //pTriggerProcessCoroutine = StartCoroutine(PlayerTriggerProcess());
    } 

    public override void SwitchState(TriggerableState newstate) //state of this project should never change.
    {
        //state = newstate;
    }

}
