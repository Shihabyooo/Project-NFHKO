using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    static public GameManager gameMan;
    static public PathFinder pathFinder;
    static public InventorySystem inventory;
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
    }

}
