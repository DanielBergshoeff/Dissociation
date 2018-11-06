﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    private float timer;

    private void Start() {
        timer = EnemyManager.TimePerAttack;
    }

    // Update is called once per frame
    void Update () {
        if (!EnemyManager.Paused) {
            if (Vector3.Distance(this.transform.position, EnemyManager.Target.transform.position) < 6.0f) {
                timer -= Time.deltaTime;
                if (timer <= 0) {
                    Debug.Log("Reset by enemy attack");
                    PersonalityController.thisPersonalityController.ResetGame();
                }
            }
            else {
                timer = EnemyManager.TimePerAttack;
            }
        }
	}
}