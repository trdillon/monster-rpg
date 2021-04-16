using System.Collections.Generic;
using Itsdits.Ravar.Core.Signal;
using UnityEngine;

namespace Itsdits.Ravar
{
    /// <summary>
    /// Controller class for Level scenes. Handles GameObjects within that scene and disabling on pause.
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        [Tooltip("GameObject that holds the World objects such as terrain, portals, etc.")]
        [SerializeField] private GameObject _world;
        [Tooltip("List of the Characters that this scene contains.")]
        [SerializeField] private List<GameObject> _characters;

        private List<SpriteRenderer> _characterSprites = new List<SpriteRenderer>();

        private void Awake()
        {
            GameSignals.PAUSE_GAME.AddListener(OnPause);
            GameSignals.RESUME_GAME.AddListener(OnResume);
            BuildSpriteRendererList();
        }
        
        private void OnPause(bool pause)
        {
            if (pause)
            {
                _world.SetActive(false);
                DisableSprites();
            }
        }

        private void OnResume(bool resume)
        {
            if (resume)
            {
                _world.SetActive(true);
                EnableSprites();
            }
        }

        // We keep a list of the SpriteRenderers to enable/disable them on pause. If the characters have their
        // GameObjects disabled while the Move() coroutine is processing it will break them. This approach just
        // disables the sprites and allows the characters to finish the coroutine.
        private void BuildSpriteRendererList()
        {
            foreach (GameObject character in _characters)
            {
                var sprite = character.GetComponent<SpriteRenderer>();
                _characterSprites.Add(sprite);
            }
        }

        private void DisableSprites()
        {
            foreach (SpriteRenderer sprite in _characterSprites)
            {
                sprite.enabled = false;
            }
        }

        private void EnableSprites()
        {
            foreach (SpriteRenderer sprite in _characterSprites)
            {
                sprite.enabled = true;
            }
        }
    }
}