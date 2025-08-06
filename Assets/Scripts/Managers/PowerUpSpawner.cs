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
/// Handles the spawning and pooling of Power-Up objects during gameplay.
/// </summary>
public class PowerUpSpawner : MonoBehaviour {
    [SerializeField] private GameObject[] m_powerupsPrefabs;
    [SerializeField] private Transform m_canvasPowerUpParent;
    [SerializeField] private int m_maxPowerUpsInPool;
    [SerializeField] private float m_spawnCoolDown;

    private float m_timeSinceLastSpawn = 10f;
    private int m_lastX = 0;
    private Vector3[] m_spawnPositions = new Vector3[] {
        new Vector3(484f, -423f, -153f),
        new Vector3(484f, -403f, -103f),
        new Vector3(484f, -383f, -53f)
    };

    public List<GameObject> activePowerUps;

    private void Awake () {
        activePowerUps = new List<GameObject>();
    }

    private void Update () {
        Spawner();
    }

    /// <summary>
    /// Controls the spawn rate of power-ups based on the current velocity and cooldown.
    /// Automatically triggers a power-up spawn when enough time has passed.
    /// </summary>
    private void Spawner () {
        m_timeSinceLastSpawn += (Time.deltaTime * RoundManager.instance.currentVelocity) / m_spawnCoolDown;
        if (m_timeSinceLastSpawn > RoundManager.instance.click) {
            SpawnPowerUp();
            m_timeSinceLastSpawn = 0f;
        }
    }

    /// <summary>
    /// Spawns a new power-up if the pool is not full.
    /// Reuses and repositions an existing inactive power-up from the pool if available.
    /// </summary>
    private void SpawnPowerUp () {
        if (activePowerUps.Count < m_maxPowerUpsInPool) {
            GameObject newPowerUp = Instantiate(m_powerupsPrefabs[Random.Range(0, m_powerupsPrefabs.Length)], Vector3.zero, Quaternion.identity);
            newPowerUp.transform.SetParent(m_canvasPowerUpParent);
            PositioningObject(newPowerUp);
            activePowerUps.Add(newPowerUp);
        } else if (!activePowerUps[0].activeInHierarchy){
            GameObject poolObject = activePowerUps[0];
            activePowerUps.Remove(poolObject);
            PositioningObject(poolObject);
            activePowerUps.Add(poolObject);
            poolObject.SetActive(true);
        }
    }

    /// <summary>
    /// Assigns a new position to the given object using a random spawn location.
    /// </summary>
    /// <param name="poolObject">The GameObject to be positioned.</param>
    private void PositioningObject (GameObject poolObject) {
        m_lastX = (m_lastX + Random.Range(1, 3)) % 3;
        poolObject.transform.localPosition = m_spawnPositions[m_lastX];
    }

    /// <summary>
    /// Returns the power-up object to the pool by deactivating and re-adding it to the list.
    /// </summary>
    /// <param name="powerUp">The GameObject to be returned to the pool.</param>
    public void ReturnToPool (GameObject powerUp) {
        powerUp.SetActive(false);
        if (activePowerUps.Remove(powerUp)) {
            activePowerUps.Add(powerUp);
        }
    }
}