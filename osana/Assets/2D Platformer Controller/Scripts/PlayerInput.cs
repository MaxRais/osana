﻿using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if (Input.GetButtonDown("Jump"))
        {
            player.OnJumpInputDown();
        }

        if (Input.GetButtonUp("Jump"))
        {
            player.OnJumpInputUp();
        }

		if (Input.GetKeyDown (KeyCode.Mouse0) || Input.GetKeyDown (KeyCode.X) )
		{
			player.ShootProjectile ();
		}
		if (Input.GetKeyDown (KeyCode.Mouse1) || Input.GetKeyDown (KeyCode.C) )
		{
			player.Dash ();
		}
		if (Input.GetKey (KeyCode.LeftShift) )
		{
			player.SetDirectionalInput (Vector2.zero);
		}
    }
}
