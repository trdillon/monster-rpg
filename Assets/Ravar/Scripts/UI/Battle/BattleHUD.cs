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
            levelText.text = "Lvl " + monster.Level;
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
        /// Update the monster HP
        /// </summary>
        /// <returns>New HP</returns>
        public IEnumerator UpdateHP()
        {
            if (_monster.IsHpChanged)
            {
                yield return hpBar.SetHPSlider((float)_monster.CurrentHp / _monster.MaxHp);
                _monster.IsHpChanged = false;
            }
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