using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float heatlh;
    [SerializeField]
    private float maxHeatlh;
    
    private FloatingHealthBar healthBar;
    private Coroutine hideHealthBar;


    private void Start()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>(true);
    }

    float Health
    {
        set
        {
            heatlh = value;
            healthBar.UpdateHealth(value, maxHeatlh);

            if(heatlh <= 0)
                Death();
        }
        get => heatlh;
    }

    public void Damage(float damage)
    {
        healthBar.gameObject.SetActive(true);
        if (hideHealthBar != null)
            StopCoroutine(hideHealthBar);
        Health = math.max(Health - damage, 0);
        hideHealthBar = StartCoroutine(hideHealth(3f));
    }

    public void Heal(float heal)
    {
        Health = math.min(Health + heal, maxHeatlh);
    }

    private IEnumerator hideHealth(float time)
    {
        yield return new WaitForSeconds(time);
        if (healthBar.gameObject != null)
            healthBar.gameObject.SetActive(false);
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}
