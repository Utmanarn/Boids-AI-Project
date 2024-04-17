using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float _visualRange;
    private List<Transform> _boidsInView;
    private Vector2 _movementDirection;
    private Vector2 _movementDirectionPotential;

    [SerializeField] private float cohesionStrength = 100f;
    [SerializeField] private float separationStrength = 100f;

    private void FixedUpdate()
    {
        BoidDetection();
        BoidUpdate();
    }

    private void BoidDetection()
    {
        
    }

    private void BoidUpdate()
    {
        _movementDirectionPotential = _rb.velocity + Cohesion() + Alignment() + Separation();
        _rb.AddForce(_movementDirectionPotential);
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

    private Vector2 Alignment()
    {
        Vector2 workVector = Vector2.zero;

        foreach (var boid in _boidsInView)
        {
            workVector += boid.GetComponent<SheepController>()._rb.velocity;
        }

        workVector = workVector / _boidsInView.Count;

        return (workVector - _rb.velocity) / 8;
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
}
