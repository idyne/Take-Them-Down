using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator anim = null;
    [SerializeField] private Transform bulletSpawn = null;
    [SerializeField] private Transform muzzle = null;
    [SerializeField] private GameObject muzzleFlashPrefab = null;
    [SerializeField] private float shootPeriod = 1;
    [SerializeField] private GameObject bulletPrefab = null;
    [SerializeField] private GameObject canvas = null;
    [SerializeField] private GameObject timer = null;
    [SerializeField] private Image timerImage = null;
    private float shootDelay = 0;
    private float time = 0;
    private Rigidbody rb;
    private bool isAlive = true;

    private static Level levelManager = null;
    private int shootCount = 0;

    private void Awake()
    {
        if (!levelManager)
            levelManager = (Level)LevelManager.Instance;
        shootDelay = Random.Range(0, shootPeriod);
        time += shootDelay;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        canvas.transform.LookAt(Camera.main.transform);
        Vector3 canvasRot = canvas.transform.localRotation.eulerAngles;
        float angle = Vector3.Angle(transform.forward, Vector3.back);
        Vector3 cross = Vector3.Cross(transform.forward, Vector3.back);
        if (cross.y < 0) angle = -angle;
        canvasRot.y = angle;
        canvas.transform.localRotation = Quaternion.Euler(canvasRot);
        AdjustPeriod();
    }

    private void AdjustPeriod()
    {
        int wallLayerMask = 1 << 8;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, wallLayerMask))
        {
            float a = 1f;
            float b = 3f;
            shootPeriod = (hit.distance - 1.5f) * ((b - a) / 7) + a;
        }
    }

    private void Update()
    {

        time += Time.deltaTime;
        if (!levelManager.Player.IsShot && isAlive)
        {
            if ((int)((time + 0.4f) / shootPeriod) > shootCount)
            {
                shootCount = (int)((time + 0.4f) / shootPeriod);
                Shoot();
            }
        }
        else
        {
            time = 0;
        }
        SetPeriodTimer();
    }

    private void Shoot()
    {

        anim.SetTrigger("Shoot");
        LeanTween.delayedCall(0.4f, () =>
        {
            if (isAlive)
            {
                Destroy(Instantiate(muzzleFlashPrefab, muzzle.position, Quaternion.identity), 2);
                Bullet bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity).GetComponent<Bullet>();
                bullet.owner = this;
                bullet.transform.LookAt(bulletSpawn.position + transform.forward);
            }
        });
    }

    public void Fall()
    {
        print("Fall time: " + Time.time);
        Destroy(GetComponent<Collider>());
        timer.SetActive(false);
        isAlive = false;
        anim.SetTrigger("Fall");
        rb.isKinematic = false;
        rb.AddForce((-transform.forward + Vector3.up) * 5, ForceMode.Impulse);
        levelManager.RemoveEnemy(this);
        Destroy(gameObject, 4);
    }

    private void SetPeriodTimer()
    {
        timerImage.fillAmount = (time % shootPeriod) / shootPeriod;
    }

    public bool IsAlive
    {
        get
        {
            return isAlive;
        }
    }
}
