using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static UnityAction<Vector2> onJoystick;
    public static UnityAction<GameObject> placedFromPlayer;
    public static UnityAction<bool> onAIMove;
}
