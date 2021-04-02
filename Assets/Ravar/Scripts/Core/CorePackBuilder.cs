using Lowscope.Saving;
using UnityEngine;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// Builder class to ensure each scene has a single <see cref="CorePack"/>.
    /// </summary>
    public class CorePackBuilder : MonoBehaviour
    {
        [SerializeField] GameObject corePackPrefab;

        private void Awake()
        {
            var existingPacks = FindObjectsOfType<CorePack>();
            if (existingPacks.Length == 0)
            {
                //SaveMaster.SpawnSavedPrefab(Lowscope.Saving.Enums.InstanceSource.Resources, "Core/CorePack");
                Instantiate(corePackPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            }
        }
    }
}
