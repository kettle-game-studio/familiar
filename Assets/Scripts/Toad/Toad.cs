using UnityEngine;

public class Toad : MonoBehaviour
{
    public Player player;
    public Transform leftSide;
    public Transform rightSide;

    public Transform toadBody;
    public CollisionChecker collisionChecker;
    public Hittable hittable;
    public TriggerChecker distanceTrigger;
    public SpriteRenderer shadow;
    public TriggerChecker tongue;

    [HideInInspector]
    public Vector2 position;
    [HideInInspector]
    public float scaleDirection;
    [HideInInspector]
    public float shadowTransparency;

    void Start()
    {
        position.x = transform.position.x;
        position.y = toadBody.transform.localPosition.y;
        shadowTransparency = 0;
        scaleDirection = 1;
    }

    void Update()
    {
        transform.position = new Vector3(position.x, transform.position.y, transform.position.z);
        toadBody.transform.localPosition = new Vector3(
            toadBody.transform.localPosition.x,
            position.y,
            toadBody.transform.localPosition.z
        );
        float powTransparency = shadowTransparency * shadowTransparency;
        // powTransparency = powTransparency * powTransparency;
        // powTransparency = powTransparency * powTransparency;
        // powTransparency = powTransparency * powTransparency;
        shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b, 1 - powTransparency);
        toadBody.transform.localScale = new Vector3(scaleDirection, 1, 1);
    }
}
