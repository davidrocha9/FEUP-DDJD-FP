using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;

		public bool reload;

		public bool interact;

		public bool startRound;

		public bool aim;

		public bool fire;

		public bool weapon1;

		public bool weapon2;

		public bool weapon3;

		public bool weapon4;

		public int weaponScroll;

		public bool melee;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnReload(InputValue value)
		{
			ReloadInput(value.isPressed);
		}


		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
		}

		public void OnStartRound(InputValue value)
		{
			StartRoundInput(value.isPressed);
		}

		public void OnAim(InputValue value)
		{
			AimInput(value.isPressed);
		}

		public void OnFire(InputValue value)
		{
			FireInput(value.isPressed);
		}

		public void OnWeapon1(InputValue value){
			Weapon1Input(value.isPressed);
		}
		public void OnWeapon2(InputValue value){
			Weapon2Input(value.isPressed);
		}
		public void OnWeapon3(InputValue value){
			Weapon3Input(value.isPressed);
		}
		public void OnWeapon4(InputValue value){
			Weapon4Input(value.isPressed);
		}

		public void OnWeaponScroll(InputValue value){
			ScrollInput(value.Get<Vector2>());
		}

		public void OnMelee(InputValue value){
			MeleeInput(value.isPressed);
		}

#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void ReloadInput(bool newReloadState)
		{
			reload = newReloadState;
		}

		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
		}

		public void StartRoundInput(bool newStartRoundState)
		{
			startRound = newStartRoundState;
		}

		public void AimInput(bool newAimState)
		{
			aim = newAimState;
		}

		public void FireInput(bool newFireState)
		{
			fire = newFireState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		public void Weapon1Input(bool weapon1State){
			weapon1 = weapon1State;
		}

		public void Weapon2Input(bool weapon2State){
			weapon2 = weapon2State;
		}

		public void Weapon3Input(bool weapon3State){
			weapon3 = weapon3State;
		}

		public void Weapon4Input(bool weapon4State){
			weapon4 = weapon4State;
		}

		public void ScrollInput(Vector2 scrollVec){
			float scrollValue = scrollVec.y;
			if(scrollValue > 0){
				weaponScroll = 1;
			}
			if(scrollValue < 0){
				weaponScroll = -1;
			}
			if(scrollValue == 0){
				weaponScroll = 0;
			}
		}

		public void MeleeInput(bool meleeState){
			melee = meleeState;
		}
	}
	
}