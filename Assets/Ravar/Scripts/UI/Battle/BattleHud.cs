using DG.Tweening;
using Itsdits.Ravar.Monster;
using System.Collections;
using System.Collections.Generic;
using Itsdits.Ravar.Monster.Condition;
using TMPro;
using UnityEngine;

namespace Itsdits.Ravar.UI.Battle
{
    /// <summary>
    /// Displays and manages the <see cref="MonsterObj"/> HUD during battle.
    /// </summary>
    public class BattleHud : MonoBehaviour
    {
        [Header("Text Labels")]
        [Tooltip("Text element that displays the monster's name.")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [Tooltip("Text element that displays the monster's level.")]
        [SerializeField] private TextMeshProUGUI _levelText;
        [Tooltip("Text element that displays the monster's status.")]
        [SerializeField] private TextMeshProUGUI _statusText;

        [Header("UI Bars")]
        [Tooltip("The HP bar of the monster.")]
        [SerializeField] private HpBar _hpBar;
        [Tooltip("The XP bar of the monster.")]
        [SerializeField] private GameObject _xpBar;

        [Header("Status Colors")]
        [Tooltip("Color to change the text to when the monster is poisoned.")]
        [SerializeField] private Color _psnColor;
        [Tooltip("Color to change the text to when the monster is burned.")]
        [SerializeField] private Color _brnColor;
        [Tooltip("Color to change the text to when the monster is asleep.")]
        [SerializeField] private Color _slpColor;
        [Tooltip("Color to change the text to when the monster is paralyzed.")]
        [SerializeField] private Color _parColor;
        [Tooltip("Color to change the text to when the monster is frozen.")]
        [SerializeField] private Color _frzColor;

        private Dictionary<ConditionID, Color> _statusColors;
        private MonsterObj _monster;

        /// <summary>
        /// Set the data in the HUD.
        /// </summary>
        /// <param name="monster">Monster to set data for.</param>
        public void SetData(MonsterObj monster)
        {
            _monster = monster;
            _nameText.text = monster.Base.Name;
            SetLevel();
            SetExp();
            _hpBar.SetHp((float) monster.CurrentHp / monster.MaxHp);
            _statusColors = new Dictionary<ConditionID, Color>
            {
                { ConditionID.Poison, _psnColor },
                { ConditionID.Burn, _brnColor },
                { ConditionID.Sleep, _slpColor },
                { ConditionID.Paralyze, _parColor },
                { ConditionID.Freeze, _frzColor }
            };
            SetStatusText();
            _monster.OnStatusChange += SetStatusText;
        }

        /// <summary>
        /// Update the monster HP.
        /// </summary>
        /// <returns>New HP to display.</returns>
        public IEnumerator UpdateHp()
        {
            if (!_monster.IsHpChanged)
            {
                yield break;
            }

            yield return _hpBar.SlideHp((float)_monster.CurrentHp / _monster.MaxHp);
            _monster.IsHpChanged = false;
        }

        /// <summary>
        /// Set the monster's level.
        /// </summary>
        public void SetLevel()
        {
            _levelText.text = "Lvl " + _monster.Level;
        }
        
        /// <summary>
        /// Slide the XP bar smoothly.
        /// </summary>
        /// <param name="reset">For when a monster levels up.</param>
        /// <returns>XP bar displaying current XP.</returns>
        public IEnumerator SlideExp(bool reset = false)
        {
            if (_xpBar == null)
            {
                yield break;
            }

            if (reset)
            {
                _xpBar.transform.localScale = new Vector3(0, 1, 1);
            }

            float normalizedExp = GetNormalizedExp();
            yield return _xpBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
        }
        
        private void SetExp()
        {
            if (_xpBar == null)
            {
                return;
            }

            float normalizedExp = GetNormalizedExp();
            _xpBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
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
                _statusText.text = "";
            }
            else
            {
                //TODO - add status icons to replace the status text here
                _statusText.text = _monster.Status.Id.ToString().ToUpper();
                _statusText.color = _statusColors[_monster.Status.Id];
            }
        }
    }
}