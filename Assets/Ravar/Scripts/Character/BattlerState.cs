namespace Itsdits.Ravar.Character
{
	/// <summary>
	/// Possible states of a Battler. Determines if LoS is enabled or not and which dialog to display on interaction.
	/// </summary>
	/// <remarks>Possible states are Ready, Defeated and Locked.</remarks>
	public enum BattlerState
	{
		Ready,
		Defeated,
		Locked
	}
}