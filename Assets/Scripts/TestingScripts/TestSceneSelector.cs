using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneSelector : MonoBehaviour
{
    public void SwitchToScene (string targetSceneName)
    {
        GameManager.sceneMan.LoadScene(targetSceneName, GameStateManager.State.gamePlay);
    }
}
