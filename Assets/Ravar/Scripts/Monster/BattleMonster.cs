using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Itsdits.Ravar.UI;

namespace Itsdits.Ravar.Monster 
{ 
    /// <summary>
    /// Handles the UI and animation functions of a <see cref="MonsterObj"/> in a battle.
    /// </summary>
    public class BattleMonster : MonoBehaviour
    {
        [Tooltip("The BattleHUD GameObject for this monster.")]
        [SerializeField] BattleHUD hud;
        [Tooltip("Is this the player's monster or not.")]
        [SerializeField] bool isPlayerMonster;

        private Image image;
        private Vector3 originalPos;
        private Color originalColor;

        /// <summary>
        /// The monster object that this class is initialized from.
        /// </summary>
        public MonsterObj Monster { get; set; }
        /// <summary>
        /// The BattleHUD for this monster.
        /// </summary>
        public BattleHUD Hud => hud;
        /// <summary>
        /// Is this the player's monster or not.
        /// </summary>
        public bool IsPlayerMonster => isPlayerMonster;
        /// <summary>
        /// The Image component of this monster.
        /// </summary>
        public Image Image => image;

        private void Awake()
        {
            image = GetComponent<Image>();
            originalPos = image.transform.localPosition;
            originalColor = image.color;
        }

        /// <summary>
        /// Setup function to prepare the monster in the battle screen.
        /// </summary>
        /// <param name="monster">Monster to setup.</param>
        public void Setup(MonsterObj monster)
        {
            Monster = monster;

            if (isPlayerMonster)
            {
                image.sprite = Monster.Base.LeftSprite;
            }
            else
            {
                image.sprite = Monster.Base.RightSprite;
            }

            hud.gameObject.SetActive(true);
            hud.SetData(monster);
            image.color = originalColor;
            transform.localScale = new Vector3(1, 1, 1);
            PlayBattleStartAnimation();
        }

        /// <summary>
        /// Deactive the BattleHUD GameObject to hide it.
        /// </summary>
        /// <remarks>Used in the beginning of a battle to clear the screen for battle setup.</remarks>
        public void HideHud()
        {
            Hud.gameObject.SetActive(false);
        }

        /// <summary>
        /// Play monster entering battle animation.
        /// </summary>
        public void PlayBattleStartAnimation()
        {
            if (isPlayerMonster)
            {
                image.transform.localPosition = new Vector3(-500f, originalPos.y);
            }
            else
            {
                image.transform.localPosition = new Vector3(500f, originalPos.y);
            }

            image.transform.DOLocalMoveX(originalPos.x, 1f);
        }

        /// <summary>
        /// Play monster attacking animation.
        /// </summary>
        public void PlayAttackAnimation()
        {
            var sequence = DOTween.Sequence();
            if (isPlayerMonster)
            {
                sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50, 0.25f));
            }
            else
            {
                sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50, 0.25f));
            }

            sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
        }

        /// <summary>
        /// Play animation of monster being hit.
        /// </summary>
        public void PlayHitAnimation()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(image.DOColor(Color.red, 0.1f));
            sequence.Append(image.DOColor(originalColor, 0.1f));
        }

        /// <summary>
        /// Play animation of monster being downed.
        /// </summary>
        public void PlayDownedAnimation()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
            sequence.Join(image.DOFade(0f, 0.5f));
        }

        /// <summary>
        /// Play animation of monster being captured.
        /// </summary>
        public void PlayCaptureAnimation()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(image.transform.DOShakePosition(5f, 3));
        }
    }
}