using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* WarpEffectorBehaviour */
public class WarpEffectorBehaviour : EffectorBehaviour {
    [SerializeField] private BoxCollider2D warpRegion = null;

    public override void OnValidate(){
        base.OnValidate();
        if(warpRegion == null){
            Debug.LogWarning("There is no warp region.", this);
        }
    }

    public override void Update(){
        float warpWidth = warpRegion.size.x;
        float warpHeight = warpRegion.size.y;
        float warpMinX = warpRegion.bounds.min.x;
        float warpMaxX = warpRegion.bounds.max.x;
        float warpMinY = warpRegion.bounds.min.y;
        float warpMaxY = warpRegion.bounds.max.y;
        for(int i = 0; i < obstacleSet.Obstacles.Count; i++){
            ObstacleBehaviour obstacle = obstacleSet.Obstacles[i];
            Vector2 position = obstacle.Position;
            if(position.x < warpMinX){
                position.x += warpWidth;
            }
            if(warpMaxX < position.x){
                position.x -= warpWidth;
            }
            if(position.y < warpMinY){
                position.y += warpHeight;
            }
            if(warpMaxY < position.y){
                position.y -= warpHeight;
            }
            obstacle.Position = position;
        }   
    }
}
