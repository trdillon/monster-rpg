using System.Collections;
using UnityEngine;
using DG.Tweening;
using Itsdits.Ravar.Monster;

namespace Itsdits.Ravar.Animation
{
    /// <summary>
    /// Provides animation during battles.
    /// </summary>
    public class BattleAnimator : MonoBehaviour
    {
        [SerializeField] GameObject beamSprite;
        [SerializeField] GameObject burstSprite;
        [SerializeField] GameObject crystalSprite;

        private GameObject beamObj1;
        private GameObject beamObj2;
        private GameObject beamObj3;
        private GameObject burstObj;
        private GameObject crystalObj;

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
            var origin = playerMonster.transform.position - new Vector3(2, 0);
            var destination = enemyMonster.transform.position - new Vector3(5, 2);
            var enemyPos = enemyMonster.transform.position;

            // Deploy the capture crystal.
            crystalObj = Instantiate(crystalSprite, origin, Quaternion.identity);
            var crystal = crystalObj.GetComponent<SpriteRenderer>();
            crystal.transform.localScale = new Vector3(0.2f, 0.2f);
            crystal.transform.DOScale(1, 1f);
            yield return crystal.transform.DOJump(destination, 2f, 1, 1f).WaitForCompletion();
            enemyMonster.PlayCaptureAnimation();

            // Send the first beam and instantiate the burst.
            beamObj1 = Instantiate(beamSprite, destination + new Vector3(0, 2), Quaternion.Euler(0, 0, 270));
            var beam1 = beamObj1.GetComponent<SpriteRenderer>();
            yield return beam1.transform.DOLocalMove(enemyPos, 0.5f);
            beam1.DOFade(0, 0.5f);

            burstObj = Instantiate(burstSprite, enemyMonster.transform.position, Quaternion.identity);
            var burst = burstObj.GetComponent<SpriteRenderer>();
            yield return new WaitForSeconds(2f);

            // Quit after 1 beam.
            if (beamCount == 0 || beamCount == 1) yield break;

            // Send the second beam and grow the burst.
            beamObj2 = Instantiate(beamSprite, destination + new Vector3(0, -2), Quaternion.Euler(0, 0, 310));
            var beam2 = beamObj2.GetComponent<SpriteRenderer>();
            yield return beam2.transform.DOLocalMove(enemyPos, 0.5f);
            beam2.DOFade(0, 0.5f);

            burst.transform.DOScale(5f, 0.5f);
            yield return new WaitForSeconds(2f);

            // Quit after 2 beams.
            if (beamCount == 2) yield break;

            // Send the final beam and grow the burst to max size.
            beamObj3 = Instantiate(beamSprite, destination, Quaternion.Euler(0, 0, 290));
            var beam3 = beamObj3.GetComponent<SpriteRenderer>();
            yield return beam3.transform.DOLocalMove(enemyPos, 0.5f);

            burst.transform.DOScale(10f, 0.5f);
            beam3.DOFade(0, 0.5f);
            yield return new WaitForSeconds(2f);

            // Quit after 3 beams.
            if (beamCount == 3) yield break;

            // Play the capture animation.
            enemyMonster.Image.DOFade(0, 0.1f);
            burst.transform.DOLocalMove(destination, 1f);
            yield return burst.DOFade(0, 1f);

            crystal.transform.DOScale(2f, 1.5f);
            yield return new WaitForSeconds(1f);
            crystal.transform.DOLocalMove(origin, 1.5f);
            crystal.DOFade(0f, 1.5f);
            yield return new WaitForSeconds(2f);          
        }

        /// <summary>
        /// Clean up the GameObjects we created.
        /// </summary>
        public void CleanUp()
        {
            Debug.Log("Destroying animation objects.");
            Destroy(crystalObj, 0f);
            Destroy(beamObj1, 0f);
            Destroy(beamObj2, 0f);
            Destroy(beamObj3, 0f);
            Destroy(burstObj, 0f);
        }

        /// <summary>
        /// Play the failed capture animation after breaking out of PlayCaptureAnimation.
        /// </summary>
        public void PlayFailAnimation()
        {
            burstObj.transform.DOPunchPosition(new Vector3(2, 0, 0), 2f);
            burstObj.transform.DOScale(0.1f, 1f);
            crystalObj.transform.DOPunchRotation(new Vector3(0, 0, 90), 2f);
            crystalObj.transform.DOScale(0.1f, 1f);
        }
    }
}