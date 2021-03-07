using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float movementPatternTime;

    Character character;
    NPCState state;

    float idleTimer = 0f;
    int currentMovement = 0;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Update()
    {
        // Stop movement pattern to interact
        if (DialogController.Instance.IsShowing) return;

        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > movementPatternTime)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }

        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;

        yield return character.Move(movementPattern[currentMovement]);
        currentMovement = (currentMovement + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }

    public void Interact()
    {
        if (state == NPCState.Idle)
            StartCoroutine(DialogController.Instance.ShowDialog(dialog));
    }
}
