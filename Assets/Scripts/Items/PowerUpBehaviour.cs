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
/// Controls the behavior of power-ups, including movement and interactions with the player or environment.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PowerUpBehaviour : MonoBehaviour {
    /// <summary>
    /// The amount by which the power-up moves to the left each step.
    /// </summary>
    [SerializeField] private float m_stepSize = 22f;
    private float currentTime = 0;

    void Start () {
        GetCollider();
    }

    /// <summary>
    /// Called when the script starts. Ensures the collider is set as a trigger.
    /// </summary>
    private void GetCollider () {
        Collider col = GetComponent<Collider>();
        if (col != null) {
            col.isTrigger = true;
        }
    }

    void FixedUpdate () {
        PowerUpMovementToLeft();
    }

    /// <summary>
    /// Moves the power-up to the left if the game is currently in the Playing state.
    /// </summary>
    private void PowerUpMovementToLeft () {
        if (GameManager.instance.currentState == GameState.Playing) {
            currentTime += Time.deltaTime * RoundManager.instance.currentVelocity;
            if (currentTime > RoundManager.instance.click) {
                transform.localPosition += Vector3.left * m_stepSize;
                currentTime = 0;
            }
        }
    }

    private void OnTriggerEnter (Collider other) {
        OnTriggerBehaviour(other);
    }

    private void OnTriggerStay (Collider other) {
        OnTriggerBehaviour(other);
    }

    /// <summary>
    /// Handles trigger behavior with different game elements such as the player, obstacles, and boundaries.
    /// </summary>
    /// <param name="other">The collider involved in the interaction.</param>
    private void OnTriggerBehaviour (Collider other) {
        if (other.CompareTag("ObstacleLimit")) {
            gameObject.SetActive(false);
        } else if (other.CompareTag("Player") || other.CompareTag("Obstacle")) {
            RoundManager.instance.powerUpSpawner.ReturnToPool(gameObject);
        }
    }
}
