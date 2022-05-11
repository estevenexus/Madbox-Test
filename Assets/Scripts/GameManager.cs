using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public List<Enemy> Enemies;
    public Weapon[] Weapons;
    public Hero Hero;
    public Camera MainCamera;
    public Text ScoreText;
    public GameOverPanel GameOverPanel;

    public static GameManager Instance { get { return _instance; } }

    private int Score;
    private float Difficulty;
    private float EnemySpawnTime;
    private static GameManager _instance;

    private const int MAX_ALIVE_ENEMIES = 5;

    public Vector3 ClosestEnemyPosition(Vector3 playerPosition)
    {
        var closestEnemyPosition = Enemies[0].transform.position;
        var minDistance = Vector3.Distance(playerPosition, Enemies[0].transform.position);
        for (int i = 1; i < Enemies.Count; i++)
        {
            var currentDistance = Vector3.Distance(playerPosition, Enemies[i].transform.position);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closestEnemyPosition = Enemies[i].transform.position;
            }
        }

        return closestEnemyPosition;
    }

    public void GameOver()
    {
        if (PlayerPrefs.GetInt("HighScore", 0) < Score)
        {
            PlayerPrefs.SetInt("HighScore", Score);
        }

        Time.timeScale = 0;
        GameOverPanel.gameObject.SetActive(true);
        GameOverPanel.SetScoreText(Score);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RemoveEnemy(Func<int> instanceID)
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            if (Enemies[i].GetInstanceID == instanceID)
            {
                Enemies.RemoveAt(i);
                Difficulty *= 1.25f;
                Score += (int)Difficulty * 100;
                ScoreText.text = $"SCORE: {Score}";
            }
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        var weaponIndex = UnityEngine.Random.Range(0, Weapons.Length);
        var weapon = Instantiate(Weapons[weaponIndex].gameObject, Hero.WeaponParent).GetComponent<Weapon>();
        Hero.EquipWeapon(weapon);
        Difficulty = 1;
        Score = 0;
        ScoreText.text = "SCORE: 0";
    }

    private void Update()
    {
        if (EnemySpawnTime > 0)
        {
            EnemySpawnTime -= Time.deltaTime;
            return;
        }

        if (Enemies.Count >= MAX_ALIVE_ENEMIES)
        {
            EnemySpawnTime = UnityEngine.Random.Range(3, 10);
            return;
        }

        var enemy = Instantiate(EnemyPrefab, transform).GetComponentInChildren<Enemy>();
        enemy.transform.Translate(new Vector3(UnityEngine.Random.Range(-25, 25), 0, 0));
        enemy.transform.Translate(new Vector3(0, 0, UnityEngine.Random.Range(-25, 25)));
        enemy.Initinitialize(Hero, Difficulty);
        Enemies.Add(enemy);
        EnemySpawnTime = UnityEngine.Random.Range(3, 10);
    }
}
