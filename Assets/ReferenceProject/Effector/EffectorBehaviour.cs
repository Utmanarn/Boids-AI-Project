using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* EffectorBehaviour */
public abstract class EffectorBehaviour : MonoBehaviour, IObstacleSetHandler{
    [SerializeField] protected ObstacleSetBehaviour obstacleSet = null;
    
    public virtual void OnValidate(){
        if(obstacleSet == null){
            Debug.LogWarning("There is no obstacle set.");
        }    
    }

    public virtual void OnDrawGizmosSelected(){
    }

    public virtual void OnEnable(){
        obstacleSet.RegisterHandler(this);
    }

    public virtual void OnDisable(){
        obstacleSet.UnregisterHandler(this);    
    }

    public virtual void FixedUpdate(){            
    }

    public virtual void Update(){
    }

    public virtual void OnAddObstacle(ObstacleSetBehaviour obstacleSet, ObstacleBehaviour obstacle){
    }

    public virtual void OnRemoveObstacle(ObstacleSetBehaviour obstacleSet, ObstacleBehaviour obstacle){
    }
}
