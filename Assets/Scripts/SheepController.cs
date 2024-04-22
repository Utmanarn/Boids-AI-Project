using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float visualRange = 4f;
    private List<Transform> _boidsInView;
    private Vector2 _movementDirectionPotential;

    //private BoidObjectManager _objectManager;

    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float cohesionStrength = 1f;
    [SerializeField] private float separationStrength = 1f;
    [SerializeField] private float velocityAlignmentStrength = 1f;

    [Header("Other Modifiers")] 
    [SerializeField] private float tendToPlaceInfluenceStrengthDivider = 1f;

    private Vector2 _debugSepartaion;
    private Vector2 _debugCohesion;
    private Vector2 _debugMatchAlignment;
    private Vector2 _tendToVector;

    private void Awake()
    {
        _boidsInView = new List<Transform>();
        _rb = GetComponent<Rigidbody2D>();
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

        Alignment();
        
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
            if (Vector2.Distance(boid.position, transform.position) < separationStrength)
            {
                workVector -= (Vector2) (boid.position - transform.position);
            }
        }

        return workVector;
    }

    private Vector2 MatchVelocity()
    {
        Vector2 workVector = Vector2.zero;

        foreach (var boid in _boidsInView)
        {
            workVector += boid.GetComponent<SheepController>()._rb.velocity; // Should not get component in fixedUpdate.
        }

        workVector = workVector / _boidsInView.Count;

        return (workVector - _rb.velocity) / velocityAlignmentStrength;
    }

    private Vector2 Cohesion()
    {
        Vector2 workVector = Vector2.zero;
        
        foreach (var boid in _boidsInView)
        {
            workVector += (Vector2) boid.position;
        }

        workVector = workVector / _boidsInView.Count;

        return (workVector - (Vector2)transform.position) / cohesionStrength;
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
