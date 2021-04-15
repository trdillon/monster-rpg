using UnityEngine;

namespace Itsdits.Ravar.Data
{
    /// <summary>
    /// ScriptableObject used to hold data about a game Menu scene.
    /// </summary>
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Scene Data/Menu")]
    public class Menu : GameScene
    {
        [Header("Menu Data")]
        [Tooltip("Type of Menu scene this menu is.")]
        [SerializeField] private MenuType _menuType;
    }
}
