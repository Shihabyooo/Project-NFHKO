using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    static public GameManager gameMan;
    static public PathFinder pathFinder;
    static public InventorySystem inventory;

    public bool isGameplayActive {get; private set;}
    void Start()
    {
        if (gameMan ==  null)
        {
            gameMan = this;
            pathFinder = this.gameObject.GetComponent<PathFinder>();
            inventory = this.gameObject.GetComponent<InventorySystem>();
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        StartCoroutine(FirstStart());
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


    IEnumerator FirstStart()
    {
        while (!pathFinder.isInitialized)
            yield return new WaitForSeconds(0.1f);

        isGameplayActive = true; //TODO remember to remove this when implementing a proper game start
        StartPlayerAndAI();
        yield return null;
    }
}