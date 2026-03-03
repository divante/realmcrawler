using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CharacterAnimationController : MonoBehaviour
{
  [SerializeField] private ICharacterPhysics physics;
  [SerializeField] private float turnThreshold = 90f; // degrees per second

  private Vector3 currentVelocity;
  private Vector3 movementDirection;
  private Animator animator;
  private Vector3 previousYRotation;

  private static readonly int animParamSpeedHash = Animator.StringToHash("Speed");
  private static readonly int animParamRightHash = Animator.StringToHash("MovementRight");
  private static readonly int animParamForwardHash = Animator.StringToHash("MovementForward");
  private static readonly int animParamTurningHash = Animator.StringToHash("TurningSpeed");

  public static readonly int AnimTriggerLightAttack = Animator.StringToHash("LightAttack");
  public static readonly int AnimTriggerHeavyAttack = Animator.StringToHash("HeavyAttack");
  public static readonly int AnimTriggerSpellCast   = Animator.StringToHash("SpellCast");
  public static readonly int AnimTriggerStagger      = Animator.StringToHash("Stagger");

  public void TriggerCombat(int triggerHash) => animator.SetTrigger(triggerHash);

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    physics = GetComponentInParent<ICharacterPhysics>();
    animator = GetComponent<Animator>();
    previousYRotation = GetForward();
  }

  // Update is called once per frame
  void Update()
  {
    currentVelocity = physics.GetVelocity();
    if (currentVelocity.sqrMagnitude < 0.001f)
    {
      animator.SetFloat(animParamSpeedHash, 0);
      animator.SetFloat(animParamRightHash, 0);
      animator.SetFloat(animParamForwardHash, 0);
      HandleTurn();
      return;
    }

    HandleMovement();
  }

  void HandleMovement()
  {

    float currentSpeed = physics.GetNormalizedVelocity();
    movementDirection = currentVelocity.normalized;

    Vector3 forward = GetForward();

    Vector3 right = transform.right;
    right.y = 0;
    right.Normalize();

    float relativeX = Vector3.Dot(movementDirection, right);
    float relativeY = Vector3.Dot(movementDirection, forward);

    animator.SetFloat(animParamSpeedHash, currentSpeed);
    animator.SetFloat(animParamRightHash, relativeX * currentSpeed);
    animator.SetFloat(animParamForwardHash, relativeY * currentSpeed);
  }

  void HandleTurn()
  {

    Vector3 currentYRotation = GetForward();
    float angleDelta = Vector3.SignedAngle(previousYRotation, currentYRotation, Vector3.up) / Time.deltaTime;

    if (Math.Abs(angleDelta) > turnThreshold)
    {
      animator.SetFloat(animParamTurningHash, angleDelta);
    }
    else
    {
      animator.SetFloat(animParamTurningHash, 0f);
    }

    previousYRotation = currentYRotation;


  }

  Vector3 GetForward()
  {
    Vector3 forward = transform.forward;
    forward.y = 0;
    forward.Normalize();
    return forward;
  }
}

