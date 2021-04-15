using UnityEngine;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// CorePack holds the essential game objects needed across all scenes.
    /// </summary>
    /// <remarks>Serves as a persistence container for these essential objects.</remarks>
    public class CorePack : MonoBehaviour
    {
        // We used to call DontDestroyOnLoad during Awake() to ensure the CorePack survived between scenes.
        // Starting in 1.0.8 a Game.Core scene is utilized to hold the CorePack so that is no longer needed.
        // Now this class is just used in the CorePackBuilder.Awake() function to call FindObjectsOfType<CorePack>
        // and ensure that we don't duplicate the CorePack.
    }
}
