using UnityEngine;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// CorePack holds the essential game objects needed across all scenes.
    /// </summary>
    /// <remarks>Serves as persistence for these essential objects.</remarks>
    public class CorePack : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
