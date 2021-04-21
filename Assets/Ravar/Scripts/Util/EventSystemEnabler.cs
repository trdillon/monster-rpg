using System.Collections;
using Itsdits.Ravar.Util;
using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace Itsdits.Ravar
{
    /// <summary>
    /// Helper component that attaches to EventSystem objects to ensure the InputModule gets enabled when switching
    /// scenes. When changing between UI menu scenes the event system in the new scene sometimes becomes unresponsive.
    /// </summary>
    public class EventSystemEnabler : MonoBehaviour
    {
        private InputSystemUIInputModule _inputSystemUIInputModule;

        private void Start()
        {
            _inputSystemUIInputModule = GetComponentInParent<InputSystemUIInputModule>();
        }
 
        private void OnEnable()
        {
            StartCoroutine(ActivateInputComponent());
        }
 
        private IEnumerator ActivateInputComponent()
        {
            yield return YieldHelper.EndOfFrame;
            _inputSystemUIInputModule.enabled = false;
            yield return YieldHelper.FifthSecond;
            _inputSystemUIInputModule.enabled = true;
        }
    }
}