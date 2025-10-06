using UnityEngine;


[RequireComponent(typeof(Toad))]
public class ToadPhase1 : BattlePhase
{
    public float waitTime = 1;
    public float hp = 5;

    public float jumpTime = 1;
    public float jumpHigh = 1;
    public float fallTime = 1;
    public AnimationCurve jumpCurve;
    public AnimationCurve fallCurve;

    enum State { Wait, Jump, Fall };
    Toad toad;
    State state;
    float stateTimer;

    protected override void Start()
    {
        base.Start();
        toad = GetComponent<Toad>();
    }

    protected void Update()
    {
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case State.Wait: WaitUpdate(); break;
            case State.Jump: JumpUpdate(); break;
            case State.Fall: FallUpdate(); break;
        }
    }

    void WaitUpdate()
    {
        if (hp <= 0)
            phaseOver = true;
        else if (stateTimer > waitTime && toad.distanceTrigger.isTriggered())
            Jump();
    }

    void JumpUpdate()
    {
        if (stateTimer > jumpTime)
        {
            toad.position.x = Mathf.Clamp(toad.player.transform.position.x, toad.leftSide.position.x, toad.rightSide.position.x);
            SetState(State.Fall);
            return;
        }
        float curveValue = jumpCurve.Evaluate(stateTimer / jumpTime);
        toad.position.y = jumpHigh * curveValue;
        toad.shadowTransparency = curveValue;
    }

    void FallUpdate()
    {
        if (stateTimer > fallTime)
        {
            toad.cameraController.ShakeDown();
            SetState(State.Wait);
            return;
        }
        float curveValue = 1 - fallCurve.Evaluate(stateTimer / fallTime);
        toad.position.y = jumpHigh * curveValue;
        toad.shadowTransparency = curveValue;
        toad.scaleDirection = Mathf.Sign(toad.position.x - toad.player.transform.position.x);
    }

    void Jump()
    {
        toad.cameraController.ShakeUp();
        SetState(State.Jump);
    }

    void Hit(Transform from)
    {
        if (state != State.Wait) return;
        hp -= 1;
        Debug.Log($"Hit (phase 1)! phase hp: {hp}");
    }

    void TouchSomething(Collision2D collision)
    {
        Player player = collision.collider.GetComponent<Player>();
        if (player == null)
            return;
        if (state == State.Fall)
            player.TakeDamage(toad.toadBody);
    }

    void SetState(State newState)
    {
        Debug.Log($"Toad (phase 1) state: {newState}");
        state = newState;
        stateTimer = 0;
    }

    public override void PhaseEnter()
    {
        Jump();
        toad.hittable.callback += Hit;
        toad.collisionChecker.collisionEnter += TouchSomething;
    }

    public override void PhaseExit()
    {
        toad.hittable.callback -= Hit;
        toad.collisionChecker.collisionEnter -= TouchSomething;
    }
}
