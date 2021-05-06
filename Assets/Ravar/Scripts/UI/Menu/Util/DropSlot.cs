using UnityEngine;
using UnityEngine.EventSystems;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// 
    /// </summary>
    public class DropSlot : MonoBehaviour, IDropHandler
    {
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = _rectTransform.anchoredPosition;
            }
        }
    }
}