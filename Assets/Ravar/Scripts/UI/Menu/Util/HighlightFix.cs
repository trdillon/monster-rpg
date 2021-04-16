using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Menu
{
    [RequireComponent(typeof(Selectable))]
    public class HighlightFix : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!EventSystem.current.alreadySelecting)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }
 
        public void OnDeselect(BaseEventData eventData)
        {
            GetComponent<Selectable>().OnPointerExit(null);
        }
    }
}
