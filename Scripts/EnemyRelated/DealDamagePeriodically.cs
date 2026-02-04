using UnityEngine;
using System.Collections;

public class DealDamagePeriodically : MonoBehaviour
{
    public int damageAmount = 10;   // Damage per tick
    public float interval = 1f;     // Seconds between damage
    private bool playerInside = false;
    private Coroutine damageRoutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerInside)
        {
            playerInside = true;
            damageRoutine = StartCoroutine(DealDamageOverTime(other));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerInside)
        {
            playerInside = false;
            if (damageRoutine != null)
                StopCoroutine(damageRoutine);
        }
    }

    private IEnumerator DealDamageOverTime(Collider2D player)
    {
        while (playerInside)
        {
            PlayerStats.instance.TakeDamage(damageAmount); // or player.GetComponent<PlayerHealth>().TakeDamage()
            yield return new WaitForSeconds(interval); // exact interval
        }
    }
}
