using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* CheckpointBehaviour */
public class CheckpointBehaviour : MonoBehaviour {
    [SerializeField] private Collider2D region = null;

    public Vector2 Position{
        get{
            return transform.position;    
        }    
    }

    public void OnValidate(){
        if(region == null){
            Debug.LogWarning("There is no region.", this);
        }
    }

    public bool ContainsAgent(AgentBehaviour agent){
        return region.OverlapPoint(agent.Position);
    }
}
