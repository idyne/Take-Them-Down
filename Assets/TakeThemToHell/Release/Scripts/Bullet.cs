using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    private Rigidbody rb = null;
    private static Level levelManager;
    private static bool shot = false;
    public Enemy owner;

    private void Awake()
    {
        shot = false;
        rb = GetComponent<Rigidbody>();
        if (!levelManager)
            levelManager = (Level)LevelManager.Instance;
    }

    private void Start()
    {
        //Destroy(gameObject, 5);

    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward * 10, Time.deltaTime * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Player player = collision.transform.GetComponent<Player>();
        if (player && !shot && owner.IsAlive)
        {
            shot = true;
            player.GetShot();
        }
        else if (!player)
        {
            Transform decay = Instantiate(levelManager.DecalEffectPrefab, collision.collider.ClosestPoint(transform.position), Quaternion.LookRotation(-transform.forward)).transform;
            Destroy(decay.gameObject, 5.1f);
        }
        Destroy(gameObject);
    }

    public void Vanish()
    {
        Destroy(GetComponent<Collider>());
        transform.LeanScale(Vector3.zero, 0.3f);
        Destroy(gameObject, 0.4f);
    }
}
