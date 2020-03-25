using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    static public GameManager gameMan;
    static public PathFinder pathFinder;
    void Start()
    {
        if (gameMan ==  null)
        {
            gameMan = this;
            pathFinder = this.gameObject.GetComponent<PathFinder>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

}
