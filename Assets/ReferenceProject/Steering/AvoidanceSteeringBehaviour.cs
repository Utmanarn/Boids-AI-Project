using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/* AvoidanceSteeringBehaviour */
public class AvoidanceSteeringBehaviour : SteeringBehaviour, IAgentSetHandler{

    /* AvoidanceManager */
    private class AvoidanceManager{
    
        /* RayRecord */
        private class RayRecord{
            private Vector2 direction;
            private float distance;
            private bool hit;
            
            public Vector2 Direction{
                get{
                    return direction;    
                }
                set{
                    direction = value;    
                }
            }

            public float Distance{
                get{
                    return distance;    
                }
                set{
                    distance = value;    
                }
            }

            public bool Hit{
                get{
                    return hit;    
                }
                set{
                    hit = value;    
                }
            }

            public RayRecord(Vector2 direction, float distance, bool hit){
                this.direction = direction;
                this.distance = distance;
                this.hit = hit;
            }

            public RayRecord() : this(Vector2.zero, 0.0f, false){    
            }
        }
        
        /* AgentRecord */
        private class AgentRecord{ 
            private AgentBehaviour agent;
            private List<RayRecord> rayRecords;
            private Vector2 force;

            public AgentBehaviour Agent{
                get{
                    return agent;    
                }    
            }

            public List<RayRecord> RayRecords{
                get{
                    return rayRecords;    
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
                rayRecords = new List<RayRecord>();
                force = Vector2.zero;
            }
        }

        private float weight;
        private float viewRange;
        private float viewAngle;
        private float brakeScaling;
        private float turnScaling;
        private int rayCount;
        private LayerMask layerMask;
        private List<AgentRecord> agentRecords;

        private void UpdateRayRecordCount(List<RayRecord> rayRecords){
            while(rayRecords.Count < rayCount){
                rayRecords.Add(new RayRecord());
            }
            while(rayCount < rayRecords.Count){
                rayRecords.RemoveAt(rayRecords.Count - 1);
            }
        }


        private Vector2 GetRayDirection(AgentBehaviour agent, int rayIndex){
            float rotation = agent.Rotation;
            float viewAngleDelta = viewAngle / rayCount;
            float minAngle = -0.5f * viewAngle;
            float rayAngle = rotation + minAngle + rayIndex * viewAngleDelta + 0.5f * viewAngleDelta;
            float rayAngleInRadians = Mathf.Deg2Rad * rayAngle;
            float directionX = Mathf.Cos(rayAngleInRadians);
            float directionY = Mathf.Sin(rayAngleInRadians);
            Vector2 rayDirection = new Vector2(directionX, directionY);
            return rayDirection;
        }

        private void UpdateRayRecords(AgentRecord agentRecord){
            AgentBehaviour agent = agentRecord.Agent;
            for(int rayRecordIndex = 0; rayRecordIndex < agentRecord.RayRecords.Count; rayRecordIndex++){
                RayRecord rayRecord = agentRecord.RayRecords[rayRecordIndex];
                rayRecord.Direction = GetRayDirection(agent, rayRecordIndex);
                RaycastHit2D raycastHit = Physics2D.Raycast(agent.Position, rayRecord.Direction, viewRange, layerMask);
                if(raycastHit.collider == null){
                    rayRecord.Distance = viewRange;
                    rayRecord.Hit = false;
                }
                else{
                    rayRecord.Distance = raycastHit.distance;
                    rayRecord.Hit = true;
                }
            }    
        }

        private static float GetInverseFraction(float part, float whole){
            return (whole - part) / whole;
        }

        private Vector2 ScaleBrakeAndTurn(AgentBehaviour agent, Vector2 force){
            Vector2 brakeForce = Vector2.Dot(force, agent.Forward) * agent.Forward;
            Vector2 turnForce = force - brakeForce;
            Vector2 scaledForce = brakeScaling * brakeForce + turnScaling * turnForce;
            return scaledForce;
        }

        private void UpdateForce(AgentRecord agentRecord){
            Vector2 forceSum = Vector2.zero;
            for(int rayRecordIndex = 0; rayRecordIndex < agentRecord.RayRecords.Count; rayRecordIndex++){
                RayRecord rayRecord = agentRecord.RayRecords[rayRecordIndex];
                if(rayRecord.Hit){
                    float distance = rayRecord.Distance;
                    Vector2 forceDirection = -rayRecord.Direction;
                    forceSum += GetInverseFraction(distance, viewRange) * forceDirection;
                }
            }
            Vector2 force = ScaleBrakeAndTurn(agentRecord.Agent, forceSum);
            agentRecord.Force = force;
        }


        private void UpdateAgentRecord(AgentRecord agentRecord){
            UpdateRayRecordCount(agentRecord.RayRecords);
            UpdateRayRecords(agentRecord);
            UpdateForce(agentRecord);
        }

        private void UpdateAgentRecords(){
            for(int i = 0; i < agentRecords.Count; i++){
                UpdateAgentRecord(agentRecords[i]);
            }    
        }

        private void UpdateAgents(){
            for(int i = 0; i < agentRecords.Count; i++){
                AgentRecord agentRecord = agentRecords[i];
                AgentBehaviour agent = agentRecord.Agent;
                Vector2 weightedForce = weight * agentRecord.Force;
                agent.AddForce(weightedForce);
            }    
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

        public float ViewAngle{
            set{
                if(viewAngle != value){
                    viewAngle = value;
                    UpdateAgentRecords();
                }    
            }    
        }

        public float BrakeScaling{
            set{
                if(brakeScaling != value){
                    brakeScaling = value;
                    UpdateAgentRecords();
                }    
            }    
        }

        public float TurnScaling{
            set{
                if(turnScaling != value){
                    turnScaling = value;
                    UpdateAgentRecords();
                }    
            }    
        }

        public int RayCount{
            set{
                if(rayCount != value){
                    rayCount = value;
                    UpdateAgentRecords();
                }    
            }    
        }

        public LayerMask LayerMask{
            set{
                if(layerMask != value){
                    layerMask = value;
                    UpdateAgentRecords();
                }    
            }    
        }

        public AvoidanceManager(List<AgentBehaviour> agents, LayerMask layerMask, float weight, float viewRange, float viewAngle, float brakeScaling, float turnScaling, int rayCount){
            this.weight = weight;
            this.viewRange = viewRange;
            this.viewAngle = viewAngle;
            this.brakeScaling = brakeScaling;
            this.turnScaling = turnScaling;
            this.rayCount = rayCount;
            this.layerMask = layerMask;
            agentRecords = new List<AgentRecord>();
            foreach(AgentBehaviour agent in agents){
                agentRecords.Add(new AgentRecord(agent));
            }
            UpdateAgentRecords();
        }

        public void FixedUpdate(){
            UpdateAgentRecords();
            UpdateAgents();
        }

        private Vector2 GetLocalViewPerimeterPosition(float angle){
            float angleInRadians = Mathf.Deg2Rad * angle;
            float x = viewRange * Mathf.Cos(angleInRadians);
            float y = viewRange * Mathf.Sin(angleInRadians);
            return new Vector2(x, y);
        }

        private Vector2 GetViewPerimeterPosition(AgentBehaviour agent, float angle){
            Vector2 localPosition = GetLocalViewPerimeterPosition(angle);
            Vector2 position = agent.transform.TransformPoint(localPosition);
            return position;
        }

        private float GetFraction(int part, int whole){
            return (float)part / whole;
        }

        private void DrawViewGizmos(AgentBehaviour agent){
            Gizmos.color = Color.grey;
            const int SEGMENT_COUNT = 32;
            float minAngle = -0.5f * viewAngle;
            float maxAngle = 0.5f * viewAngle;
            for(int segmentIndex = 0; segmentIndex < SEGMENT_COUNT; segmentIndex++){
                Vector2 p0 = GetViewPerimeterPosition(agent, minAngle + GetFraction(segmentIndex, SEGMENT_COUNT) * viewAngle);
                Vector2 p1 = GetViewPerimeterPosition(agent, minAngle + GetFraction(segmentIndex + 1, SEGMENT_COUNT) * viewAngle);
                Gizmos.DrawLine(p0, p1);
            }
            Vector2 agentPosition = agent.Position;
            Vector2 leftPosition = GetViewPerimeterPosition(agent, maxAngle);
            Vector2 rightPosition = GetViewPerimeterPosition(agent, minAngle);
            Gizmos.DrawLine(agentPosition, leftPosition);
            Gizmos.DrawLine(agentPosition, rightPosition);
        }

        private void DrawRayGizmos(AgentRecord agentRecord){
            Gizmos.color = Color.white;
            Vector2 agentPosition = agentRecord.Agent.Position;
            foreach(RayRecord rayRecord in agentRecord.RayRecords){
                Vector2 rayVector = rayRecord.Direction * rayRecord.Distance;
                GizmosExtras.DrawRayVector(agentPosition, rayVector);
            }
        }

        private void DrawForceGizmos(AgentRecord agentRecord){
            Vector2 agentPosition = agentRecord.Agent.Position;
            Vector2 force = agentRecord.Force;
            GizmosExtras.DrawForceVector(agentPosition, force);
        }

        public void OnDrawGizmosSelected(){
            for(int i = 0; i < agentRecords.Count; i++){
                AgentRecord agentRecord = agentRecords[i];
                DrawViewGizmos(agentRecord.Agent);
                DrawRayGizmos(agentRecord);
                DrawForceGizmos(agentRecord);
            }            
        }

        public void AddAgent(AgentBehaviour agent){
            if(0 == agentRecords.Count(agentRecord => agentRecord.Agent == agent)){
                agentRecords.Add(new AgentRecord(agent));
            }
        }
        
        public void RemoveAgent(AgentBehaviour agent){
            agentRecords.RemoveAll(agentRecord => agentRecord.Agent == agent);
        }
    }

    [SerializeField] private float viewRange = 1.0f;
    [SerializeField] [Range(0.0f, 360.0f)] private float viewAngle = 90.0f;
    [SerializeField] private float brakeScaling = 1.0f;
    [SerializeField] private float turnScaling = 1.0f;
    [SerializeField] private int rayCount = 2;
    [SerializeField] private LayerMask layerMask = new LayerMask();
    [SerializeField] private AgentSetBehaviour agentSet = null;
    private AvoidanceManager avoidanceManager = null;
    bool initialized = false;

    public override void OnValidate(){
        base.OnValidate();
        const float MIN_VIEW_RANGE = 0.001f;
        if(viewRange < MIN_VIEW_RANGE){
            viewRange = MIN_VIEW_RANGE;
            Debug.LogWarning("View range must be at least " + MIN_VIEW_RANGE + ".", this);
        }
        if(rayCount < 0){
            rayCount = 0;
            Debug.LogWarning("Ray count must be non negative.", this);
        }
        if(brakeScaling < 0.0f){
            brakeScaling = 0.0f;
            Debug.LogWarning("Brake scaling must be non negative.", this);
        }
        if(turnScaling < 0.0f){
            turnScaling = 0.0f;
            Debug.LogWarning("Turn scaling must be non negative.", this);
        }
        if(agentSet == null){
            Debug.LogWarning("There is no agent set.", this);
        }
    }

    public void Awake(){
        avoidanceManager = CreateAvoidanceManager();
        initialized = true;
    }

    public void OnDestroy(){
        initialized = false;
        avoidanceManager = null;
    }

    public void OnEnable(){
        agentSet.RegisterHandler(this);
    }

    public void OnDisable(){
        agentSet.UnregisterHandler(this);
    }

    public override void FixedUpdate(){
        avoidanceManager.Weight = weight;
        avoidanceManager.ViewRange = viewRange;
        avoidanceManager.ViewAngle = viewAngle;
        avoidanceManager.BrakeScaling = brakeScaling;
        avoidanceManager.TurnScaling = turnScaling;
        avoidanceManager.RayCount = rayCount;
        avoidanceManager.LayerMask = layerMask;
        avoidanceManager.FixedUpdate();        
    }

    private AvoidanceManager CreateAvoidanceManager(){
        return new AvoidanceManager(agentSet.Agents, layerMask, weight, viewRange, viewAngle, brakeScaling, turnScaling, rayCount);    
    }

    public void OnDrawGizmosSelected(){
        if(agentSet != null){
            AvoidanceManager avoidanceManager = initialized ? this.avoidanceManager : CreateAvoidanceManager();
            avoidanceManager.OnDrawGizmosSelected();
        }
    }

    public void OnAddAgent(AgentSetBehaviour agentSet, AgentBehaviour agent){
        avoidanceManager.AddAgent(agent);
    }

    public void OnRemoveAgent(AgentSetBehaviour agentSet, AgentBehaviour agent){
        avoidanceManager.RemoveAgent(agent);
    }
}
