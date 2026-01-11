using System;
using UnityEngine;

public interface ICharacterController
{
  event Action<Vector2> OnMoveEvent;
  event Action<Vector2> OnLookEvent;
}