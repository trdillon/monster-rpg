using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlerController : MonoBehaviour
{
    [SerializeField] Dialog introDialog;
    [SerializeField] Dialog outroDialog;
    [SerializeField] GameObject alert;
    [SerializeField] GameObject los;

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        RotateLoS(character.Animator.DefaultDirection);
    }

    public IEnumerator TriggerBattle(PlayerController player)
    {
        // Show alert
        alert.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        alert.gameObject.SetActive(false);

        // Move to the player
        var path = player.transform.position - transform.position;
        var tile = path - path.normalized; // Stop next to the player, not on them
        tile = new Vector2(Mathf.Round(tile.x), Mathf.Round(tile.y)); // Move value should always be int
        yield return character.Move(tile);

        // Show dialog for trash talk then start battle
        yield return DialogController.Instance.ShowDialog(introDialog, () => {
            Debug.Log("Start battle bro");
        });
    }

    public void RotateLoS(DefaultDirection direction)
    {
        float angle = 0f;
        if (direction == DefaultDirection.Down)
            angle = 0f;
        else if (direction == DefaultDirection.Up)
            angle = 180f;
        else if (direction == DefaultDirection.Left)
            angle = 270f;
        else if (direction == DefaultDirection.Right)
            angle = 90f;

        los.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }    
}
