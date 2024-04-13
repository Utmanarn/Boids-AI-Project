using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AgentBehaviour */
public class AgentBehaviour : MonoBehaviour{
    [SerializeField] private Rigidbody2D agentRigidbody = null;
    [SerializeField] private Collider2D agentCollider = null;
    private Vector2 forward = Vector2.zero;
    private bool initialized = false;

    private void SynchRotation(){
        float rotation = Vector2.Angle(Vector2.right, agentRigidbody.velocity);
        if(agentRigidbody.velocity.y < 0.0f){
            rotation = 360.0f - rotation;
        }
        agentRigidbody.rotation = rotation;        
    }

    private static Vector2 CalcDirection(float rotation){
        float angleInRadians = Mathf.Deg2Rad * rotation;
        float forwardX = Mathf.Cos(angleInRadians);
        float forwardY = Mathf.Sin(angleInRadians);
        Vector2 forward = new Vector2(forwardX, forwardY);
        return forward;
    }

    private static Vector2 CalcForward(float rotation){
        return CalcDirection(rotation);
    }

    private static Vector2 CalcRight(float rotation){
        return CalcDirection(rotation - 90.0f);
    }

    private void SynchForward(){
        forward = CalcForward(agentRigidbody.rotation);
    }

    public Rigidbody2D AgentRigidbody{
        get{
            return agentRigidbody;    
        }    
    }

    public Collider2D AgentCollider{
        get{
            return agentCollider;    
        }    
    }

    public Vector2 Position{
        get{
            return agentRigidbody.position;
        }    
    }

    public float Rotation{
        get{
            return agentRigidbody.rotation;    
        }    
    }

    public Vector2 Velocity{
        get{
            return agentRigidbody.velocity;
        }    
    }

    public Vector2 Forward{
        get{
            return initialized ? forward : CalcForward(agentRigidbody.rotation);    
        }    
    }

    public void OnValidate(){
        if(agentRigidbody == null){
            Debug.LogWarning("There is no agent rigidbody.", this);
        }
        if(agentCollider == null){
            Debug.LogWarning("There is no agent collider.", this);
        }
    }

    public void Awake(){
        SynchForward();
        initialized = true;
    }

    public void OnDestroy(){
        initialized = false;
    }

    private void DrawVelocityGizmo(){
        GizmosExtras.DrawVelocityVector(transform.position, agentRigidbody.velocity);
    }

    private void DrawDirectionGizmos(){
        GizmosExtras.DrawPositionVector(transform.position, CalcForward(agentRigidbody.rotation));    
        GizmosExtras.DrawPositionVector(transform.position, CalcRight(agentRigidbody.rotation));    
    }

    public void OnDrawGizmosSelected(){
        if(agentRigidbody != null){
            DrawVelocityGizmo();
            //DrawDirectionGizmos();
        }
    }

    public void AddForce(Vector2 force){
        agentRigidbody.AddForce(force);
    }

    private bool HasTolerableVelocity(){
        return 0.001f < agentRigidbody.velocity.magnitude;
    }

    public void FixedUpdate(){
        if(HasTolerableVelocity()){
            SynchRotation();
            SynchForward();
        }
    }
}
