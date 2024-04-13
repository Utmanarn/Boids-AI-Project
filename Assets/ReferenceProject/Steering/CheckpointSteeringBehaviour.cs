using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* CheckpointSteeringBehaviour */
public class CheckpointSteeringBehaviour : SteeringBehaviour{

    /* State */
    private abstract class State{
        private State parentState;
        private CheckpointSteeringBehaviour checkpointSteering;

        public State ParentState{
            get{
                return parentState;
            }    
        }

        public CheckpointSteeringBehaviour CheckpointSteering{
            get{
                return checkpointSteering;    
            }    
        }

        public float CenterDuration{
            get{
                return checkpointSteering.centerDuration;    
            }    
        }

        public float WaintDuration{
            get{
                return checkpointSteering.waitDuration;    
            }    
        }

        public AgentSetBehaviour AgentSet{
            get{
                return checkpointSteering.agentSet;                
            }    
        }

        public List<CheckpointBehaviour> Checkpoints{
            get{
                return checkpointSteering.checkpoints;    
            }    
        }
        
        public State(State parentState, CheckpointSteeringBehaviour checkpointSteering){
            this.parentState = parentState;
            this.checkpointSteering = checkpointSteering;
        }

        public CheckpointBehaviour GetFirstCheckpoint(){
            return checkpointSteering.GetFirstCheckpoint();    
        }

        public CheckpointBehaviour GetNextCheckpoint(CheckpointBehaviour currentCheckpoint){
            return checkpointSteering.GetNextCheckpoint(currentCheckpoint);
        }

        public Vector2 GetForce(AgentBehaviour agent, CheckpointBehaviour checkpoint){
            return checkpointSteering.GetForce(agent, checkpoint);
        }

        public Vector2 GetWeightedForce(AgentBehaviour agent, CheckpointBehaviour checkpoint){
            return checkpointSteering.GetWeightedForce(agent, checkpoint);
        }

        public void DrawForceGizmos(AgentBehaviour agent, CheckpointBehaviour checkpoint){
            checkpointSteering.DrawForceGizmos(agent, checkpoint);
        }

        public void DrawForceGizmos(List<AgentBehaviour> agents, CheckpointBehaviour checkpoint){
            checkpointSteering.DrawForceGizmos(agents, checkpoint);
        }

        public void DrawCheckpointGizmos(CheckpointBehaviour activeCheckpoint){
            checkpointSteering.DrawCheckpointGizmos(activeCheckpoint);    
        }

        public abstract void Transit(State fromState, State toState);
        public abstract void Enter();
        public abstract void Exit();
        public abstract bool Seek(CheckpointBehaviour currentCheckpoint);
        public abstract bool Center(CheckpointBehaviour currentCheckpoint);
        public abstract bool Wait(CheckpointBehaviour currentCheckpoint);
        public abstract bool OnDrawGizmosSelected();
        public abstract bool FixedUpdate();
    }

    /* LeafState */
    private abstract class LeafState : State{
        
    
        public LeafState(State parentState, CheckpointSteeringBehaviour checkpointSteering) : base(parentState, checkpointSteering){            
        }
        
        public override void Transit(State fromState, State toState){
            ParentState.Transit(fromState, toState);    
        }

        public override void Enter(){
        }

        public override void Exit(){
        }

        public override bool Seek(CheckpointBehaviour currentCheckpoint){
            return false;    
        }

        public override bool Center(CheckpointBehaviour currentCheckpoint){
            return false;    
        }

        public override bool Wait(CheckpointBehaviour currentCheckpoint){
            return false;    
        }

        public override bool OnDrawGizmosSelected(){
            return false;    
        }

        public override bool FixedUpdate(){
            return false;    
        }
    }

    /* SeekState */
    private class SeekState : LeafState{
        private CheckpointBehaviour currentCheckpoint;
    
        public SeekState(State parentState, CheckpointSteeringBehaviour checkpointSteering, CheckpointBehaviour currentCheckpoint) : base(parentState, checkpointSteering){
            this.currentCheckpoint = currentCheckpoint;
        }

        public override bool OnDrawGizmosSelected(){
            DrawCheckpointGizmos(currentCheckpoint);
            DrawForceGizmos(AgentSet.Agents, currentCheckpoint);
            return true;
        }

        private void AddForceToAgents(){
            for(int agentIndex = 0; agentIndex < AgentSet.Agents.Count; agentIndex++){
                AgentBehaviour agent = AgentSet.Agents[agentIndex];
                Vector2 weightedForce = GetWeightedForce(agent, currentCheckpoint);
                agent.AddForce(weightedForce);
            }            
        }

        private bool CurrentCheckpointContainsOneAgent(){
            for(int agentIndex = 0; agentIndex < AgentSet.Agents.Count; agentIndex++){
                AgentBehaviour agent = AgentSet.Agents[agentIndex];
                if(currentCheckpoint.ContainsAgent(agent)){
                    return true;
                }
            }
            return false;
        }

        public override bool FixedUpdate(){
            AddForceToAgents();
            if(CurrentCheckpointContainsOneAgent()){
                Transit(this, new CenterState(ParentState, CheckpointSteering, currentCheckpoint));
            }
            return true;
        }
    }

    /* CenterState */
    private class CenterState : LeafState{
        private CheckpointBehaviour currentCheckpoint;
        private Timer timer;
    
        public CenterState(State parentState, CheckpointSteeringBehaviour checkpointSteering, CheckpointBehaviour currentCheckpoint) : base(parentState, checkpointSteering){
            this.currentCheckpoint = currentCheckpoint;
            this.timer = new Timer(CenterDuration);
        }        

        public override bool OnDrawGizmosSelected(){
            DrawCheckpointGizmos(currentCheckpoint);
            DrawForceGizmos(AgentSet.Agents, currentCheckpoint);
            return true;
        }

        private void AddForceToAgents(){
            for(int agentIndex = 0; agentIndex < AgentSet.Agents.Count; agentIndex++){
                AgentBehaviour agent = AgentSet.Agents[agentIndex];
                Vector2 weightedForce = GetWeightedForce(agent, currentCheckpoint);
                agent.AddForce(weightedForce);
            }            
        }

        public override bool FixedUpdate(){
            AddForceToAgents();
            timer.Time += Time.deltaTime;
            if(timer.Expired){
                Transit(this, new WaitState(ParentState, CheckpointSteering, currentCheckpoint));
            }
            return true;
        }
    }

    /* WaitState */
    private class WaitState : LeafState{
        private CheckpointBehaviour currentCheckpoint;
        private Timer timer;
    
        public WaitState(State parentState, CheckpointSteeringBehaviour checkpointSteering, CheckpointBehaviour currentCheckpoint) : base(parentState, checkpointSteering){
            this.currentCheckpoint = currentCheckpoint;
            timer = new Timer(WaintDuration);
        }        

        public override bool OnDrawGizmosSelected(){
            DrawCheckpointGizmos(currentCheckpoint);
            return true;
        }

        public override bool FixedUpdate(){
            timer.Time += Time.deltaTime;
            if(timer.Expired){
                Transit(this, new SeekState(ParentState, CheckpointSteering, GetNextCheckpoint(currentCheckpoint)));
            }
            return true;
        }        
    }

    /* CheckpointSteeringState */
    private class CheckpointSteeringState : State{
        State currentState;
            
        public CheckpointSteeringState(CheckpointSteeringBehaviour checkpointSteering) : base(null, checkpointSteering){
            currentState = new SeekState(this, checkpointSteering, GetFirstCheckpoint());
        }
        
        public override void Transit(State fromState, State toState){
            if(currentState == fromState){
                currentState.Exit();
                currentState = toState;
                toState.Enter();
            }
            else{
                ParentState.Transit(fromState, toState);    
            }
        }

        public override void Enter(){
            currentState.Enter();
        }

        public override void Exit(){
            currentState.Exit();
        }

        public override bool Seek(CheckpointBehaviour currentCheckpoint){
            return currentState.Seek(currentCheckpoint);    
        }

        public override bool Center(CheckpointBehaviour currentCheckpoint){
            return currentState.Center(currentCheckpoint);
        }

        public override bool Wait(CheckpointBehaviour currentCheckpoint){
            return currentState.Wait(currentCheckpoint);    
        }

        public override bool OnDrawGizmosSelected(){
            return currentState.OnDrawGizmosSelected();
        }

        public override bool FixedUpdate(){
            return currentState.FixedUpdate();    
        }
    }


    [SerializeField] private float centerDuration = 1.0f;
    [SerializeField] private float waitDuration = 1.0f;
    [SerializeField] private AgentSetBehaviour agentSet = null;
    [SerializeField] private List<CheckpointBehaviour> checkpoints = new List<CheckpointBehaviour>();
    private State state = null;
    private bool initialized = false;

    private CheckpointBehaviour GetFirstCheckpoint(){
        return checkpoints[0];    
    }

    private CheckpointBehaviour GetNextCheckpoint(CheckpointBehaviour currentCheckpoint){
        int currentIndex = checkpoints.IndexOf(currentCheckpoint);
        int nextIndex = (currentIndex + 1) % checkpoints.Count;
        CheckpointBehaviour nextCheckpoint = checkpoints[nextIndex];
        return nextCheckpoint;
    }

    private static bool TolerableDistance(Vector2 vector0, Vector2 vector1){
        const float TOLERANCE = 0.001f;
        return TOLERANCE < Vector2.Distance(vector0, vector1);
    }

    private Vector2 GetForce(AgentBehaviour agent, CheckpointBehaviour checkpoint){
        Vector2 agentPosition = agent.Position;
        Vector2 checkpointPosition = checkpoint.Position;
        if(TolerableDistance(agentPosition, checkpointPosition)){
            Vector2 offset = checkpointPosition - agentPosition;
            Vector2 direction = offset.normalized;
            return direction;
        }
        else{
            return Vector2.zero;
        }
    }

    private Vector2 GetWeightedForce(AgentBehaviour agent, CheckpointBehaviour checkpoint){
        Vector2 force = GetForce(agent, checkpoint);
        Vector2 weightedtForce = weight * force;
        return weightedtForce;
    }

    private void DrawForceGizmos(AgentBehaviour agent, CheckpointBehaviour checkpoint){
        Vector2 forcePosition = agent.Position;
        Vector2 ForceVector = GetForce(agent, checkpoint);
        GizmosExtras.DrawForceVector(forcePosition, ForceVector);
    }

    private void DrawForceGizmos(List<AgentBehaviour> agents, CheckpointBehaviour checkpoint){
        foreach(AgentBehaviour agent in agents){
            DrawForceGizmos(agent, checkpoint);
        }
    }

    private void DrawCheckpointGizmos(CheckpointBehaviour activeCheckpoint){
        const int SEGMENT_COUNT = 32;
        const float RADIUS = 1.0f;
        foreach(CheckpointBehaviour currentCheckpoint in checkpoints){
            CheckpointBehaviour nextCheckpoint = GetNextCheckpoint(currentCheckpoint);
            Gizmos.color = Color.grey;
            Gizmos.DrawLine(currentCheckpoint.Position, nextCheckpoint.Position);
            Gizmos.color = currentCheckpoint == activeCheckpoint ? Color.green : Color.grey;
            GizmosExtras.DrawCircle(currentCheckpoint.Position, SEGMENT_COUNT, RADIUS);
        }            
    }

    private void RemoveInvalidCheckpoints(){
        checkpoints.RemoveAll(checkpoint => checkpoint == null);    
    }

    public override void OnValidate(){
        base.OnValidate();
        if(centerDuration < 0.0f){
            centerDuration = 0.0f;
            Debug.LogWarning("center duration must be non negative.", this);
        }
        if(waitDuration < 0.0f){
            waitDuration = 0.0f;
            Debug.LogWarning("waing duration must be non negative.", this);
        }
        if(agentSet == null){
            Debug.LogWarning("There is no agent set.", this);
        }
        RemoveInvalidCheckpoints();
        if(checkpoints.Count == 0){
            Debug.LogWarning("There are no checkpoints.", this);
        }
    }

    public void OnDrawGizmosSelected(){
        RemoveInvalidCheckpoints();
        if(agentSet != null && 0 < checkpoints.Count){
            State state = initialized ? this.state : new CheckpointSteeringState(this);
            state.OnDrawGizmosSelected();
        }
    }

    public void Awake(){
        state = new CheckpointSteeringState(this);
        state.Enter();
        initialized = true;
    }

    public void OnDestroy(){
        initialized = false;
        state.Exit();
        state = null;
    }

    public override void FixedUpdate(){
        state.FixedUpdate();
    }
}
