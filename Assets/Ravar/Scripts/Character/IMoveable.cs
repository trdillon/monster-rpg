using System;
using System.Collections;
using UnityEngine;

namespace Itsdits.Ravar.Character
{
	/// <summary>  
	/// Interface for moveable characters and objects.
	/// </summary>
	public interface IMoveable
	{
		/// <summary>
		/// Move the character or object.
		/// </summary>
		/// <param name="moveVector">Where to move the character or object to.</param>
		/// <param name="OnMoveFinished">What to do after the move is finished.</param>
		IEnumerator Move(Vector2 moveVector, Action OnMoveFinished);


		/// <summary>
		/// Change the direction the character is facing. Used when interacted with to acknowledge the interaction.
		/// </summary>
		/// <param name="targetPos">The location to turn the character towards.</param>
		void ChangeDirection(Vector3 targetPos);
	}
}
	
