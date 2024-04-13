using UnityEngine;

/* ObstacleBehaviour */
public class ObstacleBehaviour : MonoBehaviour {
    [SerializeField] private Rigidbody2D rigidBody2D = null;

    public Vector2 Position{
        get{
            return rigidBody2D.position;    
        }
        set{
            rigidBody2D.position = value;    
        }
    }

    public void AddForce(Vector2 force){
        rigidBody2D.AddForce(force);
    }

    public void OnValidate(){
        if(rigidBody2D == null){
            Debug.LogWarning("There is no rigid body 2D.", this);
        }
    }
}
