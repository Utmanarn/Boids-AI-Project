using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float visualRange = 2f;
    [SerializeField] private float seekDistance = 7f;
    private List<Transform> _boidsInView;
    private Vector2 _movementDirectionPotential;

    private float angular = 0f;

    private Transform playerPosition;
    GameObject player;

    //private BoidObjectManager _objectManager;

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float cohesionStrength = 1f;
    [SerializeField] private float separationStrength = 1.5f;
    [SerializeField] private float separationDistance = 1.5f;
    [SerializeField] private float velocityAlignmentStrength = 1f;
    [SerializeField] private float seekStrength = 1f;

    [Header("Other Modifiers")] 
    [SerializeField] private float tendToPlaceInfluenceStrengthDivider = 1f;

    private Vector2 _debugSepartaion;
    private Vector2 _debugCohesion;
    private Vector2 _debugMatchAlignment;
    private Vector2 _tendToVector;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerMovement>().gameObject; // This is a dirty way to find the player.

        _boidsInView = new List<Transform>();
        _rb = GetComponent<Rigidbody2D>();
        playerPosition = player.transform;
    }

    private void Start()
    {
        BoidObjectManager.SheepList.Add(this);
    }

    private void FixedUpdate()
    {
        BoidDetection();
        BoidUpdateFlockBehaviour();
    }

    private void BoidDetection()
    {
        _boidsInView.Clear();

        foreach (var sheep in BoidObjectManager.SheepList)
        {
            if (Vector2.Distance(sheep.transform.position, transform.position) < visualRange)
            {
                _boidsInView.Add(sheep.transform);
            }
        }
    }

    private void BoidUpdateFlockBehaviour()
    {
        _debugSepartaion = Separation();
        _debugCohesion = Cohesion();
        _debugMatchAlignment = MatchVelocity();

        //Alignment();
        Seek();
        Rotate();
        
        _movementDirectionPotential = _rb.velocity + _debugCohesion + _debugMatchAlignment + _debugSepartaion;
        _movementDirectionPotential += -1 * _tendToVector;
        _tendToVector = Vector2.zero;
        _rb.AddForce(_movementDirectionPotential * movementSpeed);
    }
    
    private Vector2 Separation()
    {
        Vector2 workVector = Vector2.zero;

        foreach (var boid in _boidsInView)
        {
            if (Vector2.Distance(boid.position, transform.position) < separationDistance)
            {
                workVector -= (Vector2) (boid.position - transform.position);
            }
        }

        return workVector * separationStrength;
    }

    private Vector2 MatchVelocity()
    {
        Vector2 workVector = Vector2.zero;

        foreach (var boid in _boidsInView)
        {
            workVector += boid.GetComponent<SheepController>()._rb.velocity; // Should not get component in fixedUpdate.
        }

        workVector = workVector / _boidsInView.Count;

        return (workVector - _rb.velocity) * velocityAlignmentStrength;
    }

    private Vector2 Cohesion()
    {
        Vector2 workVector = Vector2.zero;
        
        foreach (var boid in _boidsInView)
        {
            workVector += (Vector2) boid.position;
        }

        workVector = workVector / _boidsInView.Count;

        return (workVector - (Vector2)transform.position) * cohesionStrength;
    }

    private void Alignment()
    {
        Vector2 alignmentValue = new Vector2();
        
        foreach (var boid in _boidsInView)
        {
            alignmentValue += (Vector2) boid.up;
        }

        transform.up += (Vector3) alignmentValue;
    }

    private void Seek()
    {
        bool seeking = true;

        Vector2 linear = Vector2.zero;

        Vector2 playerPos = playerPosition.position;
        Vector2 pos = transform.position;

        Vector2 direction = playerPos - pos;
        float distance = direction.magnitude;


        if (distance <= seekDistance)
        {
            linear = playerPos - pos;
            linear = linear.normalized * 5;
            seeking = true;

        }
        else
        {
            seeking = false;

        }

        Vector2 acceleration = linear * seekStrength;

        _rb.AddForce(acceleration);
    }

    private void Rotate()
    {
        float angle = Mathf.Atan2(_rb.velocity.x, -_rb.velocity.y) * Mathf.Rad2Deg;

        angular = Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, 3 * Time.deltaTime);

        float rotation = angular * 1;
        _rb.rotation = rotation;
    }

    public void TendToPlace(Vector2 place)
    {
        _tendToVector = ((place - (Vector2)transform.position) / tendToPlaceInfluenceStrengthDivider);
    }

    public void TendToPlaceScaled(Vector2 place)
    {
        _tendToVector += (place - (Vector2)transform.position) / (Vector2.Distance(transform.position, place) * 0.5f);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3) _debugCohesion);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3) _debugSepartaion);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3) _debugMatchAlignment);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, visualRange);
    }
    #endif
}
