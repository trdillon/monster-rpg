using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Itsdits.Ravar.UI.Battle;

namespace Itsdits.Ravar.Monster 
{ 
    /// <summary>
    /// Handles the UI and animation functions of a <see cref="MonsterObj"/> in a battle.
    /// </summary>
    public class BattleMonster : MonoBehaviour
    {
        [Tooltip("The BattleHUD GameObject for this monster.")]
        [SerializeField] private BattleHud _hud;
        [Tooltip("Is this the player's monster or not.")]
        [SerializeField] private bool _isPlayerMonster;

        private Image _image;
        private Vector3 _originalPos;
        private Color _originalColor;

        /// <summary>
        /// The monster object that this class is initialized from.
        /// </summary>
        public MonsterObj Monster { get; private set; }
        /// <summary>
        /// The BattleHUD for this monster.
        /// </summary>
        public BattleHud Hud => _hud;
        /// <summary>
        /// Is this the player's monster or not.
        /// </summary>
        public bool IsPlayerMonster => _isPlayerMonster;
        /// <summary>
        /// The Image component of this monster.
        /// </summary>
        public Image Image => _image;
        /// <summary>
        /// The original position of the BattleMonster on the scene.
        /// </summary>
        public Vector3 OriginalPos => _originalPos;
        /// <summary>
        /// The original color of the BattleMonster.
        /// </summary>
        public Color OriginalColor => _originalColor;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _originalPos = _image.transform.localPosition;
            _originalColor = _image.color;
        }

        /// <summary>
        /// Setup function to prepare the monster in the battle screen.
        /// </summary>
        /// <param name="monster">Monster to setup.</param>
        public void Setup(MonsterObj monster)
        {
            Monster = monster;
            _image.sprite = _isPlayerMonster ? Monster.Base.LeftSprite : Monster.Base.RightSprite;

            _hud.gameObject.SetActive(true);
            _hud.SetData(monster);
            
            _image.color = _originalColor;
            transform.localScale = new Vector3(1, 1, 1);
            PlayBattleStartAnimation();
        }

        /// <summary>
        /// Deactivate the BattleHUD GameObject to hide it.
        /// </summary>
        /// <remarks>Used in the beginning of a battle to clear the screen for battle setup.</remarks>
        public void HideHud()
        {
            Hud.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Play monster attacking animation.
        /// </summary>
        public void PlayAttackAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_isPlayerMonster
                                ? _image.transform.DOLocalMoveX(_originalPos.x + 50, 0.25f)
                                : _image.transform.DOLocalMoveX(_originalPos.x - 50, 0.25f));

            sequence.Append(_image.transform.DOLocalMoveX(_originalPos.x, 0.25f));
        }

        /// <summary>
        /// Play animation of monster being hit.
        /// </summary>
        public void PlayHitAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_image.DOColor(Color.red, 0.1f));
            sequence.Append(_image.DOColor(_originalColor, 0.1f));
        }

        /// <summary>
        /// Play animation of monster being downed.
        /// </summary>
        public void PlayDownedAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_image.transform.DOLocalMoveY(_originalPos.y - 150f, 0.5f));
            sequence.Join(_image.DOFade(0f, 0.5f));
        }

        /// <summary>
        /// Play animation of monster being captured.
        /// </summary>
        public void PlayCaptureAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_image.transform.DOShakePosition(5f, 3));
        }
        
        private void PlayBattleStartAnimation()
        {
            if (_isPlayerMonster)
            {
                _image.transform.localPosition = new Vector3(-500f, _originalPos.y);
            }
            else
            {
                _image.transform.localPosition = new Vector3(500f, _originalPos.y);
            }

            _image.transform.DOLocalMoveX(_originalPos.x, 1f);
        }
    }
}