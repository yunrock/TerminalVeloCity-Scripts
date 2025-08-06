/*
 * Author: Yunuén Vladimir
 * Project: Terminal Velo.City
 * Website: https://yunuenvladimir.itch.io
 * License: GNU GPLv3
 * Tools: Unity 6, C#, PHP, SQL
 *
 * This script is part of the Terminal Velo.City project.
 * Redistribution and modification are allowed under the terms of the GNU General Public License v3.
 */
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the spawning and pooling of obstacle objects during gameplay.
/// </summary>
public class ObstacleSpawner : MonoBehaviour {

    [SerializeField] private GameObject[] m_obstaclePrefabs;
    [SerializeField] private Transform m_canvasObstacleParent;
    [SerializeField] private int m_maxObstaclesInPool = 20;
    [SerializeField] private float m_spawnCoolDown = 5;
    [SerializeField] private List<GameObject> m_activeObstacles;

    private float m_timeSinceLastSpawn = 10f;
    private int m_lastX = 0;
    private Vector3[] m_spawnPositions = new Vector3[] {
        new Vector3(484f, -423f, -153f),
        new Vector3(484f, -403f, -103f),
        new Vector3(484f, -383f, -53f)
    };

    private void Awake () {
        m_activeObstacles = new List<GameObject>();
    }

    private void Update () {
        Spawner();
    }

    /// <summary>
    /// Controls the timing and logic for spawning obstacles based on current game velocity and timing.
    /// </summary>
    private void Spawner () {
        m_timeSinceLastSpawn += (Time.deltaTime * RoundManager.instance.currentVelocity) / m_spawnCoolDown;
        if (m_timeSinceLastSpawn > RoundManager.instance.click) {
            SpawnObstacle();
            m_timeSinceLastSpawn = 0f;
            if (Random.value < 0.15f) {
                SpawnObstacle();
            }
        }
    }

    /// <summary>
    /// Spawns a new obstacle (only when needed) or reuses one from the pool if the maximum pool size has been reached.
    /// </summary>
    private void SpawnObstacle () {
        if (m_activeObstacles.Count < m_maxObstaclesInPool) {
            GameObject newObstacle = Instantiate(m_obstaclePrefabs[Random.Range(0, m_obstaclePrefabs.Length)], Vector3.zero, Quaternion.identity);
            newObstacle.transform.SetParent(m_canvasObstacleParent);
            PositioningObject(newObstacle);
            m_activeObstacles.Add(newObstacle);
        } else {
            GameObject poolObject = m_activeObstacles[0];
            m_activeObstacles.Remove(poolObject);
            PositioningObject(poolObject);
            m_activeObstacles.Add(poolObject);
            poolObject.SetActive(true);
        }
    }

    /// <summary>
    /// Sets the local position of a pooled obstacle to one of the predefined spawn positions.
    /// </summary>
    /// <param name="poolObject">The obstacle GameObject to be positioned.</param>
    private void PositioningObject (GameObject poolObject) {
        m_lastX = (m_lastX + Random.Range(1, 3)) % 3;
        poolObject.transform.localPosition = m_spawnPositions[m_lastX];
    }

    /// <summary>
    /// Deactivates the given obstacle and adds it back to the pool for future reuse.
    /// </summary>
    /// <param name="obstacle">The obstacle GameObject to return to the pool.</param>
    public void ReturnToPool (GameObject obstacle) {
        obstacle.SetActive(false);
        if (m_activeObstacles.Remove(obstacle)) {
            m_activeObstacles.Add(obstacle);
        }
    }
}