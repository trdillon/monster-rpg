using UnityEngine;

namespace Itsdits.Ravar.Character
{
    /// <summary>
    /// Interface for interactable characters and objects.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Interact with characters, objects, etc.
        /// </summary>
        /// <param name="interactor">Who or what to interact with.</param>
        void InteractWith(Transform interactor);
    }
}