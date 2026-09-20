using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Small demo-only input adapter. It keeps the upstream physics component
/// input-agnostic while making the community scene immediately testable.
/// </summary>
public sealed class DemoKeyboardController : MonoBehaviour
{
    [SerializeField] private CharacterPhysics physics;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float tangentDrag = 8f;
    [SerializeField] private float topSpeed = 8f;
    [SerializeField] private float maxSpeed = 8f;

    private Vector3 moveInput;

    public void SetPhysics(CharacterPhysics value)
    {
        physics = value;
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            moveInput = Vector3.zero;
            return;
        }

        var input = Vector2.zero;
        if (keyboard.aKey.isPressed) input.x -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;
        if (keyboard.sKey.isPressed) input.y -= 1f;
        if (keyboard.wKey.isPressed) input.y += 1f;

        input = Vector2.ClampMagnitude(input, 1f);
        moveInput = new Vector3(input.x, 0f, input.y);
    }

    private void FixedUpdate()
    {
        if (physics == null || !physics.EnableMovement) return;

        physics.HandleGroundControl(
            Time.fixedDeltaTime,
            moveInput,
            Speed: acceleration,
            tangDrag: tangentDrag,
            BreakMaxSpeed: false,
            TopSpeed: topSpeed,
            MaxSpeed: maxSpeed);
    }
}
