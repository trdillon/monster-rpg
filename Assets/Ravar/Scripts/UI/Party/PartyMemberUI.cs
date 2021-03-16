using Itsdits.Ravar.Monster;
using Itsdits.Ravar.UI.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Party { 
    public class PartyMemberUI : MonoBehaviour
    {
        [SerializeField] Text nameText;
        [SerializeField] Text levelText;
        [SerializeField] HPBar hpBar;
        [SerializeField] Color highlightColor;
        [SerializeField] Color standardColor;

        private MonsterObj _monster;

        /// <summary>
        /// Populate the UI with Monster data.
        /// </summary>
        /// <param name="monster">Monster in party</param>
        public void SetData(MonsterObj monster)
        {
            _monster = monster;
            nameText.text = monster.Base.Name;
            levelText.text = "Lvl " + monster.Level;
            hpBar.SetHP((float)monster.CurrentHp / monster.MaxHp);
        }

        /// <summary>
        /// Show which Monster is selected.
        /// </summary>
        /// <param name="selected">Is the monster selected or not</param>
        public void SetSelected(bool selected)
        {
            if (selected)
            {
                nameText.color = highlightColor;
            }
            else
            {
                nameText.color = standardColor;
            }
        }
    }
}