using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator anim = null;
    [SerializeField] private LayerMask moveLayerMask = 0;
    [SerializeField] private float speed = 1;
    [SerializeField] private LayerMask paintLayerMask = 0;
    private Level levelManager;
    private Vector2 mouseAnchor = Vector2.zero;
    private Vector2 dif = Vector3.zero;
    private bool isLocked = true;
    private bool isShot = false;

    private void Awake()
    {
        levelManager = (Level)LevelManager.Instance;
    }
    private void Update()
    {
        if (!isLocked && GameManager.Instance.State == GameManager.GameState.STARTED)
            CheckInput();
    }

    private float CalculatePathPaintPeriod(int count, int total, float time)
    {
        float a = Mathf.Pow(count / (float)total, 1 / 3f) * time;
        float b = Mathf.Pow((count + 1) / (float)total, 1f / 3f) * time;
        return b - a;
    }

    private IEnumerator PaintPath(Queue<LevelUnit> units, int count, int total, float time)
    {
        float period = CalculatePathPaintPeriod(count, total, time);
        yield return new WaitForSeconds(period);
        if (units.Count > 0)
            units.Dequeue().Paint();
        if (units.Count > 0 && !IsShot)
            StartCoroutine(PaintPath(units, count + 1, total, time));
    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseAnchor = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && mouseAnchor.magnitude != 0)
        {
            dif = (Vector2)Input.mousePosition - mouseAnchor;
            if (dif.magnitude > 0)
            {
                Vector3 direction = Vector3.zero;
                if (Mathf.Abs(dif.x) >= Mathf.Abs(dif.y))
                {
                    direction = dif.x >= 0 ? Vector3.right : Vector3.left;
                }
                else
                {
                    direction = dif.y >= 0 ? Vector3.forward : Vector3.back;
                }
                Move(direction);
            }
            mouseAnchor = Vector2.zero;
            dif = Vector2.zero;
        }
    }
    private void Move(Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, 20, moveLayerMask))
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy || hit.distance > 1)
            {
                Lock();
                float t = hit.distance / speed;
                anim.SetTrigger("Move");
                Vector3 targetPos = hit.point - dir / 2;
                Vector3 overlapBoxScale = Vector3.one * 0.45f;
                if (Mathf.Abs(dir.x) > 0)
                    overlapBoxScale.x = hit.distance / 2;
                else if (Mathf.Abs(dir.z) > 0)
                    overlapBoxScale.z = hit.distance / 2;
                Collider[] colliders = Physics.OverlapBox((transform.position + targetPos) / 2 - Vector3.up * 0.5f, overlapBoxScale);
                Queue<LevelUnit> units = new Queue<LevelUnit>();
                List<LevelUnit> unitList = new List<LevelUnit>();
                foreach (Collider collider in colliders)
                {
                    LevelUnit unit = collider.transform.GetComponent<LevelUnit>();
                    if (unit)
                        unitList.Add(unit);
                }
                unitList.Sort(delegate (LevelUnit a, LevelUnit b)
                {
                    return Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position));
                });
                foreach (LevelUnit unit in unitList)
                    units.Enqueue(unit);
                if (GameManager.Instance.PaintPath)
                    StartCoroutine(PaintPath(units, 0, units.Count, t));
                transform.LookAt(transform.position + dir);
                transform.LeanMove(targetPos, t).setEaseInCubic();

                LeanTween.delayedCall(t, () =>
                {
                    if (!isShot)
                    {
                        if (enemy)
                            HitEnemy(enemy, hit.point);
                        Unlock();
                        anim.SetTrigger("Idle");
                    }

                });
            }

        }
    }

    private void HitEnemy(Enemy enemy, Vector3 hitPoint)
    {
        enemy.Fall();
        levelManager.InstantiateWall(enemy.transform.position, enemy.transform.rotation, 1, 0);
        Instantiate(levelManager.HitEffectPrefab, hitPoint, Quaternion.LookRotation(enemy.transform.forward));
    }

    public void GetShot()
    {
        print("Die time: " + Time.time);
        Lock();
        levelManager.VanishAllBullets();
        isShot = true;
        //anim.SetTrigger("GetShot");
        LeanTween.cancel(gameObject);
        Destroy(GetComponent<Collider>());
        transform.LeanScale(Vector3.zero, 0.2f);
        LeanTween.delayedCall(0.21f, () =>
        {
            levelManager.FinishLevel(false);
        });

    }

    public void Lock()
    {
        isLocked = true;
    }
    public void Unlock()
    {
        isLocked = false;
    }

    public bool IsShot
    {
        get
        {
            return isShot;
        }
    }
}
