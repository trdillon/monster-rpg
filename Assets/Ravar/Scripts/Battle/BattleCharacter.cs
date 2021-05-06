using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.Character
{
    /// <summary>
    /// Class for referencing and accessing the battle character's image and position.
    /// </summary>
    public class BattleCharacter : MonoBehaviour
    {
        private Image _image;
        private Vector3 _originalPos;

        /// <summary>
        /// Sprite of the battle character.
        /// </summary>
        public Image Image => _image;
        /// <summary>
        /// Original position of the sprite.
        /// </summary>
        public Vector3 OriginalPos => _originalPos;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
            _originalPos = _image.transform.localPosition;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}