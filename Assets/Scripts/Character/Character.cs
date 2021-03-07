using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    CharacterAnimator animator;

    public float moveSpeed;

    public bool IsMoving { get; private set; }

    public CharacterAnimator Animator {
        get => animator;
    }

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }

    //
    // MOVEMENT
    //
    public IEnumerator Move(Vector2 moveVector, Action OnMoveFinish = null)
    {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        if (!IsPathWalkable(targetPos))
            yield break;

        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        IsMoving = false;
        OnMoveFinish?.Invoke();
    }

    //
    // HELPER FUNCTIONS
    //
    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.1f, MapLayers.Instance.ObjectsLayer | MapLayers.Instance.InteractLayer) != null)
        {
            return false;
        }
        return true;
    }

    private bool IsPathWalkable(Vector3 targetPath)
    {
        var path = targetPath - transform.position;
        var direction = path.normalized;
        var origin = transform.position + direction; // start at the next tile over or we collide with the character
        var length = path.magnitude - 1; // 1 less because we start checking from the next tile over

        if (Physics2D.BoxCast(origin, new Vector2(0.2f, 0.2f), 0f, direction, length,
            MapLayers.Instance.ObjectsLayer | MapLayers.Instance.InteractLayer | MapLayers.Instance.PlayerLayer) == true)
            return false; // we found a collider on the path

        return true;
    }
}
