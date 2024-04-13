using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* SteeringBehaviour */
public abstract class SteeringBehaviour : MonoBehaviour {
    [SerializeField] protected float weight = 1.0f;

    public float Weight{
        get{
            return weight;    
        }
        set{
            weight = value;    
        }
    }

    public virtual void OnValidate(){
        if(weight < 0.0f){
            weight  = 0.0f;
            Debug.LogWarning("Weight must be non negative.", this);
        }        
    }

    public abstract void FixedUpdate();
}
