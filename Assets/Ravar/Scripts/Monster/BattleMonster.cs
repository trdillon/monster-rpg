using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleMonster : MonoBehaviour
{
    [SerializeField] bool isPlayerMonster;
    [SerializeField] BattleHUD hud;

    Image image;
    Vector3 originalPos;
    Color originalColor;

    public Monster Monster { get; set; }
    public bool IsPlayerMonster => isPlayerMonster;
    public BattleHUD Hud => hud;
    public Image Image => image;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Monster monster)
    {
        Monster = monster;

        if (isPlayerMonster)
            image.sprite = Monster.Base.LeftSprite;
        else
            image.sprite = Monster.Base.RightSprite;

        hud.gameObject.SetActive(true);
        hud.SetData(monster);
        image.color = originalColor;
        transform.localScale = new Vector3(1, 1, 1);
        PlayBattleStartAnimation();
    }

    public void HideHud()
    {
        Hud.gameObject.SetActive(false);
    }

    //
    // ANIMATIONS
    //TODO - move these to BattleAnimator?
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

    public void PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOShakePosition(5f, 3));
    }
}
