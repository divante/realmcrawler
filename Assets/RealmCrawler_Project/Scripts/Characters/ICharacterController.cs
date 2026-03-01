using System;
using UnityEngine;

public interface ICharacterController
{
  event Action<Vector2> MoveEvent;
  event Action<Quaternion> LookEvent;

  public Vector2 GetMovement();
  public Quaternion GetLookDirection();

  public void Disable();
  public void Enable();

}