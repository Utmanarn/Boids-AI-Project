using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AttractorSteeringBehaviour */
public class AttractorSteeringBehaviour : SteeringBehaviour{
    [SerializeField] private AgentSetBehaviour agentSet = null;
    [SerializeField] private Collider2D innerRegion = null;

    public override void OnValidate(){
        base.OnValidate();
        if(agentSet == null){
            Debug.LogWarning("There is no agent set.", this);
        }
        if(innerRegion == null){
            Debug.LogWarning("There is no inne region.", this);
        }
    }

    private static bool TolerableDistance(Vector2 vector0, Vector2 vector1){
        const float TOLERANCE = 0.001f;
        return TOLERANCE < Vector2.Distance(vector0, vector1);
    }

    private Vector2 GetForce(AgentBehaviour agent){
        if(innerRegion.OverlapPoint(agent.Position)){
            return Vector2.zero;    
        }
        else{
            Vector2 targetPosition = transform.position;
            Vector2 agentPosition = agent.Position;
            if(TolerableDistance(targetPosition, agentPosition)){
                Vector2 agentToTarget = targetPosition - agentPosition;
                Vector2 force = agentToTarget.normalized;
                return force;
            }
            else{
                return Vector2.zero;    
            }
        }
    }

    public Vector2 GetWeightedForce(AgentBehaviour agent){
        Vector2 weightedForce = weight * GetForce(agent);
        return weightedForce;
    }


    private void DrawForceGizmos(AgentBehaviour agent){
        Vector2 forcePosition = agent.Position;
        Vector2 force = GetForce(agent);
        GizmosExtras.DrawForceVector(forcePosition, force);
    }

    private void DrawForceGizmos(){
        foreach(AgentBehaviour agent in agentSet.Agents){
            DrawForceGizmos(agent);
        }
    }

    public void OnDrawGizmosSelected(){
        if(agentSet != null && innerRegion != null){
            DrawForceGizmos();       
        }        
    }

    public override void FixedUpdate(){
        List<AgentBehaviour> agents = agentSet.Agents;
        for(int agentIndex = 0; agentIndex < agents.Count; agentIndex++){
            AgentBehaviour agent = agents[agentIndex];
            Vector2 weightedForce = GetWeightedForce(agent);
            agent.AddForce(weightedForce);
        }
    }
}
