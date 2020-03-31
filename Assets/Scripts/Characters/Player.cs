using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    static public Player player;

    public bool isHiding;   //because the declaration above can't be exposed in editor, and I use this to be able to move player around even in rooms with enemy without
                            //triggering player catching and stage end. 

    public override void Awake()
    {
        if (player == null)
            player = this;
        else
            Destroy(this.gameObject);

        base.Awake();
    }
    
    public void SetHidingStatus(bool newState)
    {
        isHiding = newState;
        
        //test to visualize hiding
        Material material = avatarModel.GetComponent<MeshRenderer>().material;
        Color tempColor = material.color;
        tempColor = newState? new Color(0.2f, 0.2f, 0.2f, 1.0f) : new Color(1.0f, 1.0f, 1.0f, 1.0f);
        avatarModel.GetComponent<MeshRenderer>().material.color = tempColor;
        //end test
    }

}
