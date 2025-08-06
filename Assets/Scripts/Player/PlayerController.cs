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
using UnityEngine.InputSystem;

/// <summary>
/// Handles all player input and interactions within the game,
/// including movement between lanes, attacking, activating bullet time,
/// pausing the game, and handling collision-based power-up pickups.
/// </summary>
public class PlayerController : MonoBehaviour {
    #region [Serialized Fields]
    [Header("PowerUp Icons")]
    [SerializeField] private GameObject m_ShieldIcon;
    [SerializeField] private GameObject m_GunIcon;
    [SerializeField] private GameObject m_BulletTimeIcon;
    [SerializeField] private GameObject m_100PointsIcon;

    [Header("Prefabs")]
    [SerializeField] private GameObject m_BulletPrefab;

    [Header("Player Settings")]
    [SerializeField] private float m_maxScale = 2;
    [SerializeField] private float m_increaseScaleRatio = 0.04f;
    [SerializeField] private GameObject m_ParticlesPlayerDestroyed;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip m_playerDestroyedSound;
    [SerializeField] private AudioClip m_playerMoveSound;
    [SerializeField] private AudioClip m_powerUpSound;
    [SerializeField] private AudioClip m_extraPointsSound;
    #endregion

    #region Private Variables
    private Vector3[] m_lanePositions = new Vector3[3] {
        new Vector3(-488, -423f, -153f),
        new Vector3(-488, -403f, -103f),
        new Vector3(-488, -383f, -53f)
    };
    private int m_currentLaneIndex = 1; // Start in the middle lane
    private float m_100PointsActiveTime = 0;
    private PlayerInput m_playerInput;
    private InputAction m_moveAction;
    private InputAction m_attackAction;
    private InputAction m_bulletTimeAction;
    private InputAction m_pauseAction;
    private bool m_hasGun = false;
    private bool m_hasBulletTime = false;
    private bool[] m_unlockedSkins = new bool[3];
    #endregion

    void Awake () {
        m_playerInput = GetComponent<PlayerInput>();
    }

    /// <summary>
    /// Subscribes to input actions when the GameObject is enabled.
    /// Binds movement, attack, bullet time, and pause events to their respective handlers.
    /// </summary>
    void OnEnable () {
        m_moveAction = m_playerInput.actions["Move"];
        m_moveAction.performed += OnMovePerformed;
        m_attackAction = m_playerInput.actions["Attack"];
        m_attackAction.performed += OnAttackPerformed;
        m_bulletTimeAction = m_playerInput.actions["BulletTime"];
        m_bulletTimeAction.performed += OnBulletTimePerformed;
        m_pauseAction = m_playerInput.actions["Pause"];
        m_pauseAction.performed += OnPausePerformed;
    }

    /// <summary>
    /// Unsubscribes from input actions when the GameObject is disabled.
    /// Prevents memory leaks or dangling references.
    /// </summary>
    void OnDisable () {
        m_moveAction.performed -= OnMovePerformed;
        m_attackAction.performed -= OnAttackPerformed;
        m_bulletTimeAction.performed -= OnBulletTimePerformed;
        m_pauseAction.performed -= OnPausePerformed;
    }

    private void Start () {
        m_unlockedSkins[0] = true; // Default skin unlocked (forward feature)
        transform.localPosition = m_lanePositions[m_currentLaneIndex];
    }

    private void FixedUpdate () {
        ExtraPointsAnimation();
    }

    /// <summary>
    /// Displays the 100-points animation icon for a limited duration.
    /// Automatically hides the icon after the timer expires.
    /// </summary>
    private void ExtraPointsAnimation () {
        if (m_100PointsIcon.activeSelf && m_100PointsActiveTime > 0) {
            m_100PointsActiveTime -= Time.deltaTime;
            if (m_100PointsActiveTime <= 0) {
                m_100PointsIcon.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Handles vertical lane movement when Move input is received.
    /// Plays a movement sound and moves the player up or down one lane.
    /// </summary>
    /// <param name="context">The input callback context.</param>
    private void OnMovePerformed (InputAction.CallbackContext context) {
        //Movement(context);
        if (GameManager.instance.currentState != GameState.Playing) {
            return;
        }
        Vector2 input = context.ReadValue<Vector2>();
        if (input.y > 0.9f) {
            if (m_currentLaneIndex < m_lanePositions.Length - 1) {
                AudioManager.instance.MakeSoundFX(m_playerMoveSound);
                m_currentLaneIndex++;
                JumpToLane();
            }
        } else if (input.y < -0.9f) {
            if (m_currentLaneIndex > 0) {
                AudioManager.instance.MakeSoundFX(m_playerMoveSound);
                m_currentLaneIndex--;
                JumpToLane();
            }
        }
    }

    /*
    private void Movement (InputAction.CallbackContext context) {
        Vector2 input = context.ReadValue<Vector2>();
        if (input.y > 0.9f) {
            if (m_currentLaneIndex < m_lanePositions.Length - 1) {
                m_currentLaneIndex++;
                JumpToLane();
            }
        } else if (input.y < -0.9f) {
            if (m_currentLaneIndex > 0) {
                m_currentLaneIndex--;
                JumpToLane();
            }
        }
    }*/

    /// <summary>
    /// Handles attack input. Shoots a bullet if the player has a gun power-up.
    /// Disables the gun after use.
    /// </summary>
    /// <param name="context">The input callback context.</param>
    private void OnAttackPerformed (InputAction.CallbackContext context) {
        if (GameManager.instance.currentState != GameState.Playing) {
            return;
        }
        if (m_hasGun) {
            Instantiate(m_BulletPrefab, transform.position, Quaternion.identity, transform.parent);
            m_hasGun = false; // Reset gun after shooting
            m_GunIcon.SetActive(false);
            GUIManager.instance.HideGunText();
        }
    }

    /// <summary>
    /// Handles bullet time activation input. Activates bullet time if available.
    /// Hides the related GUI and disables the power-up.
    /// </summary>
    /// <param name="context">The input callback context.</param>
    private void OnBulletTimePerformed (InputAction.CallbackContext context) {
        if (GameManager.instance.currentState != GameState.Playing || !m_hasBulletTime) {
            return;
        }
        if (RoundManager.instance != null && m_hasBulletTime) {
            RoundManager.instance.ActivateBulletTime();
            GUIManager.instance.HideBulletTimeText();
            m_hasBulletTime = false;
            m_BulletTimeIcon.SetActive(false);
        }
    }

    /// <summary>
    /// Handles the pause input. Pauses the game unless bullet time is active.
    /// </summary>
    /// <param name="context">The input callback context.</param>
    private void OnPausePerformed (InputAction.CallbackContext context) {
        if (GameManager.instance.currentState != GameState.Playing) {
            return;
        }
        if (RoundManager.instance == null || !RoundManager.instance.IsBulletTimeActive()) {
            GameManager.instance.PauseGame();
        }
    }

    /*
    private void Shoot () {
        if (m_hasGun) {
            Instantiate(m_BulletPrefab, transform.position, Quaternion.identity, transform.parent);
            m_hasGun = false; // Reset gun after shooting
            m_GunIcon.SetActive(false);
            GUIManager.instance.HideGunText();
        }
    }*/

    /// <summary>
    /// Moves the player character to the selected lane index using predefined positions.
    /// </summary>
    private void JumpToLane () {
        transform.localPosition = m_lanePositions[m_currentLaneIndex];
    }

    /// <summary>
    /// Handles interactions with various game objects via trigger detection.
    /// Applies effects for obstacles, shields, guns, bullet time, and extra points.
    /// </summary>
    /// <param name="other">The collider the player has entered.</param>
    private void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Obstacle")) {
            m_100PointsIcon.SetActive(false);
            AudioManager.instance.SlowMusic();
            GameManager.instance.PreGameOver();
            StartCoroutine(ToDieCoroutine());
        } else if (other.CompareTag("Shield")) {
            GUIManager.instance.ShowShieldText();
            m_ShieldIcon.SetActive(true);
            AudioManager.instance.MakeSoundFX(m_powerUpSound);
        } else if (other.CompareTag("Gun")) {
            GUIManager.instance.ShowGunText();
            m_hasGun = true;
            m_GunIcon.SetActive(true);
            AudioManager.instance.MakeSoundFX(m_powerUpSound);
        } else if (other.CompareTag("BulletTime")) {
            GUIManager.instance.ShowBulletTimeText();
            m_BulletTimeIcon.SetActive(true);
            m_hasBulletTime = true;
            AudioManager.instance.MakeSoundFX(m_powerUpSound);
        } else if (other.CompareTag("ExtraPoints")) {
            RoundManager.instance.ExtraPoints(100);
            m_100PointsActiveTime = 0.5f;
            m_100PointsIcon.SetActive(true);
            AudioManager.instance.MakeSoundFX(m_extraPointsSound);
        }
    }

    /// <summary>
    /// Coroutine triggered when the player collides with an obstacle.
    /// Scales up the player visually, instantiates a particle effect, then deactivates the GameObject.
    /// </summary>
    private IEnumerator ToDieCoroutine () {
        while(transform.localScale.x < m_maxScale) {
            transform.localScale += new Vector3(m_increaseScaleRatio, m_increaseScaleRatio, m_increaseScaleRatio);
            yield return null;
        }
        // Instantiate particle effect on player destruction
        if (m_ParticlesPlayerDestroyed != null) {
            GameObject particles = Instantiate(m_ParticlesPlayerDestroyed, transform.position, Quaternion.identity, transform.parent);
            particles.transform.localScale = transform.localScale;
            AudioManager.instance.MakeSoundFX(m_playerDestroyedSound);
        }
        AudioManager.instance.ResetMusicSpeed();
        gameObject.SetActive(false);
    }
}