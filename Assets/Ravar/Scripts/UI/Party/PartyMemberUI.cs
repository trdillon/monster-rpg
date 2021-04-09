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
        [SerializeField] private Text _nameText;
        [Tooltip("The Text element that displays the monster's level.")]
        [SerializeField] private Text _levelText;
        [Tooltip("The GameObject that holds the monster's HP bar.")]
        [SerializeField] private HpBar _hpBar;
        [Tooltip("The color to change the text to when highlighted.")]
        [SerializeField] private Color _highlightColor;
        [Tooltip("The color to display when the text is not highlighted.")]
        [SerializeField] private Color _standardColor;

        /// <summary>
        /// Populate the UI with Monster data.
        /// </summary>
        /// <param name="monster">Monster in party.</param>
        public void SetData(MonsterObj monster)
        {
            _nameText.text = monster.Base.Name;
            _levelText.text = "Lvl " + monster.Level;
            _hpBar.SetHp((float)monster.CurrentHp / monster.MaxHp);
        }

        /// <summary>
        /// Show which Monster is selected.
        /// </summary>
        /// <param name="selected">Is the monster selected or not.</param>
        public void SetSelected(bool selected)
        {
            _nameText.color = selected ? _highlightColor : _standardColor;
        }
    }
}