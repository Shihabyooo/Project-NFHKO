using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum State {mainMenu, pauseMenu, gamePlay, loadingScreen};
    public State gameState {get; private set;}
    //public State gameState;

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

    void OnGUI()
    {
        string message = "Current GameState: ";
        message += gameState.ToString();

        GUI.Label(new Rect (30.0f, Screen.height - 70.0f, 200.0f, 50.0f), message);
    }
}
