using Itsdits.Ravar.Character;

namespace Itsdits.Ravar.Levels
{
    /// <summary>
    /// Interface for triggerable layers and game objects.
    /// </summary>
    public interface ITriggerable
    {
        void OnTriggered(PlayerController player);
    }
}
