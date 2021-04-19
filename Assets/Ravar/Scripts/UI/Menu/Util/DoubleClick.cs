using UnityEngine;
using UnityEngine.EventSystems;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Helper component that detects a double click event.
    /// </summary>
    /// <remarks>Abstract class that provides base functionality. Inherited classes may call different
    /// functions to handle a double click event.</remarks>
    public abstract class DoubleClick : MonoBehaviour, IPointerClickHandler
    {
        /// <summary>
        /// Event triggered when player clicks on the GameObject with this component.
        /// </summary>
        /// <param name="eventData">PointerEventData is used to access clickCount.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            int clickCount = eventData.clickCount;

            if (clickCount == 2)
            {
                OnDoubleClick();
            }
        }

        protected virtual void OnDoubleClick()
        {
            Debug.Log("Double click action.");
        } 
    }
}