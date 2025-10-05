using UnityEngine;

[RequireComponent(typeof(Hittable))]
public class Fly : MonoBehaviour
{
    public Player player;
    public float deadTime;

    public float walkTime = 1;
    public float walkDistance = 1;
    public AnimationCurve walkCurve;
    public float dashTime = 1;
    public float dashDistance = 1;
    public AnimationCurve dashCurve;
    public float waitTimeFrom = 1;
    public float waitTimeTo = 2;


    enum State { Walk, Wait, Dash, Dead };
    State state;
    float stateTimer = 0;
    Vector3 startPoint;
    Vector3 endPoint;

    void Start()
    {
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
    }

    void DeadUpdate()
    {
        if (stateTimer > walkTime)
            Destroy(this);
    }

    void Walk()
    {
        startPoint = transform.position;
        // TODO
        startPoint = transform.position + Vector3.right * walkDistance;
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

    void SetState(State newState)
    {
        // Debug.Log($"Fly state: {newState}");
        state = newState;
        stateTimer = 0;
    }
}
