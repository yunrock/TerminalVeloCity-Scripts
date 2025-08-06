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
using TMPro;
using UnityEngine;

/// <summary>
/// Controls the player's character animation using a frame-based system with text representation.
/// The animation is updated based on the game's time and speed scaling provided by the RoundManager.
/// 
/// This class is intended to simulate a retro-style or minimalist animation using a sequence of characters
/// (or symbols) rendered via TextMeshProUGUI, changing frames at a rate influenced by the game's velocity.
/// </summary>
public class PlayerAnimator : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private TMP_Text m_playerText;
    [SerializeField] private float m_baseAnimationSpeed = 1f; 
    [SerializeField] private float m_animationSpeedMultiplier = 0.9f;
    #endregion 

    #region Private Variables
    private char[] m_animationFrames = { '|', '/', '—', '\\' }; 
    private int m_currentFrame = 0;
    private float m_timer;
    #endregion

    private void Start () {
        m_animationSpeedMultiplier = RoundManager.instance.GetCurrentVelocity();
    }

    private void Update () {
        PlayerAnimation();
    }

    /// <summary>
    /// Updates the player's animation frame over time while the game is in the Playing state.
    /// The frame advances based on a timer that factors in a base speed and a velocity multiplier.
    /// </summary>
    private void PlayerAnimation () {
        if (GameManager.instance.currentState == GameState.Playing) {
            float speedFactor = m_baseAnimationSpeed * m_animationSpeedMultiplier;
            m_timer += Time.deltaTime * speedFactor;

            if (m_timer >= 1f) {
                m_timer = 0f;
                m_currentFrame = (m_currentFrame + 1) % m_animationFrames.Length;
                m_playerText.text = m_animationFrames[m_currentFrame].ToString();
            }
        }
    }

    /// <summary>
    /// Sets the animation speed multiplier, usually in response to changes in game velocity.
    /// </summary>
    /// <param name="multiplier">The new animation speed multiplier value.</param>
    public void SetSpeedMultiplier (float multiplier) {
        m_animationSpeedMultiplier = multiplier;
    }

    /// <summary>
    /// Subscribes to the OnVelocityChanged event when the component is enabled,
    /// allowing the animation speed to respond dynamically to gameplay changes.
    /// </summary>
    void OnEnable () {
        RoundManager.instance.OnVelocityChanged += SetSpeedMultiplier;
    }

    /// <summary>
    /// Unsubscribes from the OnVelocityChanged event when the component is disabled
    /// to prevent memory leaks or null references.
    /// </summary>
    void OnDisable () {
        RoundManager.instance.OnVelocityChanged -= SetSpeedMultiplier;
    }
}