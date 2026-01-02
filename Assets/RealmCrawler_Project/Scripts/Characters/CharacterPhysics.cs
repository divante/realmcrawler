using UnityEngine;

[System.Serializable]
public class MovementSettings
{
  public float moveSpeed = 10f;
  public float acceleration = 15f;
  public float deceleration = 20f;
  public float mass = 1f;
  public float drag = 0.1f;
}

public class CharacterPhysics : MonoBehaviour, ICharacterPhysics
{
  [SerializeField] private MovementSettings movementSettings;

  Vector3 currentVelocity;
  Vector3 targetVelocity;

  Vector3 direction;

  private Rigidbody rigidbodyComponent;



  void Start()
  {
    rigidbodyComponent = GetComponent<Rigidbody>();
    rigidbodyComponent.freezeRotation = true;

  }

  public void TryMove(Vector2 desiredDirection)
  {
    if (desiredDirection.magnitude > 1f)
      desiredDirection.Normalize();

    direction = desiredDirection;
  }

  void Update()
  {
    // Calculate target velocity based on input
    Vector3 moveDirection = new Vector3(direction.x, 0f, direction.y);
    targetVelocity = moveDirection * movementSettings.moveSpeed;

    // Apply acceleration/deceleration
    if (direction.magnitude > 0.1f)
    {
      // Accelerate toward target velocity
      currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, movementSettings.acceleration * Time.deltaTime);
    }
    else if (currentVelocity.magnitude < 0.1f)
    {
      currentVelocity = Vector3.zero; // Stop completely if below a threshold
    }
    else
    {
      // Decelerate to stop
      currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, movementSettings.deceleration * Time.deltaTime);
      direction = Vector2.zero; // Reset direction when not moving
    }

    rigidbodyComponent.linearVelocity = currentVelocity;
  }

  public Vector3 GetVelocity()
  {
    return rigidbodyComponent.linearVelocity;
  }

  // return a float between 0 and 1 relative to the current velocity to the max velocity
  public float GetNormalizedVelocity()
  {
    return Mathf.Clamp01(currentVelocity.magnitude / movementSettings.moveSpeed);
  }

  public void ApplyForce(Vector3 force)
  {
    throw new System.NotImplementedException();
  }

  public bool IsGrounded()
  {
    throw new System.NotImplementedException();
  }

  public MovementSettings GetMovementSettings()
  {
    return movementSettings;
  }

  public void SetMoveSpeed(float speed)
  {
    movementSettings.moveSpeed = speed;
  }

}