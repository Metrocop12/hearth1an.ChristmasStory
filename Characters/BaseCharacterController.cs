﻿using ChrismasStory.Components;
using System.Collections;
using UnityEngine;

namespace ChrismasStory.Characters
{
	internal abstract class BaseCharacterController : MonoBehaviour
	{
		public GameObject originalCharacter, shipCharacter, treeCharacter;

        public enum STATE
        {
			NONE,
            ORIGINAL,
            ON_SHIP,
            AT_TREE
        };
		
		public STATE State { get; private set; }      

		
        public void ChangeState(STATE newState)
		{
			if (State != newState)
            {
				// Blink for 2 seconds means 1 second to close eyes then 1 second to open
				// Right in the middle we change the state

				if (State == STATE.ORIGINAL && newState == STATE.AT_TREE)
				{
					// If going from original position to tree, they are taking their own ship
					StartCoroutine(FlyShipCoroutine(newState));
				}
				else if (State == STATE.ORIGINAL && newState == STATE.ON_SHIP)
				{
					// From original position into the ship they are walking to the ship and into the hatch
					StartCoroutine(ShipCoroutine(newState));
				}
				else if (State == STATE.ON_SHIP && newState == STATE.AT_TREE)
				{
					// Exit hatch then walk
					StartCoroutine(ShipCoroutine(newState));
				}
				else
				{
					// This shouldn't happen but just in case
					StartCoroutine(ChangeStateCoroutine(2f, newState));
				}

				OnChangeState(State, newState);

				State = newState;
			}		
		}

		protected abstract void OnChangeState(STATE oldState, STATE newState);

		private IEnumerator ShipCoroutine(STATE state)
		{
			PlayerEffectController.CloseEyes(0.7f);
			yield return new WaitForSeconds(0.7f);

			// Eyes closed: swap character state
			OnSetState(state);

			PlayerEffectController.PlayAudioOneShot(AudioType.ShipHatchOpen, 0.3f);
			yield return new WaitForSeconds(1f);

			PlayerEffectController.PlayAudioOneShot(AudioType.ShipHatchClose, 0.3f);

			yield return new WaitForSeconds(0.3f);

			// Open eyes
			PlayerEffectController.OpenEyes(0.7f);
		}

		private IEnumerator FlyShipCoroutine(STATE state)
		{
			PlayerEffectController.CloseEyes(1f);
			yield return new WaitForSeconds(1f);

			// Eyes closed: swap character state
			OnSetState(state);

			// Play ship takeoff sound
			PlayerEffectController.PlayAudioOneShot(AudioType.ShipThrustIgnition, 0.3f);
			yield return new WaitForSeconds(1f);

			// Open eyes
			PlayerEffectController.OpenEyes(1f);
		}

		private IEnumerator ChangeStateCoroutine(float wait, STATE state)
		{
			PlayerEffectController.Blink(2);

			yield return new WaitForSeconds(wait);
			
			OnSetState(state);
		}

		private void OnSetState(STATE state)
		{
			originalCharacter?.SetActive(state == STATE.ORIGINAL);
			shipCharacter?.SetActive(state == STATE.ON_SHIP);
			treeCharacter?.SetActive(state == STATE.AT_TREE);
		}
	}
}
