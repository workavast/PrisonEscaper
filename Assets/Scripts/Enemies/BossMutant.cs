using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMutant : Enemy
{
    public GameObject roarAnim;

    private IEnumerator RoarAnimation()
    {
        roarAnim.SetActive(true);
        yield return new WaitForSeconds(1f);
        roarAnim.SetActive(false);
    }

    IEnumerator JumpToTarget()
    {
        float timeToJump = 0.6f, jumpHeight = 5f;
        Vector2 startPos = transform.position; // Начальная позиция объекта
        Vector2 targetPos = target.position; // Позиция целевого объекта

        float elapsedTime = 0f;

        while (elapsedTime < timeToJump)
        {
            float t = elapsedTime / timeToJump;

            // Формула для движения по дугообразной траектории
            float height = Mathf.Sin(Mathf.PI * t) * jumpHeight;

            // Обновление позиции объекта
            transform.position = Vector2.Lerp(startPos, targetPos, t) + Vector2.up * height;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //transform.position = targetPos;
        yield return null;
    }

    private IEnumerator JumpAttack()
    {
        animator.Play("Jump");
        StartCoroutine(JumpToTarget());
        yield return new WaitForSeconds(1f);
        base.Attack();
        animator.Play("Jump Attack");

    }

    protected override void Move()
    {
        base.Move();
        float distanceToTargetX = Mathf.Abs(target.position.x - transform.position.x);
        if (distanceToTargetX < 2.5f)
        {
            attackPoint.localPosition = new Vector3(1.25f,attackPoint.localPosition.y,attackPoint.localPosition.z);
        }
        else
        {
            attackPoint.localPosition = new Vector3(3.88f, attackPoint.localPosition.y, attackPoint.localPosition.z);
        }
    }
    protected override void Attack()
    {
        float distanceToTargetX = Mathf.Abs(target.position.x - transform.position.x);

        if (distanceToTargetX<2.5f)
        {
            base.Attack();
            animator.Play("Punch");
            return;
        }

        int attackType = Random.Range(1, 5);
        _attacking = true;
        switch(attackType)
            {
            case 1:
                StartCoroutine(JumpAttack()); // 20%
                break;
            default:
                base.Attack();
                StartCoroutine(RoarAnimation()); // 80%
                break;
        }
    }
}
