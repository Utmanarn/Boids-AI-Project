using UnityEngine;

/* ForceEffectorBehaviour */
public class ForceEffectorBehaviour : EffectorBehaviour{
    [SerializeField] private float magnitude = 1.0f;
    [SerializeField] [Range(0.0f, 360.0f)] float angle = 0.0f;

    public override void OnValidate(){
        base.OnValidate();
        if(magnitude < 0.0f){
            magnitude = 0.0f;
            Debug.LogWarning("Magnitude must be non negative.", this);
        }
    }

    private Vector2 GetForce(){ 
        float angleInRadians = angle * Mathf.Deg2Rad;
        float forceX = magnitude * Mathf.Cos(angleInRadians);
        float forceY = magnitude * Mathf.Sin(angleInRadians);
        Vector2 force = new Vector2(forceX, forceY);
        return force;
    }

    private void DrawForceGizmos(){
        Vector2 force = GetForce();
        foreach(ObstacleBehaviour obstacle in obstacleSet.Obstacles){
            GizmosExtras.DrawForceVector(obstacle.Position, force);
        }        
    }

    public override void OnDrawGizmosSelected(){
        base.OnDrawGizmosSelected();
        if(obstacleSet != null){
            DrawForceGizmos();
        }
    }

    public override void FixedUpdate(){
        Vector2 force = GetForce();
        for(int i = 0 ; i < obstacleSet.Obstacles.Count; i++){
            obstacleSet.Obstacles[i].AddForce(force);
        }
    }
}
