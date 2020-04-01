using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum State {mainMenu, pauseMenu, gamePlay, loadingScreen};
    public State gameState {get; private set;}
    public float timeElapsedSinceStageStart {get; private set;} = 0.0f;


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

    public void RestartStageTimer()
    {
        StopStageTimer();
        stageTimeKeeper = StartCoroutine(TimeKeeper());
    }
    
    public void StopStageTimer()
    {
        if (stageTimeKeeper != null)
            StopCoroutine(stageTimeKeeper);
    }

    Coroutine stageTimeKeeper = null;

    IEnumerator TimeKeeper()
    {
        timeElapsedSinceStageStart = 0.0f;
        while(true)
        {
            yield return new WaitForFixedUpdate();
            timeElapsedSinceStageStart += Time.fixedDeltaTime;
        }
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
