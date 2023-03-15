namespace  DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.InputSystem;

	public class UI_Player : MonoBehaviour
	{
		private Control _inputs = null;
		private bool _moving = false;
		[SerializeField] private float movementSpeed = 10f;
		public Vector2 move;

		void Update()
		{
			MovePlayer();
		}


		public void OnMove(InputAction.CallbackContext context)
		{
			move = context.ReadValue<Vector2>();
		}

		public void MovePlayer()
		{
			
			Vector3 movement = new Vector3(move.x, 0f, move.y);
			if(movement != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
				transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
			}
		}
	}

}