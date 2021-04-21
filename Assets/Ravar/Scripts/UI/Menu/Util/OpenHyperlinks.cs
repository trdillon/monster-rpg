using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar
{ 
    /// <summary>
    /// Helper component that opens hyperlinks defined in TextMesh Pro tags.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class OpenHyperlinks : MonoBehaviour, IPointerClickHandler 
    {
        // This class is from https://gitlab.com/jonnohopkins/tmp-hyperlinks
        
        [SerializeField] private bool _doesColorChangeOnHover = true;
        [SerializeField] private Color _hoverColor = new Color(60f / 255f, 120f / 255f, 1f);
        
        private int _currentLink = -1;
        private List<Color32[]> _originalVertexColors = new List<Color32[]>();

        private TextMeshProUGUI _textMeshPro;
        private Canvas _canvas;
        private Camera _camera;

        public bool IsLinkHighlighted => _currentLink != -1;

        private void Awake() 
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
            _canvas = GetComponentInParent<Canvas>();

            // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
            _camera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        }

        private void LateUpdate() 
        {
            // Is the cursor in the correct region (above the text area) and furthermore, in the link region?
            bool isHoveringOver = TMP_TextUtilities.IsIntersectingRectTransform(_textMeshPro.rectTransform, 
                                                                                Mouse.current.position.ReadValue(), 
                                                                                _camera);
            int linkIndex = isHoveringOver ? TMP_TextUtilities.FindIntersectingLink(_textMeshPro, 
                                                                                    Mouse.current.position.ReadValue(), 
                                                                                    _camera) : -1;

            // Clear previous link selection if one existed.
            if ( _currentLink != -1 && linkIndex != _currentLink ) 
            {
                SetLinkToColor(_currentLink, (linkIdx, vertIdx) => _originalVertexColors[linkIdx][vertIdx]);
                _originalVertexColors.Clear();
                _currentLink = -1;
            }

            // Handle new link selection.
            if (linkIndex == -1 ||
                linkIndex == _currentLink)
            {
                return;
            }
            
            _currentLink = linkIndex;
            if (_doesColorChangeOnHover)
            {
                _originalVertexColors = SetLinkToColor(linkIndex, (linkIdx, vertIdx) => _hoverColor);
            }
        }

        public void OnPointerClick(PointerEventData eventData) 
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_textMeshPro, Mouse.current.position.ReadValue(), _camera);
            if (linkIndex == -1)
            {
                return;
            }

            // Was a link clicked?
            TMP_LinkInfo linkInfo = _textMeshPro.textInfo.linkInfo[linkIndex];
            
            // Open the link id as a url, which is the metadata we added in the text field.
            Application.OpenURL(linkInfo.GetLinkID());
        }

        private List<Color32[]> SetLinkToColor(int linkIndex, Func<int, int, Color32> colorForLinkAndVert) 
        {
            TMP_LinkInfo linkInfo = _textMeshPro.textInfo.linkInfo[linkIndex];

            // Store the old character colors.
            var oldVertColors = new List<Color32[]>(); 

            // For each character in the link string.
            for (var i = 0; i < linkInfo.linkTextLength; i++) 
            { 
                // The character index into the entire text.
                int characterIndex = linkInfo.linkTextfirstCharacterIndex + i; 
                TMP_CharacterInfo charInfo = _textMeshPro.textInfo.characterInfo[characterIndex];
                // Get the index of the material / sub text object used by this character.
                int meshIndex = charInfo.materialReferenceIndex; 
                // Get the index of the first vertex of this character.
                int vertexIndex = charInfo.vertexIndex; 

                // The colors for this character.
                Color32[] vertexColors = _textMeshPro.textInfo.meshInfo[meshIndex].colors32; 
                oldVertColors.Add(vertexColors.ToArray());

                if (!charInfo.isVisible)
                { 
                    continue;
                }

                vertexColors[vertexIndex + 0] = colorForLinkAndVert(i, vertexIndex + 0);
                vertexColors[vertexIndex + 1] = colorForLinkAndVert(i, vertexIndex + 1);
                vertexColors[vertexIndex + 2] = colorForLinkAndVert(i, vertexIndex + 2);
                vertexColors[vertexIndex + 3] = colorForLinkAndVert(i, vertexIndex + 3);
            }

            // Update Geometry.
            _textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            return oldVertColors;
        }
    }
}