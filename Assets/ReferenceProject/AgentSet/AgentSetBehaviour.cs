using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* IAgentsHandler */
public interface IAgentSetHandler{
    void OnAddAgent(AgentSetBehaviour agentSet, AgentBehaviour agent);    
    void OnRemoveAgent(AgentSetBehaviour agentSet, AgentBehaviour agent);    
}

/* AgentSetBehaviour */
public class AgentSetBehaviour : MonoBehaviour {
    [SerializeField] private List<AgentBehaviour> agents = new List<AgentBehaviour>();
    private List<IAgentSetHandler> handlers = new List<IAgentSetHandler>();

    public List<AgentBehaviour> Agents{
        get{
            return agents;
        }
    }

    public void AddAgent(AgentBehaviour agent){
        if(!agents.Contains(agent)){
            agents.Add(agent);
            handlers.ForEach(handler => handler.OnAddAgent(this, agent));
        }
    }

    public void RemoveAgent(AgentBehaviour agent){
        if(agents.Contains(agent)){
            agents.Remove(agent);
            handlers.ForEach(handler => handler.OnRemoveAgent(this, agent));
        }
    }

    public bool ContainsAgent(AgentBehaviour agent){
        return agents.Contains(agent);
    }

    public List<IAgentSetHandler> Handlers{
        get{
            return handlers;    
        }    
    }

    public void RegisterHandler(IAgentSetHandler handler){
        if(!handlers.Contains(handler)){
            handlers.Add(handler);
        }
    }

    public void UnregisterHandler(IAgentSetHandler handler){
        if(handlers.Contains(handler)){
            handlers.Remove(handler);
        }
    }

    public bool HasHandler(IAgentSetHandler handler){
        return handlers.Contains(handler);
    }

    private void RemoveDestroyedAgents(){
        agents.RemoveAll(agent => agent == null);        
    }

    public void OnValidate(){
       RemoveDestroyedAgents();
    }

    public void OnDrawGizmosSelected(){
        RemoveDestroyedAgents();
        const int SEGMENT_COUNT = 32;
        const float RADIUS = 1.0f;
        Gizmos.color = Color.grey;
        foreach(AgentBehaviour agent in agents){
            GizmosExtras.DrawCircle(agent.Position, SEGMENT_COUNT, RADIUS);
        }
    }
}
