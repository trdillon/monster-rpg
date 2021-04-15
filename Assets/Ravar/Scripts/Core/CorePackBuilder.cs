using UnityEngine;
using UnityEngine.SceneManagement;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// Builder class to ensure each scene has a single <see cref="CorePack"/>.
    /// </summary>
    public class CorePackBuilder : MonoBehaviour
    {
        [Tooltip("The GameObject that serves as a parent to the CorePack.")]
        [SerializeField] private GameObject _parent;
        [Tooltip("The prefab of the CorePack that should be built and maintained during play.")]
        [SerializeField] private GameObject _corePackPrefab;

        private void Awake()
        {
            CorePack[] existingPacks = FindObjectsOfType<CorePack>();
            if (existingPacks.Length != 0)
            {
                return;
            }

            // How can we pass _parent as a parameter on instantiate if its in a different scene?
            GameObject corePack = Instantiate(_corePackPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            SceneManager.MoveGameObjectToScene(corePack, SceneManager.GetSceneByName("Game.Core"));
            corePack.transform.parent = _parent.transform;
        }
    }
}