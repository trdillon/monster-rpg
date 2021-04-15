using UnityEngine;

namespace Itsdits.Ravar.Data
{
    /// <summary>
    /// ScriptableObject used to hold data about a game Level scene.
    /// </summary>
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Scene Data/Level")]
    public class Level : GameScene
    {
        [Header("Level Data")]
        [Tooltip("Number of NPCs that appear in this scene.")]
        [SerializeField] private int _npcCount;
        [Tooltip("Number of Battlers that appear in this scene.")]
        [SerializeField] private int _battlerCount;
    }
}
