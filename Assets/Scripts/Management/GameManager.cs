using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    static public GameManager gameMan;
    static public PathFinder pathFinder;
    static public InventorySystem inventory;
    static public NarrativeManager narrativeMan;
    static public UIManager uiMan;
    static public SceneManagementHandler sceneMan;
    static public GameStateManager stateMan;
    public bool isGameplayActive {get; private set;}

    void Awake()
    {
        if (gameMan ==  null)
        {
            gameMan = this;
            
            pathFinder = this.gameObject.GetComponent<PathFinder>();
            inventory = this.gameObject.GetComponent<InventorySystem>();
            narrativeMan = this.gameObject.GetComponent<NarrativeManager>();            
            uiMan = this.gameObject.GetComponent<UIManager>();
            sceneMan = this.gameObject.GetComponent<SceneManagementHandler>();
            stateMan = this.gameObject.GetComponent<GameStateManager>();

            pathFinder.enabled = false;
            inventory.enabled = false;
            uiMan.enabled = false;
            
            sceneMan.InitializeSceneManagementHandler(); //because sceneMan's roles are required early at the begining of the game.

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }        
    }

    void Start()
    {
        //StartCoroutine(StageBeginInit());
        //The coroutine above will now only be started upon switching to a new stage (and currenGameState is set to GameState.gamePlayer)
    }

    public void StartNewStage()
    {
        StartCoroutine(StageBeginInit());
    }

    IEnumerator StageBeginInit()
    {
        inventory.enabled = true;
        inventory.InitializeInventory();
        uiMan.enabled = true;
        uiMan.InitializeUIManager();
        pathFinder.enabled = true;
        pathFinder.InitializePathFinder();

        while (!pathFinder.isInitialized) //Why? Shouldn't a simple call to PathFinder.InitializePathFinder stall the rest of the calling method untill init finishes?
            yield return new WaitForSeconds(0.5f);

        isGameplayActive = true; //TODO remember to remove this when implementing a proper game start
        StartPlayerAndAI();
        yield return null;
    }

    void FixedUpdate()
    {
        CheckPlayerVisibilityToAI();
    }

    void StartPlayerAndAI()
    {
        Player.player.CustomStart();
        AI.mainAI.CustomStart();
    }

    void CheckPlayerVisibilityToAI()
    {
        if (isGameplayActive &&
            AI.mainAI.currentNode == Player.player.currentNode &&               //check if within same room
            AI.mainAI.currentNode.nodeType == NavigationNode.NodeType.room &&   //check that the node represents a room (no need to check both, one is enough thanks the equality test above)
            !Player.player.isHiding)                                            //check that player isn't hiding
        {
            ProcessPlayerCatching();
        }
    }

    void ProcessPlayerCatching()
    {
        print ("Player is caught at room " + AI.mainAI.currentNode.gameObject.name);//test
        Player.player.isActive = false;
        AI.mainAI.isActive = false;
        isGameplayActive = false;
    }

    public void EndStage()
    {
        pathFinder.ResetPathFinder();
    }

}