using UnityEngine;
using UnityEngine.SceneManagement;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// Builder class to ensure each scene has a single <see cref="CorePack"/>.
    /// </summary>
    public class CorePackBuilder : MonoBehaviour
    {
        [Tooltip("The prefab of the CorePack that should be built and maintained during play.")]
        [SerializeField] private GameObject _corePackPrefab;

        private void Awake()
        {
            CorePack[] existingPacks = FindObjectsOfType<CorePack>();
            if (existingPacks.Length != 0)
            {
                return;
            }

            // Make sure the CorePack is moved into the Game.Core scene or we're in trouble.
            GameObject corePack = Instantiate(_corePackPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            SceneManager.MoveGameObjectToScene(corePack, SceneManager.GetSceneByName("Game.Core"));
        }
    }
}