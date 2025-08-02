using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public delegate void Call();
    public delegate void CallVector2(Vector2 vector2);

    public static Call OnJumpEvent;
    void OnJump()
    {
        Debug.Log("Jump");
        OnJumpEvent?.Invoke();
    }

    public static Call OnMeleeLightEvent;
    void OnAttackPrimary()
    {
        Debug.Log("A1");
        OnMeleeLightEvent?.Invoke();
    }

    public static Call OnMeleeHeavyEvent;
    void OnAttackSecondary()
    {
        Debug.Log("A2");
        OnMeleeHeavyEvent?.Invoke();
    }

    void OnSecondary()
    {
        Debug.Log("A2");
        OnMeleeHeavyEvent?.Invoke();
    }

    public static Call OnDashEvent;
    void OnDash()
    {
        Debug.Log("Dash");
        OnDashEvent?.Invoke();
    }

    public static CallVector2 OnAimEvent;
    void OnLook(InputValue value)
    {
        OnAimEvent?.Invoke(value.Get<Vector2>());
    }

    public static CallVector2 OnMoveEvent;
    void OnMove(InputValue value)
    {
        OnMoveEvent?.Invoke(value.Get<Vector2>());
    }

    public static Call OnMenuEvent;
    void OnMenu()
    {
        Debug.Log("Menu");
        OnMenuEvent?.Invoke();
    }
}
