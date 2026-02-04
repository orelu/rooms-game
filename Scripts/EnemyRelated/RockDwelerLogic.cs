using UnityEngine;
using System.Collections;

public class RockDwelerLogic : MonoBehaviour
{
    //public Transform player; // Assign the target object in the inspector
    public float triggerDistance = 5f;
    public float interval = 1f;
    private bool isRunning = false;
    public GameObject blade;

    public Animator animator;
    public Animator bladeAnimator;

    public EnemyBaseBehavior enemyStats;
    public GameObject particleSystem;

    void Update()
    {
        //if (player != null)
        //{
            //float distance = Vector3.Distance(transform.position, player.position);
            //distance <= triggerDistance && !isRunning
            if (!isRunning)
            {
                StartCoroutine(RunFunctionRepeatedly());
            }
            //else if (distance > triggerDistance && isRunning)
            //{
                //StopCoroutine(RunFunctionRepeatedly());
                //isRunning = false;
            //}
        //}
    }

    IEnumerator RunFunctionRepeatedly()
    {
        isRunning = true;
        while (enemyStats.health > 0)
        {
            // Start the coroutine properly
            yield return StartCoroutine(RunFunction());
            yield return new WaitForSeconds(interval);
        }
        isRunning = false;
    }

    IEnumerator RunFunction()
    {
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(1f);
        blade.SetActive(true);
        yield return new WaitForSeconds(1f);
        bladeAnimator.SetTrigger("swing");
        particleSystem.SetActive(true);
        int i = 1;
        while (i<6) {
            yield return new WaitForSeconds(1f);
            enemyStats.AttackPlayer(25, 3);
            i+=1;
        }
        bladeAnimator.SetTrigger("stopSwing");
        particleSystem.SetActive(false);
        yield return new WaitForSeconds(2f);
        blade.SetActive(false);
        animator.SetBool("isAttacking", false);
    }
}
