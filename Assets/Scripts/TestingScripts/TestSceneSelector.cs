using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneSelector : MonoBehaviour
{
    // public void SwitchToScene (string targetSceneName)
    // {
    //     GameManager.sceneMan.LoadScene(targetSceneName, GameStateManager.State.gamePlay);
    // }

    public void SwitchToScene (int stageID) //Unity won't allow this method to be used for buttons if it took uint. Dunno why...
    {
        GameManager.sceneMan.LoadScene((uint)stageID, GameStateManager.State.gamePlay);
    }
}
