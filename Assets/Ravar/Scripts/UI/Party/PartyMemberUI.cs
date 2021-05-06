using Itsdits.Ravar.Monster;
using Itsdits.Ravar.UI.Battle;
using TMPro;
using UnityEngine;

namespace Itsdits.Ravar.UI.Party
{
    /// <summary>
    /// Monster HUD UI element used in the Party selection scene.
    /// </summary>
    public class PartyMemberUI : MonoBehaviour
    {
        [Tooltip("The Text element that displays the monster's name.")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [Tooltip("The Text element that displays the monster's level.")]
        [SerializeField] private TextMeshProUGUI _levelText;
        [Tooltip("The GameObject that holds the monster's HP bar.")]
        [SerializeField] private HpBar _hpBar;

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
    }
}