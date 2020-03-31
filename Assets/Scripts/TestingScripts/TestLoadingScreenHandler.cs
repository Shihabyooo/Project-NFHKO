using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLoadingScreenHandler : MonoBehaviour
{
    public static TestLoadingScreenHandler loadingScreen;
    Slider progressBar;

    void Awake()
    {
        if (loadingScreen == null)
        {
            loadingScreen = this;
            progressBar = GameObject.Find("ProgressBar").GetComponent<Slider>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetProgress(float progress) //progress recieved as percentage 0.0f ~ 1.0f
    {
        progressBar.value = progress;
    }


}
