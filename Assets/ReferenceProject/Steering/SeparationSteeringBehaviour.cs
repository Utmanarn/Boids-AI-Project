using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* SeparationSteeringBehaviour */
public class SeparationSteeringBehaviour : SteeringBehaviour{
    [SerializeField] private VisionBehaviour vision = null;
    private bool initialized = false;

    public override void OnValidate(){
        base.OnValidate();
        if(vision == null){
            Debug.Log("There is no vision.");
        }
    }

    private static bool TolerableDistance(Vector2 vector0, Vector2 vector1){
        const float TOLERANCE = 0.001f;
        return TOLERANCE < Vector2.Distance(vector0, vector1);
    }

    private float GetInverseDistanceFraction(float distance){
        if(distance <= 0.0f){
            return 1.0f;
        }
        else if(vision.ViewRange < distance){
            return 0.0f;
        }
        else{
            return (vision.ViewRange - distance) / vision.ViewRange;    
        }
    }

    private Vector2 GetForce(VisionState visionState){
        Vector2 force = Vector2.zero;
        Vector2 sourcePosition = visionState.SourceAgent.Position;
        for(int i = 0; i < visionState.TargetAgents.Count; i++){
            Vector2 targetPosition = visionState.TargetAgents[i].Position;
            if(TolerableDistance(sourcePosition, targetPosition)){
                Vector2 targetToSource = sourcePosition - targetPosition;
                float distance = targetToSource.magnitude;
                Vector2 direction = targetToSource / distance;
                force += GetInverseDistanceFraction(distance) * direction;
            }
        }
        return force;
    }

    private Vector2 GetWeightedForce(VisionState visionState){
        return Weight * GetForce(visionState);
    }

    private void DrawForceGizmos(List<VisionState> visionStates){
        foreach(VisionState visionState in visionStates){
            Vector2 forcePosition = visionState.SourceAgent.Position;
            Vector2 forceVector = GetForce(visionState);
            GizmosExtras.DrawForceVector(forcePosition, forceVector);
        }    
    }

    public void OnDrawGizmosSelected(){
        if(vision != null){
            List<VisionState> visionStates = initialized ? vision.VisionStates : vision.CreateVisionStates();
            DrawForceGizmos(visionStates);
        }    
    }

    public void Awake(){
        initialized = true;
    }

    public void OnDestroy(){
        initialized = false;
    }

    public override void FixedUpdate(){
        for(int i = 0; i < vision.VisionStates.Count; i++){
            VisionState visionState = vision.VisionStates[i];
            Vector2 weightedForce = GetWeightedForce(visionState);
            visionState.SourceAgent.AddForce(weightedForce);
        }
    }
}
