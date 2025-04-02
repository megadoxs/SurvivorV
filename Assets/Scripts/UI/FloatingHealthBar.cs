using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    private Slider _slider;
    private Coroutine healthLerpCoroutine;
    private float animationDuration = 0.5f;

    void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void UpdateHealth(float health, float maxHealth)
    {
        float targetValue = health / maxHealth;
        
        if (healthLerpCoroutine != null)
        {
            StopCoroutine(healthLerpCoroutine);
        }
        
        healthLerpCoroutine = StartCoroutine(SmoothHealthChange(targetValue));
    }

    private IEnumerator SmoothHealthChange(float targetValue)
    {
        if (ReferenceEquals(_slider, null))
            yield break;
        
        float elapsedTime = 0f;
        float startValue = _slider.value;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            _slider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / animationDuration);
            yield return null;
        }

        _slider.value = targetValue;
    }
}
