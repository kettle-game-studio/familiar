using System.Net.Sockets;
using UnityEngine;


[RequireComponent(typeof(Toad))]
public class ToadPhase2 : BattlePhase
{
    public float hp = 5;
    public float waitBeforeFire = 1;
    public float stunTime = 1;

    public float jumpTime = 1;
    public float jumpHigh = 1;
    public AnimationCurve jumpCurve;

    public float fireTime = 1;
    public float fireDistance = 1;
    public AnimationCurve fireCurve;

    enum State { Wait, Jump, Fire, Stun };
    Toad toad;
    State state;
    float stateTimer;
    float tongueRollTimer;
    Vector2 jumpStart;
    Vector2 jumpEnd;

    protected override void Start()
    {
        base.Start();
        toad = GetComponent<Toad>();
        SetState(State.Jump);
    }

    protected void Update()
    {
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case State.Wait: WaitUpdate(); break;
            case State.Jump: JumpUpdate(); break;
            case State.Fire: FireUpdate(); break;
            case State.Stun: StunUpdate(); break;
        }
    }

    void WaitUpdate()
    {
        if (hp <= 0)
            phaseOver = true;
        else if (toad.distanceTrigger.isTriggered())
            Jump();
        else if (stateTimer > waitBeforeFire)
            SetState(State.Fire);
    }

    void JumpUpdate()
    {
        if (stateTimer > jumpTime)
        {
            toad.cameraController.ShakeDown();
            SetState(State.Wait);
            return;
        }
        float progress = stateTimer / jumpTime;
        float curveValue = jumpCurve.Evaluate(progress);
        toad.position.y = jumpHigh * curveValue;
        toad.position.x = Mathf.Lerp(jumpStart.x, jumpEnd.x, progress);
        toad.shadowTransparency = curveValue;
        toad.scaleDirection = Mathf.Sign(toad.position.x - toad.player.transform.position.x);
    }

    void FireUpdate()
    {
        if (stateTimer > fireTime)
        {
            SetState(State.Wait);
            return;
        }
        float value = fireCurve.Evaluate(stateTimer / fireTime);
        toad.tongue.transform.localScale = new Vector3(value * fireDistance, 1, 1);
    }

    void StunUpdate()
    {
        if (stateTimer > stunTime)
            SetState(State.Wait);
        if (tongueRollTimer <= fireTime)
        {
            tongueRollTimer += Time.deltaTime * 10;
            float value = fireCurve.Evaluate(tongueRollTimer / fireTime);
            toad.tongue.transform.localScale = new Vector3(value * fireDistance, 1, 1);
        }
    }

    void Hit(Transform from)
    {
        if (state != State.Stun) return;
        hp -= 1;
        Debug.Log($"Hit (phase 3)! phase hp: {hp}");
    }

    void ParryCallback(Transform from)
    {
        tongueRollTimer = stateTimer;
        SetState(State.Stun);
    }

    void TongueTrigger(Collider2D target)
    {
        Player player = target.GetComponent<Player>();
        if (player != null)
            player.Parry(toad.toadBody, ParryCallback);
    }

    void Jump()
    {
        float leftDistance = Mathf.Abs(toad.player.transform.position.x - toad.leftSide.position.x);
        float rightDistance = Mathf.Abs(toad.player.transform.position.x - toad.rightSide.position.x);
        float targetX = leftDistance > rightDistance ? toad.leftSide.position.x : toad.rightSide.position.x;

        jumpStart = toad.position;
        jumpEnd = new Vector2(targetX, 0);
        SetState(State.Jump);
        toad.cameraController.ShakeUp();
    }

    void SetState(State newState)
    {
        Debug.Log($"Toad (phase 3) state: {newState}");
        state = newState;
        stateTimer = 0;
    }

    public override void PhaseEnter()
    {
        Jump();
        toad.hittable.callback += Hit;
        toad.tongue.triggerEnter += TongueTrigger;
    }

    public override void PhaseExit()
    {
        toad.hittable.callback -= Hit;
        toad.tongue.triggerEnter -= TongueTrigger;
    }
}
