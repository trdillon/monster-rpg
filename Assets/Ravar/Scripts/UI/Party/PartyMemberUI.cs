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
        [Tooltip("The Text element that displays the monster's name.")]
        [SerializeField] Text nameText;
        [Tooltip("The Text element that displays the monster's level.")]
        [SerializeField] Text levelText;
        [Tooltip("The GameObject that holds the monster's HP bar.")]
        [SerializeField] HPBar hpBar;
        [Tooltip("The color to change the text to when highlighted.")]
        [SerializeField] Color highlightColor;
        [Tooltip("The color to display when the text is not highlighted.")]
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