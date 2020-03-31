using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagementHandler : MonoBehaviour
{
    
    string targetSceneName;
    public bool isLoading {get; private set;} = false;
    bool isInLoadingScreen = false;
    AsyncOperation bgLevelLoader;
    GameStateManager.State stateOnLoad = GameStateManager.State.mainMenu;

    void Awake()
    {
        
    }

    public void InitializeSceneManagementHandler()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(string sceneName, GameStateManager.State newStateOnLoad)
    {
        //TODO update current stage value here (once it's implemented)
        //print ("in LoadScene(), recieved newStateOnLoad to: " + newStateOnLoad); //test
        targetSceneName = sceneName;
        stateOnLoad = newStateOnLoad;
        //print ("in LoadScene(), set stateOnLoad to: " + stateOnLoad); //test
        SwitchToLoadingScreen();
    }

    void SwitchToLoadingScreen()
    {
        isInLoadingScreen = true;
        GameManager.stateMan.SwitchGameState(GameStateManager.State.loadingScreen);
        SceneManager.LoadScene("LoadingScreen");
    }

    void BeginLoadingTargetScene(float beginDelay = 0.5f, float endDelay = 0.5f)
    {
        StartCoroutine(SceneLoading(beginDelay, endDelay));
    }

    IEnumerator SceneLoading(float beginDelay, float endDelay)
    {
        yield return new WaitForSeconds(beginDelay); //mostly for development phases, to avoid having the loading screen flash instantaniously.
        
        bgLevelLoader = SceneManager.LoadSceneAsync(targetSceneName);
        bgLevelLoader.allowSceneActivation = false; //this prevents Unity from switching to the new scene it finishes loading it in the background.

        while (!bgLevelLoader.isDone)
        {
            //Remember that AsyncOperation.progress goes from 0.0f to 0.9f, not a perfect 1.0f.

            TestLoadingScreenHandler.loadingScreen.SetProgress(bgLevelLoader.progress / 0.9f); //TODO remove this and switch to final loading screen implementation

            if (bgLevelLoader.progress >= 0.891f) //0.891 is 99% if 0.9.
            {
                yield return new WaitForSeconds(endDelay); //mostly for development phases, to avoid having the loading screen flash instantaniously.
                isInLoadingScreen= false;
                bgLevelLoader.allowSceneActivation = true; //we now allow unity to switch to our loaded scene.
            }
        }

        yield return null;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isInLoadingScreen) //This only happens when SwitchToLoadingScreen() is called, and that only happens when we want to switch to a stage we've set the name of in targetSceneName
        {
            //Note to self: In my AbbasTheGame implementation, at this point, I had a GameObject.find to get references to loading screen management script. In retrospect, this wasn't
            //the best approach. Could've easily set the handler instance as a singleton (according to internet: Awake() is called before sceneLoaded, so no race condition here)
            BeginLoadingTargetScene(0.5f, 0.5f);
            return; 
        }
        //else this means we've finished loading our targetScene (since isInLoadScreen is set to false in the coroutine and before allowing Unity to switch to target scene)
        
        //print ("Scene '" + scene.name + "' was loaded successfully.");

        //TODO have GameManager do its stage start initializations here

        isLoading = false;
        GameManager.stateMan.SwitchGameState(stateOnLoad);
    }
}
