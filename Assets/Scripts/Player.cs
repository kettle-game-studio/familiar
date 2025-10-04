using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public float speed = 1;

    public float jumpTime = 1;
    public float jumpHigh = 1;
    public AnimationCurve jumpCurve;

    public float fallTime = 1;
    public float fallSpeed = 1;
    public AnimationCurve fallCurve;

    public float dashTime = 1;
    public float dashDistance = 1;
    public AnimationCurve dashCurve;

    public TriggerChecker jumpTrigger;

    enum State { Jump, Fall, Walk, Dash }

    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;
    InputAction attackAction;

    Rigidbody2D body;

    State state = State.Fall;
    float stateTimer = 0;
    bool canDash = true;
    float direction = 1;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");
        attackAction = InputSystem.actions.FindAction("Attack");
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        stateTimer += Time.deltaTime;
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector2 velocity = new Vector2(speed * moveValue.x, 0);

        if (state != State.Dash) {
            direction =
                moveValue.x > 0.5 ? 1 :
                moveValue.x < -0.5 ? -1 :
                direction;
            if (direction < 0) transform.localScale = new Vector3(-1, 1, 1);
            else transform.localScale = new Vector3(1, 1, 1);
        }

        switch (state)
        {
            case State.Walk: {
                if (!jumpTrigger.isTriggered()) {
                    SetState(State.Fall);
                    break;
                }
                if (jumpAction.IsPressed()) {
                    SetState(State.Jump);
                    break;
                }
                canDash = true;
                break;
            }
            case State.Fall: {
                if (jumpTrigger.isTriggered()) {
                    SetState(State.Walk);
                    break;
                }
                float speedK = fallTime <= 0 ? -CurveTimeDerivative(fallCurve, fallTime) : 1;
                velocity.y = -speedK * fallSpeed;
                break;
            }
            case State.Jump: {
                if (stateTimer > jumpTime) {
                    SetState(State.Fall);
                    break;
                }
                velocity.y = CurveTimeDerivative(jumpCurve, jumpTime) * jumpHigh;
                break;
            }
            case State.Dash: {
                if (stateTimer > dashTime) {
                    SetState(State.Fall);
                    break;
                }
                velocity.x = direction * CurveTimeDerivative(dashCurve, dashTime) * dashDistance;
                break;
            }
        }

        if (canDash && dashAction.IsPressed()) {
            SetState(State.Dash);
            canDash = false;
        }

        body.linearVelocity = velocity;
    }

    void SetState(State newState)
    {
        // Debug.Log($"State: {newState}");
        state = newState;
        stateTimer = 0;
    }

    float CurveTimeDerivative(AnimationCurve curve, float resolution)
    {
        float lastTime = stateTimer - Time.deltaTime;
        float h1 = curve.Evaluate(lastTime / resolution);
        float h2 = curve.Evaluate(stateTimer / resolution);
        return (h2 - h1) / Time.deltaTime;
    }
}
