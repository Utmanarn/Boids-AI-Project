using System;
using UnityEngine;

/* GroupSteeringBehaviour */
public class GroupSteeringBehaviour : SteeringBehaviour{
    [SerializeField] private AgentSetBehaviour agentSet = null;

    public override void OnValidate(){
        base.OnValidate();
        if(agentSet == null){
            Debug.LogWarning("There is no agent set.", this);
        }
    }

    private Vector2 GetCentrumPosition(){
        if(0 < agentSet.Agents.Count){
            Vector2 positionSum = Vector2.zero;
            for(int agentIndex = 0; agentIndex < agentSet.Agents.Count; agentIndex++){
                positionSum += agentSet.Agents[agentIndex].Position;
            }
            Vector2 centrumPosition = positionSum / agentSet.Agents.Count;
            return centrumPosition;
        }
        else{
            return Vector2.zero;    
        }
    }

    private Vector2 GetForce(AgentBehaviour agent, Vector2 centrumPosition){
        const float FORCE_SCALING = 0.1f;
        Vector2 agentPosition = agent.Position;
        Vector2 agentToCentrum = centrumPosition - agentPosition;
        Vector2 force = FORCE_SCALING * agentToCentrum;
        return force;
    }

    private Vector2 GetWeightedForce(AgentBehaviour agent, Vector2 centrumPosition){
        Vector2 weightedForce = weight * GetForce(agent, centrumPosition);
        return weightedForce;
    }

    private void DrawCentrumGizmos(Vector2 centrumPosition){
        const int SEGMENT_COUNT = 32;
        const float RADIUS = 0.1f;
        Gizmos.color = Color.grey;
        GizmosExtras.DrawCircle(centrumPosition, SEGMENT_COUNT, RADIUS);
    }

    private void DrawForceGizmos(Vector2 centrumPosition){
        foreach(AgentBehaviour agent in agentSet.Agents){
            Vector2 force = GetForce(agent, centrumPosition);
            GizmosExtras.DrawForceVector(agent.Position, force);
        }
    }

    public void OnDrawGizmosSelected(){
        if(agentSet != null){
            Vector2 centrumPosition = GetCentrumPosition();
            DrawCentrumGizmos(centrumPosition);
            DrawForceGizmos(centrumPosition);
        }        
    }

    public override void FixedUpdate(){
        Vector2 centrumPosition = GetCentrumPosition();
        for(int i = 0; i < agentSet.Agents.Count; i++){
            AgentBehaviour agent = agentSet.Agents[i];
            Vector2 weightedForce = GetWeightedForce(agent, centrumPosition);
            agent.AddForce(weightedForce);
        }
    }
}
