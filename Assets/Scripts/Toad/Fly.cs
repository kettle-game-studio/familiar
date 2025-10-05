using UnityEngine;

[RequireComponent(typeof(Hittable))]
public class Fly : MonoBehaviour
{
    // public Player player;
    public float deadTime = 1;

    public float walkTime = 1;
    public float walkDistance = 1;
    public AnimationCurve walkCurve;
    public float dashTime = 1;
    public float dashDistance = 1;
    public AnimationCurve dashCurve;
    public float waitTimeFrom = 1;
    public float waitTimeTo = 2;
    public Vector2 locationFrom;
    public Vector2 locationTo;
    public TriggerChecker viewTrigger;
    public TriggerChecker hitCollider;

    enum State { Walk, Wait, Dash, Dead };
    State state;
    float stateTimer = 0;
    Vector3 startPoint;
    Vector3 endPoint;

    void Start()
    {
        hitCollider.triggerEnter += TouchSomething;
        viewTrigger.triggerEnter += ViewSomething;
        GetComponent<Hittable>().callback += Hit;
        Wait();
    }

    void Update()
    {
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case State.Wait: WaitUpdate(); break;
            case State.Walk: WalkUpdate(); break;
            case State.Dash: DashUpdate(); break;
            case State.Dead: DeadUpdate(); break;
        }
    }

    void WaitUpdate()
    {
        if (stateTimer > waitTimeTo)
        {
            Walk();
            return;
        }
    }

    void WalkUpdate()
    {
        if (stateTimer > walkTime)
        {
            Wait();
            return;
        }
        float value = walkCurve.Evaluate(stateTimer / walkTime);
        transform.position = Vector3.Lerp(startPoint, endPoint, value);
    }

    void DashUpdate()
    {
        if (stateTimer > dashTime)
        {
            Wait();
            return;
        }
        float value = dashCurve.Evaluate(stateTimer / dashTime);
        transform.position = Vector3.Lerp(startPoint, endPoint, value);
    }

    void DeadUpdate()
    {
        if (stateTimer > deadTime)
            Destroy(gameObject);
    }

    void Walk()
    {
        Vector2 direction = Random.insideUnitCircle;
        startPoint = transform.position;
        Vector3 move = new Vector3(direction.x, direction.y, 0) * walkDistance;
        endPoint = transform.position + move;
        if (endPoint.x < locationFrom.x) move.x = +Mathf.Abs(move.x);
        if (endPoint.x > locationTo.x)   move.x = -Mathf.Abs(move.x);
        if (endPoint.y < locationFrom.y) move.y = +Mathf.Abs(move.y);
        if (endPoint.y > locationTo.y)   move.y = -Mathf.Abs(move.y);
        endPoint = transform.position + move;
        SetState(State.Walk);
    }

    void Wait()
    {
        SetState(State.Wait);
        stateTimer += Random.Range(0, waitTimeTo - waitTimeFrom);
    }

    void Hit(Transform from)
    {
        SetState(State.Dead);
    }

    void Dash(Vector3 target)
    {
        startPoint = transform.position;
        endPoint = (target - transform.position).normalized * dashDistance;
        SetState(State.Dash);
    }

    void ParryCallback(Transform from)
    {
        SetState(State.Dead);
    }

    void TouchSomething(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player == null)
            return;
        if (state == State.Dash)
            player.Parry(transform, ParryCallback);
    }

    void ViewSomething(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player == null)
            return;
        if (state == State.Wait)
            Dash(player.transform.position);
    }

    void SetState(State newState)
    {
        Debug.Log($"Fly state: {newState}");
        state = newState;
        stateTimer = 0;
    }
}
