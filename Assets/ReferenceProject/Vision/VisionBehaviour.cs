using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

/* VisionState */
public class VisionState{
    private AgentBehaviour sourceAgent;
    private List<AgentBehaviour> targetAgents;

    public AgentBehaviour SourceAgent{
        get{
            return sourceAgent;    
        }    
    }

    public List<AgentBehaviour> TargetAgents{
        get{
            return targetAgents;    
        }    
    }
    
    public VisionState(AgentBehaviour sourceAgent){
        this.sourceAgent = sourceAgent;
        targetAgents = new List<AgentBehaviour>();
    }

    public void AddTargetAgent(AgentBehaviour targetAgent){
        targetAgents.Add(targetAgent);
    }

    public void RemoveTargetAgent(AgentBehaviour targetAgent){
        targetAgents.Remove(targetAgent);
    }

    public void ClearTargetAgents(){
        targetAgents.Clear();   
    }
}

/* VisionBehaviour */
public class VisionBehaviour : MonoBehaviour, IAgentSetHandler{

    /* VisionStateManager */
    private class VisionStateManager{
    
        /* FixedVisionStateManager */
        private class FixedVisionStateManager{

            /* OverlapState */
            private struct OverlapState : IEquatable<OverlapState>{
                private bool overlapX;
                private bool overlapY;

                private bool InternalEquals(OverlapState overlapState){
                    return overlapX == overlapState.overlapX && overlapY == overlapState.overlapY; 
                }               

                public bool OverlapX{
                    get{
                        return overlapX;    
                    }    
                }

                public bool OverlapY{
                    get{
                        return overlapY;    
                    }    
                }

                public bool Overlap{
                    get{
                        return overlapX && overlapY;    
                    }    
                }

                public OverlapState(bool overlapX, bool overlapY){
                    this.overlapX = overlapX;
                    this.overlapY = overlapY;
                }

                public OverlapState SetOverlapX(bool overlapX){
                    return new OverlapState(overlapX, overlapY);
                }

                public OverlapState SetOverlapY(bool overlapY){
                    return new OverlapState(overlapX, overlapY);
                }

                public override bool Equals(object o){
                    if(o == null || o.GetType() != GetType()){
                        return false;
                    }
                    else{
                        return InternalEquals((OverlapState)o);    
                    }
                }

                public override int GetHashCode(){
                    return 0;
                }

                public bool Equals(OverlapState overlapState){
                    return InternalEquals(overlapState);
                }
                
                public static bool operator==(OverlapState overlapState0, OverlapState overlapState1){
                    return overlapState0.Equals(overlapState1);
                }

                public static bool operator!=(OverlapState overlapState0, OverlapState overlapState1){
                    return !overlapState0.Equals(overlapState1);
                }
            }

            /* OverlapStateManager */
            private class OverlapStateManager{
                private int agentCount;
                private OverlapState[,] overlapStateMatrix;
                
                public int AgentCount{
                    get{
                        return agentCount;    
                    }    
                }

                public OverlapStateManager(int agentCount){
                    this.agentCount = agentCount;
                    overlapStateMatrix = new OverlapState[agentCount, agentCount];
                }

                public bool GetOverlapX(int agentIndex0, int agentIndex1){
                    return overlapStateMatrix[agentIndex0, agentIndex1].OverlapX;
                }

                public bool GetOverlapY(int agentIndex0, int agentIndex1){
                    return overlapStateMatrix[agentIndex0, agentIndex1].OverlapY;
                }

                public bool GetOverlap(int agentIndex0, int agentIndex1){
                    return overlapStateMatrix[agentIndex0, agentIndex1].Overlap;
                }

                public OverlapState GetOverlapState(int agentIndex0, int agentIndex1){
                    return overlapStateMatrix[agentIndex0, agentIndex1];
                }

                public void SetOverlapState(int agentIndex0, int agentIndex1, OverlapState overlapState){
                    overlapStateMatrix[agentIndex0, agentIndex1] = overlapStateMatrix[agentIndex1, agentIndex0] = overlapState;                    
                }
                
                public void SetOverlapX(int agentIndex0, int agentIndex1, bool overlapX){
                    SetOverlapState(agentIndex0, agentIndex1, overlapStateMatrix[agentIndex0, agentIndex1].SetOverlapX(overlapX));    
                }

                public void SetOverlapY(int agentIndex0, int agentIndex1, bool overlapY){
                    SetOverlapState(agentIndex0, agentIndex1, overlapStateMatrix[agentIndex0, agentIndex1].SetOverlapY(overlapY));    
                }
            }

            /* AgentRecord */
            private class AgentRecord{
                private AgentBehaviour agent;
                private int agentIndex;
                private float viewRange;
                private List<AgentRecord> activeAgentRecords;
                private VisionState visionState;
                private Rect boundary;
                
                private static Vector2 GetBoundaryPosition(AgentBehaviour agent, float viewRange){
                    float x = agent.Position.x - viewRange;
                    float y = agent.Position.y - viewRange;
                    Vector2 boundaryPosition = new Vector2(x, y);
                    return boundaryPosition;
                }

                private static Vector2 GetBoundarySize(float viewRange){
                    float size = 2.0f * viewRange;
                    Vector2 boundarySize = new Vector2(size, size);
                    return boundarySize;
                }

                private static Rect GetBoundary(AgentBehaviour agent, float viewRange){
                    Vector2 boundaryPosition = GetBoundaryPosition(agent, viewRange);
                    Vector2 boundarySize = GetBoundarySize(viewRange);
                    Rect boundary = new Rect(boundaryPosition, boundarySize);
                    return boundary;
                }

                private void UpdateBoundaryPosition(){
                    boundary.position = GetBoundaryPosition(agent, viewRange);
                }

                public AgentBehaviour Agent{
                    get{
                        return agent;    
                    }    
                }

                public int AgentIndex{
                    get{
                        return  agentIndex;    
                    }    
                }

                public float ViewRange{
                    get{
                        return viewRange;    
                    }    
                }

                public List<AgentRecord> ActiveAgentRecords{
                    get{
                        return activeAgentRecords;    
                    }    
                }

                public VisionState VisionState{
                    get{
                        return visionState;    
                    }    
                }

                public Rect Boundary{
                    get{
                        return boundary;    
                    }    
                }

                public AgentRecord(AgentBehaviour agent, int agentIndex, float viewRange){
                    this.agent = agent;
                    this.agentIndex = agentIndex;
                    this.viewRange = viewRange;
                    activeAgentRecords = new List<AgentRecord>();
                    visionState = new VisionState(agent);
                    boundary = GetBoundary(agent, viewRange);
                }
                
                public void Update(){
                    UpdateBoundaryPosition();
                }
            }

            /* EdgeType */
            private enum EdgeType{
                MIN,
                MAX
            }

            /* Edge */
            private abstract class Edge : IComparable<Edge>{
                private AgentRecord agentRecord;
                private EdgeType edgeType;
                private float position;

                public AgentRecord AgentRecord{
                    get{
                        return agentRecord;
                    }    
                }
                
                public EdgeType EdgeType{
                    get{
                        return edgeType;    
                    }
                }

                public float Position{
                    get{
                        return position;    
                    }
                }

                public Edge(AgentRecord agentRecord, EdgeType edgeType, float position){
                    this.agentRecord = agentRecord;
                    this.edgeType = edgeType;
                    this.position = position;
                }

                public void Update(){
                    position = GetPosition();    
                }

                public int CompareTo(Edge edge){
                    return position.CompareTo(edge.position);
                }

                public static bool operator<(Edge edge0, Edge edge1){
                    return edge0.position < edge1.position;    
                }

                public static bool operator>(Edge edge0, Edge edge1){
                    return edge0.position > edge1.position;    
                }

                public abstract float GetPosition();
            }

            /* MinXEdge */
            private class MinXEdge : Edge{
                private static float GetPosition(AgentRecord agentRecord){
                    return agentRecord.Boundary.xMin;
                }

                private static EdgeType GetEdgeType(){
                    return EdgeType.MIN;
                }

                public MinXEdge(AgentRecord agentRecord) : base(agentRecord, GetEdgeType(), GetPosition(agentRecord)){
                }

                public override float GetPosition(){
                    return GetPosition(AgentRecord);
                }
            }

            /* MaxXEdge */
            private class MaxXEdge : Edge{
                private static float GetPosition(AgentRecord agentRecord){
                    return agentRecord.Boundary.xMax;
                }

                private static EdgeType GetEdgeType(){
                    return EdgeType.MAX;
                }

                public MaxXEdge(AgentRecord agentRecord) : base(agentRecord, GetEdgeType(), GetPosition(agentRecord)){
                }

                public override float GetPosition(){
                    return GetPosition(AgentRecord);
                }
            }

            /* MinYEdge */
            private class MinYEdge : Edge{
                private static float GetPosition(AgentRecord agentRecord){
                    return agentRecord.Boundary.yMin;
                }

                private static EdgeType GetEdgeType(){
                    return EdgeType.MIN;
                }

                public MinYEdge(AgentRecord agentRecord) : base(agentRecord, GetEdgeType(), GetPosition(agentRecord)){
                }

                public override float GetPosition(){
                    return GetPosition(AgentRecord);
                }
            }

            /* MaxYEdge */
            private class MaxYEdge : Edge{
                private static float GetPosition(AgentRecord agentRecord){
                    return agentRecord.Boundary.yMax;
                }

                private static EdgeType GetEdgeType(){
                    return EdgeType.MAX;
                }

                public MaxYEdge(AgentRecord agentRecord) : base(agentRecord, GetEdgeType(), GetPosition(agentRecord)){
                }

                public override float GetPosition(){
                    return GetPosition(AgentRecord);
                }
            }

            private float viewRange;
            private float viewAngle;
            private OverlapStateManager overlapStateManager;
            private List<AgentRecord> agentRecords;
            private List<VisionState> visionStates;
            private List<Edge> xEdges;
            private List<Edge> yEdges;

            public List<VisionState> VisionStates{
                get{
                    return visionStates;
                }    
            }

            private void ClearTargetAgents(){
                for(int i = 0; i < agentRecords.Count; i++){
                    agentRecords[i].VisionState.ClearTargetAgents();
                }
            }

            private bool WithinViewRange(AgentRecord agentRecord0, AgentRecord agentRecord1){
                return Vector2.Distance(agentRecord0.Agent.Position, agentRecord1.Agent.Position) < viewRange;
            }

            private bool WithinViewAngle(AgentRecord sourceAgentRecord, AgentRecord targetAgentRecord){
                Vector2 sourceForward = sourceAgentRecord.Agent.Forward;
                Vector2 sourceToTarget = targetAgentRecord.Agent.Position - sourceAgentRecord.Agent.Position;
                return Vector2.Angle(sourceForward, sourceToTarget) < 0.5f * viewAngle;
            }

            private void AddTargetAgents(){
                for(int i = 0; i < agentRecords.Count; i++){
                    AgentRecord agentRecord = agentRecords[i];
                    List<AgentRecord> activeAgentRecords = agentRecord.ActiveAgentRecords;
                    for(int j = 0; j < activeAgentRecords.Count; j++){
                        AgentRecord activeAgentRecord = activeAgentRecords[j];
                        if(agentRecord.AgentIndex < activeAgentRecord.AgentIndex && WithinViewRange(agentRecord, activeAgentRecord)){
                            if(WithinViewAngle(agentRecord, activeAgentRecord)){
                                agentRecord.VisionState.AddTargetAgent(activeAgentRecord.Agent);
                            }
                            if(WithinViewAngle(activeAgentRecord, agentRecord)){
                                activeAgentRecord.VisionState.AddTargetAgent(agentRecord.Agent);
                            }
                        }
                   }
                }
            }

            private void UpdateTargetAgents(){
                ClearTargetAgents();
                AddTargetAgents();    
            }

            private void UpdateAgentRecords(){
                for(int i = 0; i < agentRecords.Count; i++){
                    agentRecords[i].Update();
                }    
            }

            private static void Swap(List<Edge> edges, int i, int j){
                Edge temp = edges[i];
                edges[i] = edges[j];
                edges[j] = temp;
            }

            private void UpdateXEdges(){
                for(int i = 0; i < xEdges.Count; i++){
                    xEdges[i].Update();
                }    
            }

            private void SortAndUpdateX(){
                for(int i = 1; i < xEdges.Count; i++){
                    for(int j = i; 0 < j && xEdges[j] < xEdges[j - 1]; j--){
                        Swap(xEdges, j - 1, j);
                        Edge edge0 = xEdges[j - 1];
                        Edge edge1 = xEdges[j];
                        AgentRecord agentRecord0 = edge0.AgentRecord;
                        AgentRecord agentRecord1 = edge1.AgentRecord;
                        int agentIndex0 = agentRecord0.AgentIndex;
                        int agentIndex1 = agentRecord1.AgentIndex;
                        bool overlapBefore = overlapStateManager.GetOverlap(agentIndex0, agentIndex1);
                        if(edge0.EdgeType == EdgeType.MAX && edge1.EdgeType == EdgeType.MIN){                            
                            overlapStateManager.SetOverlapX(agentIndex0, agentIndex1, false);
                        }
                        else if(edge0.EdgeType == EdgeType.MIN && edge1.EdgeType == EdgeType.MAX){                            
                            overlapStateManager.SetOverlapX(agentIndex0, agentIndex1, true);
                        }
                        bool overlapAfter = overlapStateManager.GetOverlap(agentIndex0, agentIndex1);
                        if(overlapBefore && !overlapAfter){
                            agentRecord0.ActiveAgentRecords.Remove(agentRecord1);
                            agentRecord1.ActiveAgentRecords.Remove(agentRecord0);
                        }
                        if(!overlapBefore && overlapAfter){
                            agentRecord0.ActiveAgentRecords.Add(agentRecord1);
                            agentRecord1.ActiveAgentRecords.Add(agentRecord0);
                        }
                    }
                }
            }

            private void UpdateYEdges(){
                for(int i = 0; i < yEdges.Count; i++){
                    yEdges[i].Update();
                }    
            }

            private void SortAndUpdateY(){
                for(int i = 1; i < yEdges.Count; i++){
                    for(int j = i; 0 < j && yEdges[j] < yEdges[j - 1]; j--){
                        Swap(yEdges, j - 1, j);
                        Edge edge0 = yEdges[j - 1];
                        Edge edge1 = yEdges[j];
                        AgentRecord agentRecord0 = edge0.AgentRecord;
                        AgentRecord agentRecord1 = edge1.AgentRecord;
                        int agentIndex0 = agentRecord0.AgentIndex;
                        int agentIndex1 = agentRecord1.AgentIndex;
                        bool overlapBefore = overlapStateManager.GetOverlap(agentIndex0, agentIndex1);
                        if(edge0.EdgeType == EdgeType.MAX && edge1.EdgeType == EdgeType.MIN){                            
                            overlapStateManager.SetOverlapY(agentIndex0, agentIndex1, false);
                        }
                        else if(edge0.EdgeType == EdgeType.MIN && edge1.EdgeType == EdgeType.MAX){                            
                            overlapStateManager.SetOverlapY(agentIndex0, agentIndex1, true);
                        }
                        bool overlapAfter = overlapStateManager.GetOverlap(agentIndex0, agentIndex1);
                        if(overlapBefore && !overlapAfter){
                            agentRecord0.ActiveAgentRecords.Remove(agentRecord1);
                            agentRecord1.ActiveAgentRecords.Remove(agentRecord0);
                        }
                        if(!overlapBefore && overlapAfter){
                            agentRecord0.ActiveAgentRecords.Add(agentRecord1);
                            agentRecord1.ActiveAgentRecords.Add(agentRecord0);
                        }
                    }
                }
            }

            private void UpdateActiveAgentRecordsX(){
                UpdateXEdges();
                SortAndUpdateX();    
            }

            private void UpdateActiveAgentRecordsY(){
                UpdateYEdges();
                SortAndUpdateY();
            }

            private void UpdateActiveAgentRecords(){
                UpdateActiveAgentRecordsX();
                UpdateActiveAgentRecordsY();
            }

            public FixedVisionStateManager(List<AgentBehaviour> agents, float viewRange, float viewAngle){
                this.viewRange = viewRange;
                this.viewAngle = viewAngle;
                overlapStateManager = new OverlapStateManager(agents.Count);
                agentRecords = new List<AgentRecord>();
                visionStates = new List<VisionState>();
                xEdges = new List<Edge>();
                yEdges = new List<Edge>();
                for(int agentIndex = 0; agentIndex < agents.Count; agentIndex++){
                    AgentBehaviour agent = agents[agentIndex];
                    AgentRecord agentRecord = new AgentRecord(agent, agentIndex, viewRange);
                    agentRecords.Add(agentRecord);
                    visionStates.Add(agentRecord.VisionState);
                    xEdges.Add(new MinXEdge(agentRecord));
                    xEdges.Add(new MaxXEdge(agentRecord));
                    yEdges.Add(new MinYEdge(agentRecord));
                    yEdges.Add(new MaxYEdge(agentRecord));
                }
                xEdges.Sort();
                for(int i = 0; i < xEdges.Count - 1; i++){
                    if(xEdges[i].EdgeType == EdgeType.MIN){
                        AgentRecord agentRecordI = xEdges[i].AgentRecord;
                        for(int j = i + 1; j < xEdges.Count && xEdges[j].AgentRecord != agentRecordI; j++){
                            AgentRecord agentRecordJ = xEdges[j].AgentRecord;
                            overlapStateManager.SetOverlapX(agentRecordI.AgentIndex, agentRecordJ.AgentIndex, true);
                       }                                    
                    }
                }
                yEdges.Sort();
                for(int i = 0; i < yEdges.Count - 1; i++){
                    if(yEdges[i].EdgeType == EdgeType.MIN){
                        AgentRecord agentRecordI = yEdges[i].AgentRecord;
                        for(int j = i + 1; j < yEdges.Count && yEdges[j].AgentRecord != agentRecordI; j++){
                            AgentRecord agentRecordJ = yEdges[j].AgentRecord;
                            overlapStateManager.SetOverlapY(agentRecordI.AgentIndex, agentRecordJ.AgentIndex, true);
                            if(overlapStateManager.GetOverlap(agentRecordI.AgentIndex, agentRecordJ.AgentIndex)){
                                if(!agentRecordI.ActiveAgentRecords.Contains(agentRecordJ)){
                                    agentRecordI.ActiveAgentRecords.Add(agentRecordJ);
                                }
                                if(!agentRecordJ.ActiveAgentRecords.Contains(agentRecordI)){
                                    agentRecordJ.ActiveAgentRecords.Add(agentRecordI);
                                }
                            }
                       }                                    
                    }
                }
                UpdateTargetAgents();
            }
            
            public void Update(){
                UpdateAgentRecords();
                UpdateActiveAgentRecords();
                UpdateTargetAgents();
            }

            private void DrawBoundary(Rect boundary, Color color){
                Gizmos.color = color;
                Vector3 p0 = new Vector3(boundary.xMin, boundary.yMin, 0.0f);
                Vector3 p1 = new Vector3(boundary.xMin, boundary.yMax, 0.0f);
                Vector3 p2 = new Vector3(boundary.xMax, boundary.yMax, 0.0f);
                Vector3 p3 = new Vector3(boundary.xMax, boundary.yMin, 0.0f);
                Gizmos.DrawLine(p0, p1);
                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p0);
            }

            private void DrawGizmos(AgentRecord agentRecord){
                DrawBoundary(agentRecord.Boundary, Color.red);
                foreach(AgentRecord activeAgentRecord in agentRecord.ActiveAgentRecords){
                    DrawBoundary(activeAgentRecord.Boundary, Color.green);
                }
            }

            public void DrawGizmos(){
                DrawGizmos(agentRecords[0]);
                //Debug.Log("Antal: " + agentRecords[0].ActiveAgentRecords.Count);
            }
        }
        
        private List<AgentBehaviour> agents;
        private float viewRange;
        private float viewAngle;
        private FixedVisionStateManager fixedVisionStateManager;

        private void ValidateFixedVisionStateManager(){
            if(fixedVisionStateManager == null){
                fixedVisionStateManager = new FixedVisionStateManager(agents, viewRange, viewAngle);
            }
        }

        private void InvalidateFixedVisionStateManager(){
            fixedVisionStateManager = null;    
        }

        public float ViewRange{
            get{
                return viewRange;    
            }    
            set{
                if(viewRange != value){
                    viewRange = value;
                    InvalidateFixedVisionStateManager();
                }    
            }
        } 

        public float ViewAngle{
            get{
                return viewAngle;    
            }
            set{
                if(viewAngle != value){
                    viewAngle = value;
                    InvalidateFixedVisionStateManager();
                }    
            }
        }

        public List<VisionState> VisionStates{
            get{
                ValidateFixedVisionStateManager();
                return fixedVisionStateManager.VisionStates;
            }    
        }

        public VisionStateManager(List<AgentBehaviour> agents, float viewRange, float viewAngle){
            this.agents = new List<AgentBehaviour>(agents);
            this.viewRange = viewRange;
            this.viewAngle = viewAngle;
            fixedVisionStateManager = null;
        }        

        public void AddAgent(AgentBehaviour agent){
            if(!agents.Contains(agent)){
                agents.Add(agent);
                InvalidateFixedVisionStateManager();
            }
        }

        public void RemoveAgent(AgentBehaviour agent){
            if(agents.Contains(agent)){
                agents.Remove(agent);
                InvalidateFixedVisionStateManager();
            }
        }

        public void Update(){
            ValidateFixedVisionStateManager();
            fixedVisionStateManager.Update();
        }

        public void DrawGizmos(){
            ValidateFixedVisionStateManager();
            fixedVisionStateManager.DrawGizmos();
        }
    }


    [SerializeField] private float viewRange = 1.0f;
    [SerializeField] [Range(0.0f, 360.0f)] private float viewAngle = 90.0f; 
    [SerializeField] private AgentSetBehaviour agentSet = null;
    private VisionStateManager visionStateManager = null;
    private bool initialized = false;

    public List<VisionState> VisionStates{
        get{
            return visionStateManager.VisionStates;    
        }    
    }

    public float ViewRange{
        get{
            return viewRange;    
        }    
    }

    public float ViewAngle{
        get{
            return viewAngle;    
        }    
    }

    public List<VisionState> CreateVisionStates(){
        List<AgentBehaviour> agents = agentSet == null ? new List<AgentBehaviour>() : agentSet.Agents;
        VisionStateManager visionStateManager = new VisionStateManager(agents, viewRange, viewAngle);
        return visionStateManager.VisionStates;
    }

    public void OnValidate(){
        const float MIN_VIEW_RANGE = 0.001f;
        if(viewRange < MIN_VIEW_RANGE){
            Debug.LogWarning("View range must be at least " + MIN_VIEW_RANGE, this);
            viewRange = MIN_VIEW_RANGE;
        }
        if(agentSet == null){
            Debug.LogWarning("There is no agent set.");
        }
    }

    private Vector2 GetLocalViewPerimeterPosition(float angle){
        float angleInRadians = angle * Mathf.Deg2Rad;
        float localX = viewRange * Mathf.Cos(angleInRadians);
        float localY = viewRange * Mathf.Sin(angleInRadians);
        Vector2 localViewPerimeterPosition = new Vector2(localX, localY);
        return localViewPerimeterPosition;
    }

    private Vector2 GetViewPerimeterPosition(AgentBehaviour agent, float angle){
        return agent.transform.TransformPoint(GetLocalViewPerimeterPosition(angle));
    }

    private static float GetFraction(int part, int whole){
        return (float)part / whole;
    }

    private void DrawViewGizmos(AgentBehaviour agent){
        Gizmos.color = Color.grey;
        int segmentCount = 32;
        float minAngle = -0.5f * viewAngle;
        float maxAngle = 0.5f * viewAngle;
        Gizmos.DrawLine(agent.Position, GetViewPerimeterPosition(agent, minAngle));
        Gizmos.DrawLine(agent.Position, GetViewPerimeterPosition(agent, maxAngle));
        for(int segmentIndex = 0; segmentIndex < segmentCount; segmentIndex++){
            float angle0 = minAngle + GetFraction(segmentIndex, segmentCount) * viewAngle;
            float angle1 = minAngle + GetFraction(segmentIndex + 1, segmentCount) * viewAngle;
            Vector2 from = GetViewPerimeterPosition(agent, angle0);
            Vector2 to = GetViewPerimeterPosition(agent, angle1);
            Gizmos.DrawLine(from, to);
        }
    }

    private void DrawVisibilityGizmos(VisionState visionState){
        AgentBehaviour sourceAgent = visionState.SourceAgent;
        foreach(AgentBehaviour targetAgent in visionState.TargetAgents){
            Vector2 positionVector = targetAgent.Position - sourceAgent.Position;
            GizmosExtras.DrawPositionVector(sourceAgent.Position, positionVector);
        }
    }

    private void DrawVisionStateGizmos(VisionState visionState){
        DrawViewGizmos(visionState.SourceAgent);
        DrawVisibilityGizmos(visionState);
    }

    private void DrawVisionStateGizmos(List<VisionState> visionStates){
        foreach(VisionState visionState in visionStates){
            DrawVisionStateGizmos(visionState);       
        }
    }

    public void OnDrawGizmosSelected(){
        List<VisionState> visionStates = initialized ? visionStateManager.VisionStates : CreateVisionStates();
        DrawVisionStateGizmos(visionStates);
        /*
        if(visionStateManager != null){
            visionStateManager.DrawGizmos();
        }
        */
    }

    public void Awake(){
        visionStateManager = new VisionStateManager(agentSet.Agents, viewRange, viewAngle);
        initialized = true;
    }

    public void OnDestroy(){
        initialized = false;
        visionStateManager = null;
    }

    public void OnEnable(){
        agentSet.RegisterHandler(this);
    }

    public void OnDisable(){
        agentSet.UnregisterHandler(this);
    }

    public void FixedUpdate(){
        visionStateManager.ViewRange = viewRange;
        visionStateManager.ViewAngle = viewAngle;
        visionStateManager.Update();
    }

    public void OnAddAgent(AgentSetBehaviour agentSet, AgentBehaviour agent){
        visionStateManager.AddAgent(agent);
    }

    public void OnRemoveAgent(AgentSetBehaviour agentSet, AgentBehaviour agent){
        visionStateManager.RemoveAgent(agent);
    }
}
