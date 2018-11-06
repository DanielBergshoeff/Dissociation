using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    [SerializeField]
    private float health;
    
    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            if(value <= 0 && health > 0)
            {
                Die();
            }
            health = value;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
