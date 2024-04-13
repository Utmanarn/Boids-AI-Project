using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* AlignmentSteeringBehaviour */
public class AlignmentSteeringBehaviour : SteeringBehaviour{
    [SerializeField] private VisionBehaviour vision = null;
    private bool initialized = false;

    public override void OnValidate(){
        base.OnValidate();
        if(vision == null){
            Debug.LogWarning("There is no vision.", this);            
        }
    }

    private Vector2 GetForce(VisionState visionState){
        int targetCount = visionState.TargetAgents.Count;
        if(0 < targetCount){
            Vector2 forwardSum = Vector2.zero;
            for(int i = 0; i < targetCount; i++){
                forwardSum += visionState.TargetAgents[i].Forward;
            }
            Vector2 forwardAverage = forwardSum / targetCount;
            Vector2 force = forwardAverage - visionState.SourceAgent.Forward;
            return force;
        }
        else{
            return Vector2.zero;
        }
    }

    private Vector2 GetWeightedForce(VisionState visionState){
        return weight * GetForce(visionState);
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
