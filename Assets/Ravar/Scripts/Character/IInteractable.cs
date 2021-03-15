using UnityEngine;

namespace Itsdits.Ravar.Character
{
    /// <summary>
    /// Interface for interactable characters and objects.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Interact with characters, objects, world, etc.
        /// </summary>
        /// <param name="interactWith">What to interact with.</param>
        void Interact(Transform interactWith);
    }
}