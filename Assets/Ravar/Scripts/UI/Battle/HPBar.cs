using System.Collections;
using UnityEngine;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Handles visual changes in the HP bar displayed in the <see cref="BattleHUD"/>.
    /// </summary>
    public class HPBar : MonoBehaviour
    {
        [SerializeField] GameObject health;

        /// <summary>
        /// Set the HP.
        /// </summary>
        /// <param name="hpNormalized">HP</param>
        public void SetHP(float hpNormalized)
        {
            health.transform.localScale = new Vector3(hpNormalized, 1f);
        }

        /// <summary>
        /// Slide the HP bar smoothly.
        /// </summary>
        /// <param name="newHp">HP at end of slide</param>
        /// <returns></returns>
        public IEnumerator SlideHP(float newHp)
        {
            float currentHp = health.transform.localScale.x;
            float amountToChange = currentHp - newHp;

            while (currentHp - newHp > Mathf.Epsilon)
            {
                currentHp -= amountToChange * Time.deltaTime;
                health.transform.localScale = new Vector3(currentHp, 1f);
                yield return null;
            }

            health.transform.localScale = new Vector3(newHp, 1f);
        }
    }
}