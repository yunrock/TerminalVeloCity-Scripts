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
/// Controls the behavior of a bullet, including its movement and collision interactions.
/// </summary>
public class BulletBehaviour : MonoBehaviour {

    /// <summary>
    /// The sound effect to play when the bullet is fired.
    /// </summary>
    [SerializeField] private AudioClip m_bulletAudio;
    /// <summary>
    /// The velocity at which the bullet travels.
    /// </summary>
    [SerializeField] private float m_bulletVelocity = 400f;

    private Rigidbody m_rb;

    void Awake () {
        m_rb = GetComponent<Rigidbody>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start () {
        BulletSetup();
    }

    /// <summary>
    /// Called before the first frame update.
    /// Applies linear velocity to the bullet and plays the firing sound effect.
    /// </summary>
    private void BulletSetup () {
        m_rb.linearVelocity = Vector3.right * m_bulletVelocity;
        if (AudioManager.instance != null) {
            AudioManager.instance.MakeSoundFX(m_bulletAudio);
        }
    }

    /// <summary>
    /// Destroys the bullet if it collides with an object tagged "Obstacle".
    /// </summary>
    /// <param name="other">The collider the bullet entered.</param>
    private void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Obstacle")) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Continues to check for contact with obstacles and destroys the bullet if necessary.
    /// </summary>
    /// <param name="other">The collider the bullet is staying in contact with.</param>
    private void OnTriggerStay (Collider other) {
        if (other.CompareTag("Obstacle")) {
            Destroy(gameObject);
        }
    }
}
