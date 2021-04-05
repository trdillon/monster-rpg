using DG.Tweening;
using Itsdits.Ravar.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Displays and manages the <see cref="MonsterObj"/> HUD during battle.
    /// </summary>
    public class BattleHUD : MonoBehaviour
    {
        [Header("Text Labels")]
        [Tooltip("Text element that displays the monster's name.")]
        [SerializeField] Text nameText;
        [Tooltip("Text element that displays the monster's level.")]
        [SerializeField] Text levelText;
        [Tooltip("Text element that displays the monster's status.")]
        [SerializeField] Text statusText;

        [Header("UI Bars")]
        [Tooltip("The HP bar of the monster.")]
        [SerializeField] HPBar hpBar;
        [Tooltip("The XP bar of the monster.")]
        [SerializeField] GameObject xpBar;

        [Header("Status Colors")]
        [Tooltip("Color to change the text to when the monster is poisoned.")]
        [SerializeField] Color psnColor;
        [Tooltip("Color to change the text to when the monster is burned.")]
        [SerializeField] Color brnColor;
        [Tooltip("Color to change the text to when the monster is asleep.")]
        [SerializeField] Color slpColor;
        [Tooltip("Color to change the text to when the monster is paralyzed.")]
        [SerializeField] Color parColor;
        [Tooltip("Color to change the text to when the monster is frozen.")]
        [SerializeField] Color frzColor;

        private Dictionary<ConditionID, Color> statusColors;
        private MonsterObj _monster;

        /// <summary>
        /// Set the data in the HUD.
        /// </summary>
        /// <param name="monster">Monster to set data for.</param>
        public void SetData(MonsterObj monster)
        {
            _monster = monster;
            nameText.text = monster.Base.Name;
            SetLevel();
            SetExp();
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
        /// <returns>New HP to display.</returns>
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
        /// <param name="reset">For when a monster levels up.</param>
        /// <returns>XP bar displaying current XP.</returns>
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