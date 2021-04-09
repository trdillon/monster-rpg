using System.Collections;
using UnityEngine;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Handles visual changes in the HP bar displayed in the <see cref="BattleHud"/>.
    /// </summary>
    public class HpBar : MonoBehaviour
    {
        [Tooltip("GameObject that holds the monster's health.")]
        [SerializeField] private GameObject _health;

        /// <summary>
        /// Set the HP of the monster..
        /// </summary>
        /// <param name="hpNormalized">HP to set.</param>
        public void SetHp(float hpNormalized)
        {
            _health.transform.localScale = new Vector3(hpNormalized, 1f);
        }

        /// <summary>
        /// Slide the HP bar smoothly.
        /// </summary>
        /// <param name="newHp">HP at end of slide.</param>
        /// <returns>HP bar displaying current HP.</returns>
        public IEnumerator SlideHp(float newHp)
        {
            float currentHp = _health.transform.localScale.x;
            float amountToChange = currentHp - newHp;

            while (currentHp - newHp > Mathf.Epsilon)
            {
                currentHp -= amountToChange * Time.deltaTime;
                _health.transform.localScale = new Vector3(currentHp, 1f);
                yield return null;
            }

            _health.transform.localScale = new Vector3(newHp, 1f);
        }
    }
}