using System.Collections;
using UnityEngine;
using DG.Tweening;
using Itsdits.Ravar.Character;
using Itsdits.Ravar.Monster;
using Itsdits.Ravar.Util;
using UnityEngine.UI;

namespace Itsdits.Ravar.Animation
{
    /// <summary>
    /// Creates battle animations during the battle scene.
    /// </summary>
    public class BattleAnimator : MonoBehaviour
    {
        [Header("Characters")]
        [Tooltip("Sprite of the player.")]
        [SerializeField] private Image _playerImage;
        [Tooltip("Sprite of the Battler.")]
        [SerializeField] private Image _battlerImage;
        
        [Header("Monsters")]
        [Tooltip("Sprite of the player monster. Left-sided sprite.")]
        [SerializeField] private BattleMonster _playerMonster;
        [Tooltip("Sprite of the enemy monster. Right-sided sprite.")]
        [SerializeField] private BattleMonster _enemyMonster;
        
        //TODO - Pool these or something, but they shouldn't stay this way.
        [SerializeField] private GameObject _beamSprite;
        [SerializeField] private GameObject _burstSprite;
        [SerializeField] private GameObject _crystalSprite;
        private GameObject _beamObj1;
        private GameObject _beamObj2;
        private GameObject _beamObj3;
        private GameObject _burstObj;
        private GameObject _crystalObj;
        
        /// <summary>
        /// Player's Battle scene character image.
        /// </summary>
        public Image PlayerImage => _playerImage;
        /// <summary>
        /// Enemy's Battle scene character image.
        /// </summary>
        public Image BattlerImage => _battlerImage;
        /// <summary>
        /// Player's BattleMonster in this Battle scene.
        /// </summary>
        public BattleMonster PlayerMonster => _playerMonster;
        /// <summary>
        /// Enemy's BattleMonster in this Battle scene.
        /// </summary>
        public BattleMonster EnemyMonster => _enemyMonster;

        /// <summary>
        /// Play the Capture Crystal animation.
        /// </summary>
        /// <param name="playerMonster">Player's monster</param>
        /// <param name="enemyMonster">Enemy's monster</param>
        /// <param name="beamCount">Count returned by capture algo determines the animation length.</param>
        /// <returns></returns>
        public IEnumerator PlayCrystalAnimation(BattleMonster playerMonster, BattleMonster enemyMonster, int beamCount)
        {
            // Factory or Flyweight pattern for this?
            /*
            * TODO - refactor this animation, i'm sure there's a better way to do this
            * My thinking was I wanted absolute control over where to position things and it seemed easiest
            * to just create this massive block of code to achieve it, but I'm sure my inexperience is showing here big time.
            * If it seems stupid, but it works, then I guess its not so stupid.
            */
            //TODO - instantiate these under _Dynamic
            // Build the animation points.
            Vector3 enemyPosition = enemyMonster.transform.position;
            Vector3 origin = playerMonster.transform.position - new Vector3(2, 0);
            Vector3 destination = enemyPosition - new Vector3(5, 2);

            // Deploy the capture crystal.
            _crystalObj = Instantiate(_crystalSprite, origin, Quaternion.identity);
            var crystal = _crystalObj.GetComponent<SpriteRenderer>();
            crystal.transform.localScale = new Vector3(0.2f, 0.2f);
            crystal.transform.DOScale(1, 1f);
            yield return crystal.transform.DOJump(destination, 2f, 1, 1f).WaitForCompletion();
            enemyMonster.PlayCaptureAnimation();

            // Send the first beam and instantiate the burst.
            _beamObj1 = Instantiate(_beamSprite, destination + new Vector3(0, 2), Quaternion.Euler(0, 0, 270));
            var beam1 = _beamObj1.GetComponent<SpriteRenderer>();
            yield return beam1.transform.DOLocalMove(enemyPosition, 0.5f);
            beam1.DOFade(0, 0.5f);

            _burstObj = Instantiate(_burstSprite, enemyPosition, Quaternion.identity);
            var burst = _burstObj.GetComponent<SpriteRenderer>();
            yield return YieldHelper.TWO_SECONDS;

            // Quit after 1 beam.
            if (beamCount == 0 || beamCount == 1)
            {
                yield break;
            }

            // Send the second beam and grow the burst.
            _beamObj2 = Instantiate(_beamSprite, destination + new Vector3(0, -2), Quaternion.Euler(0, 0, 310));
            var beam2 = _beamObj2.GetComponent<SpriteRenderer>();
            yield return beam2.transform.DOLocalMove(enemyPosition, 0.5f);
            beam2.DOFade(0, 0.5f);

            burst.transform.DOScale(5f, 0.5f);
            yield return YieldHelper.TWO_SECONDS;

            // Quit after 2 beams.
            if (beamCount == 2)
            {
                yield break;
            }

            // Send the final beam and grow the burst to max size.
            _beamObj3 = Instantiate(_beamSprite, destination, Quaternion.Euler(0, 0, 290));
            var beam3 = _beamObj3.GetComponent<SpriteRenderer>();
            yield return beam3.transform.DOLocalMove(enemyPosition, 0.5f);

            burst.transform.DOScale(10f, 0.5f);
            beam3.DOFade(0, 0.5f);
            yield return YieldHelper.TWO_SECONDS;

            // Quit after 3 beams.
            if (beamCount == 3)
            {
                yield break;
            }

            // Play the capture animation.
            enemyMonster.Image.DOFade(0, 0.1f);
            burst.transform.DOLocalMove(destination, 1f);
            yield return burst.DOFade(0, 1f);

            crystal.transform.DOScale(2f, 1.5f);
            yield return YieldHelper.ONE_SECOND;
            crystal.transform.DOLocalMove(origin, 1.5f);
            crystal.DOFade(0f, 1.5f);
            yield return YieldHelper.TWO_SECONDS;          
        }

        /// <summary>
        /// Clean up the GameObjects we created.
        /// </summary>
        public void CleanUp()
        {
            Destroy(_crystalObj, 0f);
            Destroy(_beamObj1, 0f);
            Destroy(_beamObj2, 0f);
            Destroy(_beamObj3, 0f);
            Destroy(_burstObj, 0f);
        }
        
        /// <summary>
        /// Plays the battle start animation for the provided monster.
        /// </summary>
        /// <param name="monster">Monster to animate.</param>
        public void PlayBattleStartAnimation(BattleMonster monster)
        {
            monster.Image.transform.localPosition = monster == _playerMonster ? 
                new Vector3(-500f, monster.OriginalPos.y) : new Vector3(500f, monster.OriginalPos.y);

            monster.Image.transform.DOLocalMoveX(monster.OriginalPos.x, 1f);
        }

        /// <summary>
        /// Play the failed capture animation after breaking out of PlayCaptureAnimation.
        /// </summary>
        public void PlayFailAnimation()
        {
            _burstObj.transform.DOPunchPosition(new Vector3(2, 0, 0), 2f);
            _burstObj.transform.DOScale(0.1f, 1f);
            _crystalObj.transform.DOPunchRotation(new Vector3(0, 0, 90), 2f);
            _crystalObj.transform.DOScale(0.1f, 1f);
        }
        
        /// <summary>
        /// Shows the character sprites in the Battle scene.
        /// </summary>
        /// <param name="player">The Player.</param>
        /// <param name="battler">The Battler.</param>
        public void ShowCharacterSprites(PlayerController player, BattlerController battler)
        {
            _playerMonster.gameObject.SetActive(false);
            _enemyMonster.gameObject.SetActive(false);
            _playerImage.gameObject.SetActive(true);
            _battlerImage.gameObject.SetActive(true);
            _playerImage.sprite = player.BattleSprite;
            _battlerImage.sprite = battler.Sprite;
        }
    }
}