using Itsdits.Ravar.Monster;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Visual representation of an individual party member in the <see cref="PartyScreen"/>.
    /// </summary>
    public class PartyMemberUI : MonoBehaviour
    {
        [SerializeField] Text nameText;
        [SerializeField] Text levelText;
        [SerializeField] HPBar hpBar;
        [SerializeField] Color highlightColor;
        [SerializeField] Color standardColor;

        /// <summary>
        /// Populate the UI with Monster data.
        /// </summary>
        /// <param name="monster">Monster in party.</param>
        public void SetData(MonsterObj monster)
        {
            nameText.text = monster.Base.Name;
            levelText.text = "Lvl " + monster.Level;
            hpBar.SetHP((float)monster.CurrentHp / monster.MaxHp);
        }

        /// <summary>
        /// Show which Monster is selected.
        /// </summary>
        /// <param name="selected">Is the monster selected or not.</param>
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