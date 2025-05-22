using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{

    [SerializeField] 
    private TMP_Text announcement;
    
    [SerializeField]
    private TMP_Text body;
    
    public static TextManager instance;
    
    private Dictionary<TMP_Text, float> textTimers = new();
    private Dictionary<TMP_Text, float> updatingTextTimers = new();

    private void Awake()
    {
        instance = this;   
    }

    public void ShowAnnouncement(string text, float delay)
    {
        announcement.text = text;
        announcement.color = new Color(body.color.r, body.color.g, body.color.b);
        announcement.gameObject.SetActive(true);
        textTimers.Add(announcement, delay);
    }

    public void ShowBody(string text, float delay)
    {
        body.text = text;
        body.color = new Color(body.color.r, body.color.g, body.color.b);
        body.gameObject.SetActive(true);
        updatingTextTimers.Add(body, delay);
    }

    private void Update()
    {
        var keys = textTimers.Keys.ToList();

        foreach (var key in keys)
        {
            if (textTimers[key] > 0f) {
                textTimers[key] -= Time.deltaTime;
                continue;
            }
            
            var color = key.color;
            key.color = new Color(color.r, color.g, color.b, Mathf.Max(color.a - Time.deltaTime, 0));

            if (color.a == 0)
            {
                key.gameObject.SetActive(false);
                textTimers.Remove(key);
            }
        }
        
        var keys2 = updatingTextTimers.Keys.ToList();

        foreach (var key in keys2)
        {
            if (updatingTextTimers[key] > 0f)
            {
                updatingTextTimers[key] -= Time.deltaTime;
                key.text = System.Text.RegularExpressions.Regex.Replace(key.text, @"\s*\d+$", "") +
                           Mathf.CeilToInt(updatingTextTimers[key]);
                continue;
            }

            var color = key.color;
            key.color = new Color(color.r, color.g, color.b, Mathf.Max(color.a - Time.deltaTime, 0));

            if (color.a == 0)
            {
                key.gameObject.SetActive(false);
                updatingTextTimers.Remove(key);
            }
        }
    }
}
