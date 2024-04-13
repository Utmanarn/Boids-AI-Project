using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* IObstacleSetHandler */
public interface IObstacleSetHandler{
    void OnAddObstacle(ObstacleSetBehaviour obstacleSet, ObstacleBehaviour obstacle);    
    void OnRemoveObstacle(ObstacleSetBehaviour obstacleSet, ObstacleBehaviour obstacle);    
}


/* ObstacleSetBehaviour */
public class ObstacleSetBehaviour : MonoBehaviour {
    [SerializeField] private List<ObstacleBehaviour> obstacles = new List<ObstacleBehaviour>();
    private List<IObstacleSetHandler> handlers = new List<IObstacleSetHandler>();

    public List<ObstacleBehaviour> Obstacles{
        get{
            return obstacles;
        }
    }

    public void AddObstacle(ObstacleBehaviour obstacle){
        if(!obstacles.Contains(obstacle)){
            obstacles.Add(obstacle);
            handlers.ForEach(handler => handler.OnAddObstacle(this, obstacle));
        }
    }

    public void RemoveObstacle(ObstacleBehaviour obstacle){
        if(obstacles.Contains(obstacle)){
            obstacles.Remove(obstacle);
            handlers.ForEach(handler => handler.OnRemoveObstacle(this, obstacle));
        }
    }

    public bool ContainsObstacle(ObstacleBehaviour obstacle){
        return obstacles.Contains(obstacle);
    }

    public List<IObstacleSetHandler> Handlers{
        get{
            return handlers;    
        }    
    }

    public void RegisterHandler(IObstacleSetHandler handler){
        if(!handlers.Contains(handler)){
            handlers.Add(handler);
        }
    }

    public void UnregisterHandler(IObstacleSetHandler handler){
        if(handlers.Contains(handler)){
            handlers.Remove(handler);
        }
    }

    public bool HasHandler(IObstacleSetHandler handler){
        return handlers.Contains(handler);
    }

    private void RemoveDestroyedAgents(){
        obstacles.RemoveAll(agent => agent == null);        
    }

    public void OnValidate(){
       RemoveDestroyedAgents();
    }

    public void OnDrawGizmosSelected(){
        RemoveDestroyedAgents();
        const int SEGMENT_COUNT = 32;
        const float RADIUS = 1.0f;
        Gizmos.color = Color.grey;
        foreach(ObstacleBehaviour obstacle in obstacles){
            GizmosExtras.DrawCircle(obstacle.Position, SEGMENT_COUNT, RADIUS);
        }
    }}
