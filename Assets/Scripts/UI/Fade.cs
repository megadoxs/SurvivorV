using System;
using UnityEngine;

public class Fade : MonoBehaviour
{
    [SerializeField]
    private float timeToFade = 1f;

    
    public static Fade instance;
    public CanvasGroup canvasGroup;
    public static bool state;

    private void Start()
    {
        instance = this;
        state = false;
    }

    void Update()
    {
        if(state && canvasGroup.alpha < 1)
            canvasGroup.alpha += Time.deltaTime / timeToFade;
        else if (!state && canvasGroup.alpha > 0)
            canvasGroup.alpha -= Time.deltaTime / timeToFade;
    }

    public static bool Done()
    {
        return state && instance.canvasGroup.alpha >= 1 || !state && instance.canvasGroup.alpha <= 0;
    }
}
