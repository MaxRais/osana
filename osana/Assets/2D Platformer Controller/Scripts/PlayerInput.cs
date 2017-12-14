﻿using UnityEngine;
using UnityEngine.SceneManagement;

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

		if (Input.GetButtonDown("Fire1"))
		{
			player.ShootProjectile ();
		}
		if (Input.GetButtonDown("Fire2") )
		{
			player.Dash ();
		}
		if (Input.GetButton("Fire3") || Input.GetButton("Fire4"))
		{
			player.SetDirectionalInput (Vector2.zero);
		}
		if (Input.GetButtonDown ("Start"))
			SceneManager.LoadScene ("Menu");
    }
}
