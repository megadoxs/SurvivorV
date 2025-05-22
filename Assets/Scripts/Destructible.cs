using System.Collections;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;
    
    [SerializeField] 
    private ItemStack[] drops;

    public void Destroy()
    {
        Collider2D parentCollider = GetComponent<Collider2D>();
        Vector2 spawnPosition = parentCollider.bounds.center;
        foreach (ItemStack drop in drops)
        {
            var item = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
            item.GetComponent<PickupItem>().SetItemStack(drop.Clone());
        }
        
        StartCoroutine(FadeOut(1.0f));
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(Respawn());
    }
    
    private IEnumerator FadeOut(float duration)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
    
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        sr.enabled = false;
    }
    
    private IEnumerator FadeIn(float duration)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.enabled = true;

        Color originalColor = sr.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        GetComponent<Collider2D>().enabled = true;
    }
    
    private IEnumerator Respawn() //TODO give the option for each object to have a different respawn time, could still default to something
    {
        yield return new WaitForSeconds(60);
        StartCoroutine(FadeIn(1f));
    }
}
