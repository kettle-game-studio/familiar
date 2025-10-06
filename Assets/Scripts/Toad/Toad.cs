using UnityEngine;

public class Toad : MonoBehaviour
{
    public Player player;
    public CameraController cameraController;
    public Transform leftSide;
    public Transform rightSide;

    public Animator animator;
    public Transform toadBody;
    public CollisionChecker collisionChecker;
    public Hittable hittable;
    public TriggerChecker distanceTrigger;
    public SpriteRenderer shadow;
    public TriggerChecker tongue;

    public AudioSource audio;
    public AudioClip firstScream;
    public AudioClip[] screams;
    public float takeDamageTime = 0.1f;

    [HideInInspector]
    public Vector2 position;
    [HideInInspector]
    public float scaleDirection;
    [HideInInspector]
    public float shadowTransparency;

    int takenDamage = 0;

    void Start()
    {
        position.x = transform.position.x;
        position.y = toadBody.transform.localPosition.y;
        shadowTransparency = 0;
        scaleDirection = 1;

        hittable.callback += OnHit;
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

    void OnHit(Transform from)
    {
        if (takenDamage == 0)
        {
            audio.clip = firstScream;
            audio.Play();
        }
        else if (! (audio.isPlaying && audio.time < 0.5))
        {
            audio.clip = screams[Random.Range(0, screams.Length - 1)];
            audio.Play();
        }
        takenDamage++;
    }
}
