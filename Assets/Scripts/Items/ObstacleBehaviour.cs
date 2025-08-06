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
using UnityEngine;

/// <summary>
/// Handles the behavior of obstacles in the game, including movement, 
/// collision, destruction effects, and point rewards.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ObstacleBehaviour : MonoBehaviour {
    /// <summary>
    /// Particle effect to instantiate when the obstacle is destroyed.
    /// </summary>
    [SerializeField] private GameObject m_obstacleDestroyedParticleEffect;
    /// <summary>
    /// Sound effect to play when the obstacle is destroyed.
    /// </summary>
    [SerializeField] private AudioClip m_obstacleDestroyedSFX;
    /// <summary>
    /// Sound effect to play when the obstacle reaches the limit.
    /// </summary>
    [SerializeField] private AudioClip m_obstacleOnLimitSFX;
    /// <summary>
    /// The step size in units that the obstacle moves when updated.
    /// </summary>
    [SerializeField] private float m_stepSize = 22f;

    private float currentTime = 0;

    void Start () {
        GetCollider();
    }

    /// <summary>
    /// Gets the Collider component and sets it as a trigger.
    /// </summary>
    private void GetCollider () {
        Collider col = GetComponent<Collider>();
        if (col != null) {
            col.isTrigger = true;
        }
    }

    void FixedUpdate () {
        ObstacleMovementToLeft();
    }

    /// <summary>
    /// Moves the obstacle left based on the current game velocity and timing.
    /// </summary>
    private void ObstacleMovementToLeft () {
        if (GameManager.instance.currentState == GameState.Playing) {
            currentTime += Time.deltaTime * RoundManager.instance.currentVelocity;
            if (currentTime > RoundManager.instance.click) {
                transform.localPosition += Vector3.left * m_stepSize;
                currentTime = 0;
            }
        }
    }

    /// <summary>
    /// Triggered when another collider enters this obstacle's collider.
    /// Handles interactions with bullets, shields, and limit boundaries.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter (Collider other) {
        if (other.CompareTag("ObstacleLimit")) {
            AudioManager.instance.MakeSoundFX(m_obstacleOnLimitSFX);
            RoundManager.instance.ExtraPoints(10);
            gameObject.SetActive(false);
            return;
        }
        if (other.CompareTag("Bullet") || other.name.Equals("Shield and Icon")) {
            other.gameObject.SetActive(false);
            DestructionBehaviour();
            RoundManager.instance.obstacleSpawner.ReturnToPool(gameObject);
            RoundManager.instance.ExtraPoints(20);
            if (other.CompareTag("Shield")) {
                GUIManager.instance.HideShieldText();
            }
            return;
        }
    }

    /// <summary>
    /// Instantiates particle effects and plays sound effects when the obstacle is destroyed.
    /// </summary>
    private void DestructionBehaviour () {
        Instantiate(m_obstacleDestroyedParticleEffect, transform.position, Quaternion.identity, transform.parent);
        if (AudioManager.instance != null && m_obstacleDestroyedSFX != null) {
            AudioManager.instance.MakeSoundFX(m_obstacleDestroyedSFX);
        }
    }

}