﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum State {mainMenu, pauseMenu, gamePlay, loadingScreen};
    public State gameState {get; private set;}
    public float timeElapsedSinceStageStart {get; private set;} = 0.0f;
    public int currentStageID = -1; //TODO make this private when testing is done.
    public Stage currentStageParameters = null; //TODO make this private when testing is done.
    public int successfulPranks = 0; //TODO make this private when testing is done.

    public void SwitchGameState(State newState)
    {
        switch (newState)
        {
            case State.mainMenu:
                print ("Switching gameState to mainMenu"); //test
                break;

            case State.gamePlay:
                print ("Switching gameState to gamePlay"); //test
                if (gameState == State.loadingScreen) //this only happens when we load a new gameplay scene
                    GameManager.gameMan.StartNewStage();
                break;

            case State.pauseMenu:
                print ("Switching gameState to pauseMenu"); //test
                break;

            case State.loadingScreen:
                print ("Switching gameState to loadingScreen"); //test
                break;

            default:
                print ("Default case triggered in SiwthGameState()");
                break;
        }
        
        gameState = newState;
        //print ("New gameState: " + gameState);
    }

    public void InitializeNewStageState()
    {
        successfulPranks = 0;
        RestartStageTimer();
    }

    void RestartStageTimer()
    {
        StopStageTimer();
        timeElapsedSinceStageStart = 0.0f;
        stageTimeKeeper = StartCoroutine(TimeKeeper());
    }
    
    public void StopStageTimer() //also functions as Pause, since the time at calling is stored, can be resumed by ResumeStageTimer() and won't be reset unless RestartStageTimer()
    {
        if (stageTimeKeeper != null)
            StopCoroutine(stageTimeKeeper);
    }

    public void ResumeStageTimer()
    {
        StopStageTimer(); //in case I was stupid enough to call this method while a TimeKeeper is already running.
        stageTimeKeeper = StartCoroutine(TimeKeeper());
    }

    Coroutine stageTimeKeeper = null;

    IEnumerator TimeKeeper()
    {
        while(true)
        {
            yield return new WaitForFixedUpdate();
            timeElapsedSinceStageStart += Time.fixedDeltaTime;
        }
    }

    public void UpdateStageParameters(int stageID)
    {
        currentStageID = stageID;
        
        if (stageID >= 0) //I'm having an assumption here that, sometimes in the future, I'm going to send negative numbers as stageIDs for non playable levels (e.g. menus. loading, etc)
            currentStageParameters = StagesData.GetStageParameters((uint)currentStageID);
    }

    public void BumpSuccessfulPranksCount(uint increment = 1)
    {
        successfulPranks += (int)increment;
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        string message = "Current GameState: " + gameState.ToString();
        if (gameState == State.gamePlay)
            message += ". Gameplay state: " + (GameManager.gameMan.isGameplayActive? "Active." : "Inactive");

        GUI.Label(new Rect (30.0f, Screen.height - 40.0f, Screen.width - 60.0f , 30.0f), message);

        GUI.Label(new Rect(30.0f, Screen.height - 70.0f, 200.0f, 30.0f), "Time elapsed: " + timeElapsedSinceStageStart);

    }
#endif

}
