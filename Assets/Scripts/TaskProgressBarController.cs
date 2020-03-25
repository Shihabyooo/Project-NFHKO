using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskProgressBarController : MonoBehaviour
{
    
    SpriteRenderer sprite;
    [SerializeField] float maxValue = 1.0f;
    [SerializeField] float minValue = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        sprite = this.gameObject.GetComponent<SpriteRenderer>();
    }

    public void SetProgress(float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0.0f, 1.0f);
        
        float barSize =  percentage * (maxValue - minValue);
        sprite.size = new Vector2(barSize, sprite.size.y);

        Color newBarColor = new Color((1.0f - percentage), 1.0f, 0.0f, 1.0f);
        sprite.color = newBarColor;
    }

    public void SetBarVisibility(bool isVisible)
    {
        Color barColor = sprite.color;
        if (isVisible)
            barColor.a = 1.0f;
        else
            barColor.a = 0.0f;

        sprite.color = barColor;
    }
    

}
