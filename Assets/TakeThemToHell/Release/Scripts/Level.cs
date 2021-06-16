using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Level : LevelManager
{
    [HideInInspector] public Player Player;
    [SerializeField] private Color[] fogColors;
    [SerializeField] private Color[] towerColors;
    [Header("Prefabs")]
    [SerializeField] private GameObject wallPrefab = null;
    [SerializeField] private GameObject hitEffectPrefab = null;
    [SerializeField] private GameObject decalEffectPrefab = null;
    private List<Enemy> enemies = null;
    private Fog fog = null;
    private Tower[] towers = null;

    private new void Awake()
    {
        base.Awake();
        type = LevelType.MAIN;
        Player = FindObjectOfType<Player>();
        enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        fog = FindObjectOfType<Fog>();
        towers = FindObjectsOfType<Tower>();
    }

    private void Start()
    {
        SetEnvironmentColors();
    }

    private void SetEnvironmentColors()
    {
        int index = GameManager.Instance.CurrentLevel % fogColors.Length;
        Color fogColor = fogColors[index];
        fog.Color = fogColor;
        Color towerColor = fogColors.Length == towerColors.Length ? towerColors[index] : fogColor;
        foreach (Tower tower in towers)
        {
            tower.Color = towerColor;
        }

    }
    public override void FinishLevel(bool success)
    {
        Player.Lock();
        GameManager.Instance.FinishLevel(success);
    }

    public override void StartLevel()
    {
        Player.Unlock();
    }

    public void InstantiateWall(Vector3 position, Quaternion rotation, float time, float delay)
    {
        position.y -= 1f;
        LeanTween.delayedCall(delay, () =>
        {
            LeanTween.moveLocalY(InstantiateWall(position, rotation), position.y + 1f, time);
        });
    }

    public GameObject InstantiateWall(Vector3 position, Quaternion rotation)
    {
        return Instantiate(wallPrefab, position, rotation);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        if (EnemyCount == 0)
            FinishLevel(true);
    }

    public int EnemyCount
    {
        get
        {
            return enemies.Count;
        }
    }

    public void VanishAllBullets()
    {
        Bullet[] bullets = FindObjectsOfType<Bullet>();
        foreach (Bullet bullet in bullets)
            bullet.Vanish();
    }

    public GameObject HitEffectPrefab
    {
        get
        {
            return hitEffectPrefab;
        }
    }

    public GameObject DecalEffectPrefab
    {
        get
        {
            return decalEffectPrefab;
        }
    }
}
