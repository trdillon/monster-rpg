using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Menu
{
    public class AutoScroll : MonoBehaviour
    {
        [SerializeField] private bool _debug;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Scrollbar _scrollbar;
        [SerializeField] private float _scrollPadding = 20f;

        private void Start () 
        {
            StartCoroutine(DetectScroll());
        }

        private IEnumerator DetectScroll () 
        {
            GameObject prevGo = null;
            var currentRect = new Rect();
            var viewRect = new Rect();
            var view = _scrollRect.GetComponent<RectTransform>();
         
            while (true) 
            {
                GameObject current = EventSystem.current.currentSelectedGameObject;
                if (current != null && current.transform.parent == transform) 
                {
                    // Get a cached instance of the RectTransform
                    if (current != prevGo) 
                    {
                        var rt = current.GetComponent<RectTransform>();
                     
                        // Create rectangles for comparison
                        currentRect = GetRect(current.transform.position, rt.rect, Vector2.zero);
                        viewRect = GetRect(_scrollRect.transform.position, view.rect, view.offsetMax);
                        Vector2 heading = currentRect.center - viewRect.center;
                        if (heading.y > 0f && !viewRect.Contains(currentRect.max)) 
                        {
                            float distance = Mathf.Abs(currentRect.max.y - viewRect.max.y) + _scrollPadding;
                            Vector2 anchoredPosition = view.anchoredPosition;
                            anchoredPosition = new Vector2(anchoredPosition.x, anchoredPosition.y - distance);
                            view.anchoredPosition = anchoredPosition;
                            if (_debug)
                            {
                                Debug.LogFormat("Scroll up {0}", distance.ToString(CultureInfo.CurrentCulture)); // Decrease y value
                            }
                        } 
                        else if (heading.y < 0f && !viewRect.Contains(currentRect.min)) 
                        {
                            float distance = Mathf.Abs(currentRect.min.y - viewRect.min.y) + _scrollPadding;
                            Vector2 anchoredPosition = view.anchoredPosition;
                            anchoredPosition = new Vector2(anchoredPosition.x, anchoredPosition.y + distance);
                            view.anchoredPosition = anchoredPosition;
                            if (_debug)
                            {
                                Debug.LogFormat("Scroll down {0}", distance.ToString(CultureInfo.CurrentCulture)); // Increase y value
                            }
                        }
                        // Get adjusted rectangle positions
                        currentRect = GetRect(current.transform.position, rt.rect, Vector2.zero);
                        viewRect = GetRect(_scrollRect.transform.position, view.rect, view.offsetMax);
                    }
                }
                prevGo = current;
             
                if (_debug) 
                {
                    DrawBoundary(viewRect, Color.cyan);
                    DrawBoundary(currentRect, Color.green);
                }
                yield return null;
            }
        }

        private static Rect GetRect (Vector3 pos, Rect rect, Vector2 offset) 
        {
            float x = pos.x + rect.xMin - offset.x;
            float y = pos.y + rect.yMin - offset.y;
            var xy = new Vector2(x, y);
         
            return new Rect(xy, rect.size);
        }
        
        public static void DrawBoundary (Rect rect, Color color) 
        {
            var topLeft = new Vector2(rect.xMin, rect.yMax);
            var bottomRight = new Vector2(rect.xMax, rect.yMin);
         
            Debug.DrawLine(rect.min, topLeft, color); // Top
            Debug.DrawLine(rect.max, topLeft, color); // Left
            Debug.DrawLine(rect.min, bottomRight, color); // Bottom
            Debug.DrawLine(rect.max, bottomRight, color); // Right
        }
    }
}
