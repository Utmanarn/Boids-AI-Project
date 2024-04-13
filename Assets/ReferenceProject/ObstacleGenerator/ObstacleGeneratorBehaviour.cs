using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ObstacleGeneratorBehaviour */
public class ObstacleGeneratorBehaviour : MonoBehaviour {
    [SerializeField] private int obstacleCount = 0;
    [SerializeField] private float minScaling = 1.0f;
    [SerializeField] private float deltaScaling = 0.0f;
    [SerializeField] private Transform obstacleParent = null;
    [SerializeField] private ObstacleSetBehaviour obstacleSet = null;
    [SerializeField] private Collider2D startRegion = null;
    [SerializeField] private List<ObstacleBehaviour> obstaclePrefabs = new List<ObstacleBehaviour>();
    private List<ObstacleBehaviour> obstacles = new List<ObstacleBehaviour>();

    public void OnValidate(){
        if(obstacleCount < 0){
            obstacleCount = 0;
            Debug.LogWarning("Obstacle count must be non negative.", this);
        }
        if(minScaling < 0.0f){
            minScaling = 0.0f;
            Debug.LogWarning("Min scaling must be non negative.", this);
        }
        if(deltaScaling < 0.0f){
            deltaScaling = 0.0f;
            Debug.LogWarning("Delta scaling must be non negative.", this);
        }
        if(obstacleSet == null){
            Debug.LogWarning("There is no obstacle set.", this);
        }
        if(startRegion == null){
            Debug.LogWarning("There is no start region.", this);
        }
        obstaclePrefabs.RemoveAll(obstaclePrefab => obstaclePrefab == null);
        if(obstaclePrefabs.Count == 0){
            Debug.LogWarning("There are no obstacle prefabs.", this);
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

    private ObstacleBehaviour GetRandomObstaclePrefab(){
        int randomIndex = Random.Range(0, obstaclePrefabs.Count);
        ObstacleBehaviour randomObstaclePrefab = obstaclePrefabs[randomIndex];
        return randomObstaclePrefab;
    }

    private Vector3 GetRandomScaling(){
        float scale = Random.Range(minScaling, minScaling + deltaScaling);
        Vector3 scaling = new Vector3(scale, scale, scale);
        return scaling;
    }

    private ObstacleBehaviour InstantiateAgent(){
        ObstacleBehaviour obstaclePrefab = GetRandomObstaclePrefab();
        Vector2 position = GetRandomInstantiatePosition();
        Quaternion rotation = GetRandomInstantiateRotation();        
        ObstacleBehaviour obstacle = Instantiate(obstaclePrefab, position, rotation, obstacleParent);
        obstacle.transform.localScale = GetRandomScaling();
        return obstacle;
    }

    private void InstantiateAgents(){
        while(obstacles.Count < obstacleCount){
            ObstacleBehaviour obstacle = InstantiateAgent();
            obstacles.Add(obstacle);
            obstacleSet.AddObstacle(obstacle);
        }    
    }

    private ObstacleBehaviour GetRandomObstacle(){
        return obstacles[Random.Range(0, obstacles.Count)];        
    }

    public void DestroyObstacle(ObstacleBehaviour obstacle){
        Destroy(obstacle.gameObject);
    }

    private void DestroyObstacles(){
        while(obstacleCount < obstacles.Count){
            ObstacleBehaviour obstacle = GetRandomObstacle();
            obstacleSet.RemoveObstacle(obstacle);
            obstacles.Remove(obstacle);
            DestroyObstacle(obstacle);
        }    
    }

    public void Update(){
        obstacles.RemoveAll(agent => agent == null);
        if(obstacles.Count < obstacleCount){
            InstantiateAgents();
        }
        else if(obstacleCount < obstacles.Count){
            DestroyObstacles();
        }
    }
}

