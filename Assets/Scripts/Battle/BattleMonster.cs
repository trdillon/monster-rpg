using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleMonster : MonoBehaviour
{
    // For testing these are set in the inspector
    //TODO - set these dynamically
    [SerializeField] MonsterBase monsterBase;
    [SerializeField] int level;
    [SerializeField] bool isPlayerMonster;
    Image image;
    Vector3 originalPos;
    Color originalColor;

    public Monster Monster { get; set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup()
    {
        Monster = new Monster(monsterBase, level);

        if (isPlayerMonster)
            image.sprite = Monster.Base.BackSprite;
        else
            image.sprite = Monster.Base.FrontSprite;

        PlayBattleStartAnimation();
    }

    public void PlayBattleStartAnimation()
    {
        if (isPlayerMonster)
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, originalPos.y);

        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerMonster)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.red, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayDownedAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
}
