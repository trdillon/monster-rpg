using System.Collections.Generic;
using Itsdits.Ravar.Core.Signal;
using UnityEngine;

namespace Itsdits.Ravar
{
    /// <summary>
    /// Controller class for Level scenes.
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        [Tooltip("GameObject that holds the World objects such as terrain, portals, etc.")]
        [SerializeField] private GameObject _world;
        [Tooltip("List of the Characters that this scene contains.")]
        [SerializeField] private List<GameObject> _characters;

        private List<SpriteRenderer> _characterSprites = new List<SpriteRenderer>();

        private void OnEnable()
        {
            GameSignals.PAUSE_GAME.AddListener(OnPause);
            GameSignals.RESUME_GAME.AddListener(OnResume);
            BuildSpriteRendererList();
        }

        private void OnDisable()
        {
            GameSignals.PAUSE_GAME.RemoveListener(OnPause);
            GameSignals.RESUME_GAME.RemoveListener(OnResume);
        }

        private void OnPause(bool pause)
        {
            if (!pause)
            {
                return;
            }
            
            _world.SetActive(false);
            DisableSprites();
        }

        private void OnResume(bool resume)
        {
            if (!resume)
            {
                return;
            }

            _world.SetActive(true);
            EnableSprites();
        }

        // We keep a list of the SpriteRenderers to enable/disable them on pause. If the characters have their
        // GameObjects disabled while the Move() coroutine is running it will break them. This approach just
        // disables the sprites and allows the characters to finish the coroutine. Setting Time.timeScale = 0 will not
        // stop characters from moving. It also can cause other issues with input and audio so we want to avoid it.
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