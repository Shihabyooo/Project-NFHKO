using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    static public Player player;
    static public TaskProgressBarController progressBar; //TODO consider moving to Character.cs
    GameObject avatarModel; //TODO consider moving to Character.cs
    public bool isHiding {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = this;
            progressBar = this.transform.Find("TaskProgressBar").gameObject.GetComponent<TaskProgressBarController>();
        }
        else
            Destroy(this.gameObject);

        avatarModel = this.transform.Find("Model").gameObject;
        //currentState = CharacterState.idle;
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
