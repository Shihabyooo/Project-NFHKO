using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeManager : MonoBehaviour
{
    public float playerMessageDuration = 5.0f;

    public void ProcessTriggerView(Triggerable triggerable)
    {

        string message = "";

        switch(triggerable.state)
        {
            case Triggerable.TriggerableState.untriggered:
                message = triggerable.script.inspectionMessageUntriggered;
                break;
            case Triggerable.TriggerableState.playerTriggered:
                message = triggerable.script.inspectionMessageTriggeredPlayer;
                break;
            case Triggerable.TriggerableState.aiTriggered:
                message = triggerable.script.inspectionMessageTriggeredAI;
                break;
        }

        if (message == "" )
            message = "[Empty Message]"; //test

        ShowMessage(message);
    }

    public void ShowMessage(string message)
    {
        StopAllCoroutines();

        GameManager.uiMan.SetPlayerMessage(message, true);
        StartCoroutine(MessageWait(playerMessageDuration, true));
    }

    IEnumerator MessageWait(float timeOut, bool fadeOut = false)
    {
        yield return new WaitForSeconds (timeOut);
        
        if (fadeOut)
            GameManager.uiMan.FadeMessage(false);
        else
            GameManager.uiMan.SetPlayerMessageVisibility(false);

        yield return null;
    }

}
