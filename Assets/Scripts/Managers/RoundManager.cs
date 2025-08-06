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
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the gameplay round, including score calculation, difficulty scaling,
/// and special states such as Bullet Time and Game Over behavior.
/// </summary>
public class RoundManager : MonoBehaviour {

    #region Singleton
    public static RoundManager instance;
    #endregion

    #region Public Variables
    public ObstacleSpawner obstacleSpawner;
    public PowerUpSpawner powerUpSpawner;
    public float click = 3;
    public float currentVelocity = 10;
    public delegate void VelocityChangedHandler (float newVelocity);
    public event VelocityChangedHandler OnVelocityChanged;
    #endregion

    #region Serialized Fields
    [SerializeField] private float m_speedIncreaseRate = 200f;
    [SerializeField] private float m_gameOverSlowingVelocity = 0.35f;

    [Header("Bullet Time Settings")]
    [SerializeField] private float m_bulletTimeDuration = 2f;
    [SerializeField] private float m_bulletTimeScale = 0.3f;
    #endregion

    #region Private Variables
    private float m_score;
    private float m_timer = 0;
    private float m_bulletTimeTimer = 0f;
    private float m_originalTimeScale = 1f;
    private bool m_isBulletTimeActive = false;
    #endregion

    void Awake () {
        SingletonSetup();
    }

    void Update () {
        RoundManagement();
    }

    /// <summary>
    /// Ensures only one instance of this RoundManager exists (singleton pattern).
    /// </summary>
    private void SingletonSetup () {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    /// <summary>
    /// Main logic to manage gameplay round while in the Playing state.
    /// </summary>
    private void RoundManagement () {
        if (GameManager.instance.currentState == GameState.Playing) {
            BulletTimeLogic();
            Score();
            Difficulty();
        }
    }

    /// <summary>
    /// Gradually increases the game difficulty by updating the velocity at regular intervals.
    /// </summary>
    private void Difficulty () {
        m_timer += Time.deltaTime;
        if (m_timer >= click) {
            currentVelocity += m_speedIncreaseRate * Time.deltaTime;
            OnVelocityChanged?.Invoke(currentVelocity);
            m_timer = 0;
        }
    }

    /// <summary>
    /// Updates the score based on time and current velocity.
    /// </summary>
    private void Score () {
        m_score += Time.deltaTime * currentVelocity;
        GUIManager.instance.SetScore((int) m_score);
    }

    /// <summary>
    /// Handles logic for Bullet Time duration and deactivation.
    /// </summary>
    private void BulletTimeLogic () {
        if (m_isBulletTimeActive) {
            m_bulletTimeTimer -= Time.unscaledDeltaTime;
            if (m_bulletTimeTimer <= 0f) {
                DeactivateBulletTime();
            }
        }
    }

    /// <summary>
    /// Deactivates Bullet Time, restoring the original time scale and music speed.
    /// </summary>
    private void DeactivateBulletTime () {
        Time.timeScale = m_originalTimeScale;
        m_isBulletTimeActive = false;
        AudioManager.instance.ResetMusicSpeed();
    }

    /// <summary>
    /// Activates Bullet Time, slowing down time and music.
    /// Resets the Bullet Time timer if already active.
    /// </summary>
    public void ActivateBulletTime() {
        if (!m_isBulletTimeActive) {
            m_originalTimeScale = Time.timeScale;
            Time.timeScale = m_bulletTimeScale;
            m_isBulletTimeActive = true;
            m_bulletTimeTimer = m_bulletTimeDuration;
            AudioManager.instance.SlowMusic();
        } else {
            m_bulletTimeTimer = m_bulletTimeDuration;
        }
    }

    /// <summary>
    /// Checks whether Bullet Time is currently active.
    /// </summary>
    /// <returns>True if Bullet Time is active, false otherwise.</returns>
    public bool IsBulletTimeActive() {
        return m_isBulletTimeActive;
    }

    /// <summary>
    /// Adds extra points to the player's score.
    /// </summary>
    /// <param name="extraPoints">The number of points to add.</param>
    public void ExtraPoints (int extraPoints) {
        m_score += extraPoints;
    }

    /// <summary>
    /// Gets the current game velocity.
    /// </summary>
    /// <returns>The current velocity.</returns>
    public float GetCurrentVelocity () {
        return currentVelocity;
    }

    /// <summary>
    /// Initiates the Game Over sequence by slowing down time.
    /// </summary>
    public void GameOverBehaviour () {
        Time.timeScale = 0.5f; 
        StartCoroutine(SlowDownTimeCoroutine());
    }

    /// <summary>
    /// Coroutine that gradually slows down time until it reaches zero, then triggers Game Over.
    /// </summary>
    private IEnumerator SlowDownTimeCoroutine () {
        while (Time.timeScale > 0f) {
            Time.timeScale = Mathf.Max(0f, Time.timeScale - (m_gameOverSlowingVelocity * Time.unscaledDeltaTime));
            yield return null;
        }
        GameManager.instance.GameOver();
    }

    /// <summary>
    /// Gets the current integer score.
    /// </summary>
    /// <returns>The player's current score.</returns>
    public int GetScore () {
        return (int) m_score;
    }

    /// <summary>
    /// Gets the Bullet Time scale factor.
    /// </summary>
    /// <returns>The time scale used during Bullet Time.</returns>
    public float GetBulletTimeScale () {
        return m_bulletTimeScale;
    }
}
