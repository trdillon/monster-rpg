using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Vector2 input;
    private bool isMoving;

    public LayerMask foregroundLayer;
    public LayerMask fightgroundLayer;
    public float moveSpeed;

    public event Action OnEncounter;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if(!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // prevent diagonal movement
            if (input.x != 0) input.y = 0;

            if(input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }
        }

        animator.SetBool("isMoving", isMoving);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.1f, foregroundLayer) != null)
        {
            return false;
        }
        return true;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, fightgroundLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 7) //TODO - decide the percentage of event triggers
            {
                animator.SetBool("isMoving", false);
                OnEncounter();
            }
        }
    }
}
