using UnityEngine;
using UnityEngine.EventSystems;

namespace Itsdits.Ravar
{
    /// <summary>
    /// Abstract Menu Controller class that provides access to local scene management functions.
    /// </summary>
    public abstract class MenuController : MonoBehaviour
    {
        [Header("Scene Management")]
        [Tooltip("Event System object in this scene.")]
        [SerializeField] private EventSystem _eventSystem;
        [Tooltip("Camera for this scene.")]
        [SerializeField] private Camera _camera;

        private AudioListener _audioListener;
        
        protected EventSystem EventSystem => _eventSystem;
        protected Camera Camera => _camera;
        protected AudioListener AudioListener { get; set; }

        protected virtual void EnableSceneManagement()
        {
            _eventSystem.enabled = true;
            _audioListener = _camera.GetComponent<AudioListener>();
            _audioListener.enabled = true;
        }
        
        protected virtual void DisableSceneManagement()
        {
            _eventSystem.enabled = false;
            _audioListener.enabled = false;
        }
    }
}