using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/* AgentWanderState */
public class AgentWanderState{
    private AgentBehaviour agent;
    private float angle;

    public AgentWanderState(AgentBehaviour agent, float angle){
        this.agent = agent;
        this.angle = angle;
    }

    public AgentBehaviour Agent{
        get{
            return agent;    
        }
        set{
            agent = value;    
        }
    }

    public float Angle{
        get{
            return angle;    
        }
        set{
            angle = value;    
        }
    }
}

/* WanderSteeringBehaviour */
public class WanderSteeringBehaviour : SteeringBehaviour, IAgentSetHandler{
    [SerializeField] private float offset = 1.0f;
    [SerializeField] private float radius = 1.0f;
    [SerializeField] [Range(0.0f, 1.0f)] private float delta = 0.5F;
    [SerializeField] private AgentSetBehaviour agentSet = null;
    private List<AgentWanderState> agentWanderStates = new List<AgentWanderState>();
    private readonly int GIZMO_SEGMENT_COUNT = 32;

    public override void OnValidate(){
        base.OnValidate();
        if(offset < 0.0f){
            offset = 0.0f;
            Debug.LogWarning("Offset must be non negative.", this);
        }
        if(radius < 0.0f){
            radius = 0.0f;
            Debug.LogWarning("Radius must be non negative.", this);
        }
        if(agentSet == null){
            Debug.LogWarning("There is no agent set.", this);    
        }
    }

    private float GetRandomAngle(){
        return Random.Range(0.0f, 360.0f);    
    }

    private void SynchAgentWanderStates(){
        agentSet.Agents.RemoveAll(agent => agent == null);
        agentWanderStates.RemoveAll(agentWanderState => agentWanderState.Agent == null);
        agentWanderStates.RemoveAll(agentWanderState => !agentSet.Agents.Contains(agentWanderState.Agent));
        foreach(AgentBehaviour agent in agentSet.Agents){
            if(!agentWanderStates.Any(agentWanderState => agentWanderState.Agent == agent)){
                agentWanderStates.Add(new AgentWanderState(agent, GetRandomAngle()));
            }
        }
    }

    private Vector2 GetLocalCirclePosition(){
        return new Vector2(offset + radius, 0.0f);
    }

    private Vector2 GetCirclePosition(AgentBehaviour agent){
        return agent.transform.TransformPoint(GetLocalCirclePosition());
    }

    private Vector2 GetLocalPerimeterPosition(float angle){
        float angleInRadians = angle * Mathf.Deg2Rad;
        float localX = offset + radius + radius * Mathf.Cos(angleInRadians);
        float localY = radius * Mathf.Sin(angleInRadians);
        Vector2 localPosition = new Vector2(localX, localY);
        return localPosition;
    }

    private Vector2 GetPerimeterPosition(AgentBehaviour agent, float angle){
        return agent.transform.TransformPoint(GetLocalPerimeterPosition(angle));
    }

    private Vector2 GetForce(AgentBehaviour agent, float angle){
        return agent.transform.TransformVector(GetLocalPerimeterPosition(angle));    
    }

    private Vector2 GetForce(AgentWanderState agentWanderState){
        return GetForce(agentWanderState.Agent, agentWanderState.Angle);    
    }

    private Vector2 GetWeightedForce(AgentBehaviour agent, float angle){
        return weight * GetForce(agent, angle);    
    }

    private Vector2 GetWeightedForce(AgentWanderState agentWanderState){
        return GetWeightedForce(agentWanderState.Agent, agentWanderState.Angle);    
    }

    private float DeltaAngle{
        get{
            return 360.0f * delta;     
        }
    }

    private void DrawCircleGizmo(AgentBehaviour agent){
        Gizmos.color = Color.grey;
        for(int segmentIndex = 0; segmentIndex < GIZMO_SEGMENT_COUNT; segmentIndex++){
            float fromAngle = segmentIndex * (360.0f / GIZMO_SEGMENT_COUNT);
            float toAngle = (segmentIndex + 1) * (360.0f / GIZMO_SEGMENT_COUNT);
            Vector2 fromPosition = GetPerimeterPosition(agent, fromAngle);
            Vector2 toPosition = GetPerimeterPosition(agent, toAngle);
            Gizmos.DrawLine(fromPosition, toPosition);
        }
    }

    private void DrawDeltaGizmo(AgentWanderState agentWanderState){
        Gizmos.color = Color.red;
        float baseAngle = agentWanderState.Angle - 0.5f * DeltaAngle;
        for(int segmentIndex = 0; segmentIndex < GIZMO_SEGMENT_COUNT; segmentIndex++){
            float fromAngle = baseAngle + segmentIndex * (DeltaAngle / GIZMO_SEGMENT_COUNT);
            float toAngle = baseAngle + (segmentIndex + 1) * (DeltaAngle / GIZMO_SEGMENT_COUNT);
            Vector2 fromPosition = GetPerimeterPosition(agentWanderState.Agent, fromAngle);
            Vector2 toPosition = GetPerimeterPosition(agentWanderState.Agent, toAngle);
            Gizmos.DrawLine(fromPosition, toPosition);
        }
    }

    public void DrawForceGizmo(AgentWanderState agentWanderState){
        Vector2 position = agentWanderState.Agent.Position;
        Vector2 force = GetForce(agentWanderState);
        GizmosExtras.DrawForceVector(position, force);
    }

    public void OnDrawGizmosSelected(){
        if(agentSet != null && !agentSet.HasHandler(this)){
            SynchAgentWanderStates();
        }
        foreach(AgentWanderState agentWanderState in agentWanderStates){
            DrawCircleGizmo(agentWanderState.Agent);
            DrawDeltaGizmo(agentWanderState);
            DrawForceGizmo(agentWanderState);
        }
    }

    public void OnEnable(){
        SynchAgentWanderStates();
        agentSet.RegisterHandler(this);
    }

    public void OnDisable(){
        agentSet.UnregisterHandler(this);
    }

    private void StepAngle(AgentWanderState agentWanderState){
        agentWanderState.Angle += Random.Range(-0.5f * DeltaAngle, 0.5f * DeltaAngle);
    }

    private void AddForceToAgent(AgentWanderState agentWanderState){
        agentWanderState.Agent.AddForce(GetWeightedForce(agentWanderState));
    }

    public override void FixedUpdate(){
        for(int i = 0; i < agentWanderStates.Count; i++){
            AgentWanderState agentWanderState = agentWanderStates[i];
            StepAngle(agentWanderState);
            AddForceToAgent(agentWanderState);
        }    
    }

    public void OnAddAgent(AgentSetBehaviour agentSet, AgentBehaviour agent){
        if(agentSet == this.agentSet && !agentWanderStates.Any(agentWanderState => agentWanderState.Agent == agent)){
            agentWanderStates.Add(new AgentWanderState(agent, GetRandomAngle()));
        }
    }

    public void OnRemoveAgent(AgentSetBehaviour agentSet, AgentBehaviour agent){
        if(agentSet == this.agentSet && agentWanderStates.Any(agentWanderState => agentWanderState.Agent == agent)){
            agentWanderStates.RemoveAll(agentWanderState => agentWanderState.Agent == agent);
        }
    }
}
