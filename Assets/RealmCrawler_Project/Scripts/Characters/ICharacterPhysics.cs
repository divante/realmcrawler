using UnityEngine;

public interface ICharacterPhysics
{
  void TryMove(Vector2 direction);
  void StopMove();
  Vector3 GetVelocity();
  float GetNormalizedVelocity();
  void ApplyForce(Vector3 force);
  bool IsGrounded();
}