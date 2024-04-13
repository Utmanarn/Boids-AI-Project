using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AgentGenerator */
public class AgentGeneratorBehaviour : MonoBehaviour {
    [SerializeField] private int agentCount = 0;
    [SerializeField] private AgentBehaviour agentPrefab = null;
    [SerializeField] private Transform agentParent = null;
    [SerializeField] private AgentSetBehaviour agentSet = null;
    [SerializeField] private Collider2D startRegion = null;
    private List<AgentBehaviour> agents = new List<AgentBehaviour>();

    public void OnValidate(){
        if(agentCount < 0){
            agentCount = 0;
            Debug.LogWarning("Agent count must be non negative.", this);
        }
        if(agentPrefab == null){
            Debug.LogWarning("There is no agent prefab.", this);
        }
        if(agentSet == null){
            Debug.LogWarning("There is no agent set.", this);
        }
        if(startRegion == null){
            Debug.LogWarning("There is no start region.", this);
        }        
    }

    private Vector2 GetRandomBoundsPosition(){
        float x = Random.Range(startRegion.bounds.min.x, startRegion.bounds.max.x);    
        float y = Random.Range(startRegion.bounds.min.y, startRegion.bounds.max.y);
        Vector2 position = new Vector2(x, y);
        return position;
    }

    private Vector2 GetRandomInstantiatePosition(){
        Vector2 spawnPosition = GetRandomBoundsPosition();
        while(!startRegion.OverlapPoint(spawnPosition)){
            spawnPosition = GetRandomBoundsPosition();
        }
        return spawnPosition;
    }

    private float GetRandomAngle(){
        return Random.Range(0.0f, 360.0f);    
    }

    private Quaternion GetRandomInstantiateRotation(){
        return Quaternion.Euler(0.0f, 0.0f, GetRandomAngle());
    }

    private AgentBehaviour InstantiateAgent(){
        Vector2 position = GetRandomInstantiatePosition();
        Quaternion rotation = GetRandomInstantiateRotation();
        AgentBehaviour agent = Instantiate(agentPrefab, position, rotation, agentParent);
        return agent;
    }

    private void InstantiateAgents(){
        while(agents.Count < agentCount){
            AgentBehaviour agent = InstantiateAgent();
            agents.Add(agent);
            agentSet.AddAgent(agent);
        }    
    }

    private AgentBehaviour GetRandomAgent(){
        return agents[Random.Range(0, agents.Count)];        
    }

    public void DestroyAgent(AgentBehaviour agent){
        Destroy(agent.gameObject);
    }

    private void DestroyAgents(){
        while(agentCount < agents.Count){
            AgentBehaviour agent = GetRandomAgent();
            agentSet.RemoveAgent(agent);
            agents.Remove(agent);
            DestroyAgent(agent);
        }    
    }

    public void Update(){
        agents.RemoveAll(agent => agent == null);
        if(agents.Count < agentCount){
            InstantiateAgents();
        }
        else if(agentCount < agents.Count){
            DestroyAgents();
        }
    }
}
