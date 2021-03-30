using UnityEngine;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// CorePack holds the essential game objects needed across all scenes. It serves as persistence for these objects.
    /// </summary>
    public class CorePack : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
