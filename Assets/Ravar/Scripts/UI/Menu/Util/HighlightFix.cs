using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Utility component that fixes a strange highlight issue in Unity when hovering the pointer away from a selection.
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    public class HighlightFix : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!EventSystem.current.alreadySelecting)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            GetComponent<Selectable>().OnPointerExit(null);
        }
    }
}