using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car settings")]
    public float accelerationFactor = 30.0f;

	public float turnFactor = 3.5f;
    public float driftFactor = 0.9f;
    public float maxSpeed = 20;

    float accelerationInput = 0;
    float steeringInput = 0;

    float rotationAngel = 0;
    float velocityVsUp = 0;

    Rigidbody2D carRigidbody;

    void Awake()
    {
        carRigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        ApplyEngineForce();
        ApplyStreeding(); 
        KillOrthogonalVelocity();
    }

	void ApplyStreeding()
	{
		float minSpeedBeforeAllowTurningFactor = (carRigidbody.velocity.magnitude / 8);
		minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);
		rotationAngel -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

		carRigidbody.MoveRotation(rotationAngel);
	}

	void ApplyEngineForce()
	{
        velocityVsUp = Vector2.Dot(transform.up, carRigidbody.velocity);
        if (velocityVsUp > maxSpeed && accelerationInput > 0) 
            return;

        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0) 
            return;

        if (carRigidbody.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
            return;

        if (accelerationInput == 0)
        {
            carRigidbody.drag = Mathf.Lerp(carRigidbody.drag, 3.0f, Time.fixedDeltaTime * 3);
        }
        else
        {
            carRigidbody.drag = 0;
        }
        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

		carRigidbody.AddForce(engineForceVector, ForceMode2D.Force);


	}

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
		accelerationInput = inputVector.y;
    }

    void KillOrthogonalVelocity()
    {
        //Vector2 resistaceVelocity = transform.up * surfaceResistance;
        Vector2 forwardVector = transform.up * Vector2.Dot(carRigidbody.velocity, transform.up);
		Vector2 rightVector = transform.right * Vector2.Dot(carRigidbody.velocity, transform.right);

        carRigidbody.velocity = forwardVector + rightVector* driftFactor;
	}
}
