using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public static GameObject Target;
    public static float TimePerAttack;
    public static bool Paused;

    [SerializeField]
    private GameObject target;
    [SerializeField]
    private float timePerAttack;

    // Use this for initialization
    void Start () {
        Target = target;
        TimePerAttack = timePerAttack;
        Paused = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
