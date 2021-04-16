using Itsdits.Ravar.Core.Signal;
using UnityEngine;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// Controller class for the Game.Core scene. Handles the CorePack and objects within it.
    /// </summary>
    public class CoreController : MonoBehaviour
    {
        [Tooltip("GameObject that serves as the parent to the CorePack and CorePackBuilder.")]
        [SerializeField] private GameObject _corePack;
        
        private void Awake()
        {
            GameSignals.PAUSE_GAME.AddListener(OnPause);
            GameSignals.RESUME_GAME.AddListener(OnResume);
        }

        private void OnPause(bool pause)
        {
            if (pause)
            {
                _corePack.SetActive(false);
            }
        }

        private void OnResume(bool resume)
        {
            if (resume)
            {
                _corePack.SetActive(true);
            }
        }
    }
}