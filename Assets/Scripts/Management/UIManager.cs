using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    Transform worldSpaceCanvas; //world-space UI canvas
    Transform playerMessageBox;
    Text playerMessageText;
    RawImage playerMessageBG;
    public float playerMessageHeight = 0.5f;
    public float textFadeInSpeed = 2.0f;
    public float textFadeOutSpeed = 0.5f;

    public void InitializeUIManager()
    {
        GameObject worldSpaceCanvasObj = GameObject.Find("WorldSpaceCanvas");
        
        #if UNITY_EDITOR
        if (worldSpaceCanvasObj == null)  //debugging aid in case I forgot to sort my nodes into a StageNode object
        {
            print ("ERROR! Could not find an object with name WorldSpaceCanvas in scene. Forgot to put UI canvas into one?");
            print ("Pausing play mode");
            UnityEditor.EditorApplication.isPaused = true;
        }
        #endif
        
        worldSpaceCanvas = worldSpaceCanvasObj.transform;

        playerMessageBox = worldSpaceCanvas.Find("PlayerMessage");
        playerMessageText = playerMessageBox.Find("Text").GetComponent<Text>();
        playerMessageBG = playerMessageBox.Find("BG").GetComponent<RawImage>();
        
        SetPlayerMessageVisibility(false);
    }

    void Update()
    {
        playerMessageBox.position = Player.player.transform.position + new Vector3(0.0f, playerMessageHeight, 0.0f);
    }

    public void SetPlayerMessage (string message, bool setVisible = false, bool fadeIn = false)
    {
        playerMessageText.text = message;

        if (setVisible)
        {
            if(fadeIn)
                FadeMessage(true);
            else
                SetPlayerMessageVisibility(true);
        }
    }

    public void SetPlayerMessageVisibility(bool newState)
    {
        CancelFadeCoroutine();

        Color textColour = playerMessageText.color;
        Color bgColour = playerMessageBG.color;
        if (newState)
        {
            textColour.a = 1.0f;
            bgColour.a = 1.0f;
            playerMessageText.color = textColour;
            playerMessageBG.color = bgColour;
        }
        else
        {
            textColour.a = 0.0f;
            bgColour.a = 0.0f;
            playerMessageText.color = textColour;
            playerMessageBG.color = bgColour;
        }
    }

    public void FadeMessage (bool fadeIn)
    {
        CancelFadeCoroutine(); //TODO consider remove this. Call to this method already exists in SetPlayerMessageVisibility;
        SetPlayerMessageVisibility(!fadeIn);
        fadeCoroutine = StartCoroutine(Fade(fadeIn));
    }

    void CancelFadeCoroutine()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    Coroutine fadeCoroutine = null;
    IEnumerator Fade(bool fadeIn)
    {
        Color textColour = playerMessageText.color;
        Color bgColour = playerMessageBG.color;
        bgColour.a = textColour.a = Mathf.Max(bgColour.a, textColour.a); //to make sure that both alphas are equal, to make assignments/tests bellow safe
        bool isFading = true;

            while (isFading)
            {
                playerMessageText.color = textColour;
                playerMessageBG.color = bgColour;

                if (fadeIn)
                {
                    bgColour.a = textColour.a += textFadeInSpeed * Time.deltaTime;
                
                    if (bgColour.a > 1.0f)
                        isFading = false;        
                }
                else
                {
                    bgColour.a = textColour.a -= textFadeOutSpeed * Time.deltaTime;
                    
                    if (bgColour.a < 0.0f)
                        isFading = false;        
                }

                yield return new WaitForEndOfFrame();
            }
        
        yield return null;
    }

}
