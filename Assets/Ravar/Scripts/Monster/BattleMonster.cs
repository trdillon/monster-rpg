using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Itsdits.Ravar.UI.Battle;
using Itsdits.Ravar.Animation;

namespace Itsdits.Ravar.Monster { 
    public class BattleMonster : MonoBehaviour
    {
        [SerializeField] BattleHUD hud;
        [SerializeField] bool isPlayerMonster;

        private Image image;
        private Vector3 originalPos;
        private Color originalColor;

        public MonsterObj Monster { get; set; }
        public BattleHUD Hud => hud;
        public bool IsPlayerMonster => isPlayerMonster;
        public Image Image => image;

        private void Awake()
        {
            image = GetComponent<Image>();
            if (image == null)
            {
                Debug.LogError($"BM001: {Monster.Base.Name} missing Image.");
            }
            originalPos = image.transform.localPosition;
            originalColor = image.color;
        }

        /// <summary>
        /// Setup the monster for battle.
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
            
            if (image.sprite == null)
            {
                Debug.LogError($"BM002: {Monster.Base.Name} missing sprite.");
            }

            hud.gameObject.SetActive(true);
            hud.SetData(monster);
            image.color = originalColor;
            transform.localScale = new Vector3(1, 1, 1);
            PlayBattleStartAnimation();
        }

        /// <summary>
        /// Hide the BattleHud.
        /// </summary>
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