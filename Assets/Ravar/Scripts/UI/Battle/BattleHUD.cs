using DG.Tweening;
using Itsdits.Ravar.Monster;
using Itsdits.Ravar.Monster.Condition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Battle
{
    public class BattleHUD : MonoBehaviour
    {
        #region config
        [SerializeField] Text nameText;
        [SerializeField] Text levelText;
        [SerializeField] Text statusText;
        [SerializeField] HPBar hpBar;
        [SerializeField] GameObject xpBar;
        [SerializeField] Color psnColor;
        [SerializeField] Color brnColor;
        [SerializeField] Color slpColor;
        [SerializeField] Color parColor;
        [SerializeField] Color frzColor;

        private Dictionary<ConditionID, Color> statusColors;
        private MonsterObj _monster;
        #endregion
        /// <summary>
        /// Set the data in the HUD.
        /// </summary>
        /// <param name="monster">Monster to set data for</param>
        public void SetData(MonsterObj monster)
        {
            _monster = monster;
            nameText.text = monster.Base.Name;
            SetLevel();
            SetExp();
            Debug.Log($"{monster.Base.Name} has {monster.CurrentHp} / {monster.MaxHp} HP.");
            hpBar.SetHP((float) monster.CurrentHp / monster.MaxHp);
            statusColors = new Dictionary<ConditionID, Color>()
            {
                { ConditionID.PSN, psnColor },
                { ConditionID.BRN, brnColor },
                { ConditionID.SLP, slpColor },
                { ConditionID.PAR, parColor },
                { ConditionID.FRZ, frzColor }
            };
            SetStatusText();
            _monster.OnStatusChange += SetStatusText;
        }

        /// <summary>
        /// Update the monster HP.
        /// </summary>
        /// <returns>New HP</returns>
        public IEnumerator UpdateHP()
        {
            if (_monster.IsHpChanged)
            {
                yield return hpBar.SlideHP((float)_monster.CurrentHp / _monster.MaxHp);
                _monster.IsHpChanged = false;
            }
        }

        /// <summary>
        /// Set the monster's level.
        /// </summary>
        public void SetLevel()
        {
            levelText.text = "Lvl " + _monster.Level;
        }

        /// <summary>
        /// Sets the XP bar.
        /// </summary>
        public void SetExp()
        {
            if (xpBar == null)
            {
                return;
            }

            float normalizedExp = GetNormalizedExp();
            xpBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
        }

        /// <summary>
        /// Slide the XP bar smoothly.
        /// </summary>
        /// <param name="reset">For when a monster levels up</param>
        /// <returns></returns>
        public IEnumerator SlideExp(bool reset = false)
        {
            if (xpBar == null)
            {
                yield break;
            }

            if (reset)
            {
                xpBar.transform.localScale = new Vector3(0, 1, 1);
            }

            float normalizedExp = GetNormalizedExp();
            yield return xpBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
        }

        private float GetNormalizedExp()
        {
            int currLevelExp = _monster.Base.GetExpForLevel(_monster.Level);
            int nextLevelExp = _monster.Base.GetExpForLevel(_monster.Level + 1);

            float normalizedExp = (float)(_monster.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
            return Mathf.Clamp(normalizedExp, 0, 1);
        }

        private void SetStatusText()
        {
            if (_monster.Status == null)
            {
                statusText.text = "";
            }
            else
            {
                statusText.text = _monster.Status.Id.ToString().ToUpper();
                statusText.color = statusColors[_monster.Status.Id];
            }
        }
    }
}