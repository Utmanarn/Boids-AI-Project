using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/* SpaceSteeringBehaviour */
public class SpaceSteeringBehaviour : SteeringBehaviour, IAgentSetHandler{

    /* SpaceManager */
    private class SpaceManager{

        /* AgentRecord */
        private class AgentRecord{
            AgentBehaviour agent;
            Vector2 force;

            public AgentBehaviour Agent{
                get{
                    return agent;    
                }    
            }

            public Vector2 Force{
                get{
                    return force;    
                }
                set{
                    force = value;    
                }
            }

            public AgentRecord(AgentBehaviour agent){
                this.agent = agent;
                this.force = Vector2.zero;
            }            
        }

        private float weight;
        private float viewRange;
        private List<AgentRecord> agentRecords;

        private float GetInverseFraction(float part, float whole){
            return (whole - part) / whole;
        }

        private static bool TolerableDistance(float distance){
            return 0.001f < distance;
        }

        private bool WithinViewRange(float distance){
            return distance < viewRange;
        }

        private void UpdateAgentRecordPair(AgentRecord agentRecord0, AgentRecord agentRecord1){
            Vector2 position0 = agentRecord0.Agent.Position;
            Vector2 position1 = agentRecord1.Agent.Position;
            Vector2 offset = position1 - position0;
            float distance = offset.magnitude;
            if(TolerableDistance(distance) && WithinViewRange(distance)){
                Vector2 direction = offset.normalized;
                Vector2 force = GetInverseFraction(distance, viewRange) * direction;
                agentRecord0.Force -= force;
                agentRecord1.Force += force;
            }
        }
 



        private void ResetAgentRecordForces(){
            for (int i = 0; i < agentRecords.Count; i++){
                agentRecords[i].Force = Vector2.zero;
            }
        }

        private void AccumulateAgentRecordForce(){
            for (int i = 0; i < agentRecords.Count; i++){
                AgentRecord agentRecord0 = agentRecords[i];
                for (int j = i + 1; j < agentRecords.Count; j++){
                    AgentRecord agentRecord1 = agentRecords[j];
                    UpdateAgentRecordPair(agentRecord0, agentRecord1);
                }
            }
        }

        private void UpdateAgentRecords()
        {
            ResetAgentRecordForces();
            AccumulateAgentRecordForce();
        }


        public float Weight{
            set{
                weight = value;    
            }    
        }

        public float ViewRange{
            set{
                if(viewRange != value){
                    viewRange = value;
                    UpdateAgentRecords();
                }    
            }
        }
        
        public SpaceManager(List<AgentBehaviour> agents, float weight, float viewRange){
            this.weight = weight;
            this.viewRange = viewRange;
            agentRecords = new List<AgentRecord>();
            foreach(AgentBehaviour agent in agents){
                AgentRecord agentRecord = new AgentRecord(agent);
                agentRecords.Add(agentRecord);
            }
            UpdateAgentRecords();
        }

        private void DrawViewGizmos(AgentBehaviour agent){
            const int SEGMENT_COUNT = 32;
            Gizmos.color = Color.grey;
            GizmosExtras.DrawCircle(agent.Position, SEGMENT_COUNT, viewRange);
        }

        private void DrawForceGizmos(AgentBehaviour agent, Vector2 force){
            GizmosExtras.DrawForceVector(agent.Position, force);            
        }

        public void OnDrawGizmosSelected(){
            foreach(AgentRecord agentRecord in agentRecords){
                DrawViewGizmos(agentRecord.Agent);
                DrawForceGizmos(agentRecord.Agent, agentRecord.Force);
            }        
        }

        private void UpdateAgents(){
            for(int i = 0; i < agentRecords.Count; i++){
                AgentBehaviour agent = agentRecords[i].Agent;
                Vector2 force = weight * agentRecords[i].Force;
                agent.AddForce(force);
            }    
        }

        public void FixedUpdate(){
            UpdateAgentRecords();
            UpdateAgents();
        }

        public void AddAgent(AgentBehaviour agent){
            if(0 == agentRecords.Count(agentRecord => agentRecord.Agent == agent)){
                AgentRecord agentRecord = new AgentRecord(agent);
                agentRecords.Add(agentRecord);
            }
        }
 
        public void RemoveAgent(AgentBehaviour agent){
            agentRecords.RemoveAll(agentRecord => agentRecord.Agent == agent);
        }
    }

    [SerializeField] private float viewRange = 1.0f;
    [SerializeField] private AgentSetBehaviour agentSet = null;
    private SpaceManager spaceManager = null;
    private bool initialized = false;

    public override void OnValidate(){
        base.OnValidate();
        const float MIN_VIEW_RANGE = 0.001f;
        if(viewRange < MIN_VIEW_RANGE){
            viewRange = MIN_VIEW_RANGE;
            Debug.LogWarning("View range must be at least " + MIN_VIEW_RANGE, this);
        }
        if(agentSet == null){
            Debug.LogWarning("There is no agent set.", this);
        }
    }

    private SpaceManager CreateAvoidanceManager(){
        return new SpaceManager(agentSet.Agents, weight, viewRange);  
    }

    public void OnDrawGizmosSelected(){
        if(agentSet != null){
            SpaceManager avoidanceManager = initialized ? this.spaceManager : CreateAvoidanceManager();
            avoidanceManager.OnDrawGizmosSelected();
        }
    }

    public void Awake(){
        spaceManager = CreateAvoidanceManager();
        initialized = true;
    }

    public void OnDestroy(){
        initialized = false;
        spaceManager = null;
    }

    public void OnEnable(){
        agentSet.RegisterHandler(this);
    }

    public void OnDisable(){
        agentSet.UnregisterHandler(this);
    }

    public override void FixedUpdate(){
        spaceManager.Weight = weight;
        spaceManager.ViewRange = viewRange;
        spaceManager.FixedUpdate();
    }

    public void OnAddAgent(AgentSetBehaviour agentSet, AgentBehaviour agent){
        spaceManager.AddAgent(agent);
    }

    public void OnRemoveAgent(AgentSetBehaviour agentSet, AgentBehaviour agent){
        spaceManager.RemoveAgent(agent);
    }
}
