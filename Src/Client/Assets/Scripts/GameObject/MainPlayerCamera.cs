﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public new Camera camera;
    public Transform viewPoint;

    public GameObject player;

    private void LateUpdate()
    {
        if (player == null)
        {
            player = User.Instance.CurrentCharacterObject;
        }
        if (player == null)
            return;

        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
    }
}
