using UnityEngine;

public interface ICharacterPhysics
{
  void TryMove(Vector2 direction);
  Vector3 GetVelocity();
  float GetNormalizedVelocity();
  void ApplyForce(Vector3 force);
  bool IsGrounded();
}