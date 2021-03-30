using Itsdits.Ravar.Character.Player;

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
