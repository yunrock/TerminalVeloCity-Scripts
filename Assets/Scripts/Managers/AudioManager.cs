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
using UnityEngine.UI;

/// <summary>
/// Manages background music and sound effects across the game. Implements a singleton pattern.
/// </summary>
public class AudioManager : MonoBehaviour {
    /// <summary>
    /// Singleton instance of the AudioManager.
    /// </summary>
    public static AudioManager instance {
        get; private set;
    }

    #region Serialized Fields
    [Header("Audio Sources")]
    [SerializeField] private AudioSource m_musicSource;
    [SerializeField] private AudioSource m_sFXSource;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI m_musicToggleText;
    [SerializeField] private TextMeshProUGUI m_sFXToggleText;
    [SerializeField] private Slider m_musicVolumeSlider;
    [SerializeField] private Slider m_sFXVolumeSlider;

    [Header("Music")]
    [SerializeField] private AudioClip m_mainMenuMusic;
    [SerializeField] private AudioClip m_gameplayMusic;
    [SerializeField] private AudioClip m_gameOverMusic;
    [SerializeField] private AudioClip m_pauseMusic;
    [SerializeField] private AudioClip m_startGameMusic;

    [Header("SFX")]
    [SerializeField] private AudioClip m_buttonClickSound;
    [SerializeField] private AudioClip m_gameOverSoundFX;
    [SerializeField] private AudioClip m_pauseSoundFX;
    #endregion

    private bool isMusicOn = true;
    private bool isSfxOn = true;

    private void Awake () {
        SingletonSetup();
    }

    private void Start () {
        LoadPreferences();
        ApplyAudioSettings();
        UpdateUI();
    }

    /// <summary>
    /// Ensures only one instance of AudioManager exists.
    /// </summary>
    private void SingletonSetup () {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Toggles background music on or off.
    /// </summary>
    public void ToggleMusic () {
        isMusicOn = !isMusicOn;
        m_musicSource.mute = !isMusicOn;
        PlayerPrefs.SetInt("musicOn", isMusicOn ? 1 : 0);
        UpdateUI();
    }

    /// <summary>
    /// Toggles sound effects (SFX) on or off.
    /// </summary>
    public void ToggleSFX () {
        isSfxOn = !isSfxOn;
        m_sFXSource.mute = !isSfxOn;
        PlayerPrefs.SetInt("sfxOn", isSfxOn ? 1 : 0);
        UpdateUI();
    }

    /// <summary>
    /// Sets the volume of the music source.
    /// </summary>
    /// <param name="volume">The new volume (0 to 1).</param>
    public void SetMusicVolume (float volume) {
        m_musicSource.volume = volume;
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    /// <summary>
    /// Sets the volume of the SFX source.
    /// </summary>
    /// <param name="volume">The new volume (0 to 1).</param>
    public void SetSFXVolume (float volume) {
        m_sFXSource.volume = volume;
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    /// <summary>
    /// Plays a sound effect once through the SFX source.
    /// </summary>
    /// <param name="soundFX">The sound clip to play.</param>
    public void MakeSoundFX (AudioClip soundFX) {
        m_sFXSource.PlayOneShot(soundFX);
    }

    /// <summary>
    /// Slows down the music using the game's bullet time scale.
    /// </summary>
    public void SlowMusic () {
        m_musicSource.pitch = RoundManager.instance.GetBulletTimeScale() * 2;
    }

    /// <summary>
    /// Resets the pitch of the music to its default speed.
    /// </summary>
    public void ResetMusicSpeed () {
        m_musicSource.pitch = 1f;
    }

    /// <summary>
    /// Reset Music speed. Plays the main menu background music.
    /// Music loop is enabled.
    /// </summary>
    public void PlayMainMenuMusic () {
        if(m_musicSource.isPlaying && m_musicSource.clip == m_mainMenuMusic) {
            return;
        }
        ResetMusicSpeed();
        m_musicSource.loop = true;
        m_musicSource.clip = m_mainMenuMusic;
        m_musicSource.Play();
    }

    /// <summary>
    /// Reset music speed. Plays the background music for gameplay.
    /// Music loop is enabled.
    /// </summary>
    public void PlayGameplayMusic () {
        if(m_musicSource.isPlaying && m_musicSource.clip == m_gameplayMusic) {
            return;
        }
        ResetMusicSpeed();
        m_musicSource.loop = true;
        m_musicSource.clip = m_gameplayMusic;
        m_musicSource.Play();
    }

    /// <summary>
    /// Plays the game over sound effect.
    /// </summary>
    public void PlayPauseSoundFX () {
        if(m_sFXSource.isPlaying && m_sFXSource.clip == m_pauseSoundFX) {
            return;
        }
        m_sFXSource.PlayOneShot(m_pauseSoundFX);
    }

    /// <summary>
    /// Plays the game over sound effect.
    /// </summary>
    public void PlayGameOverSoundFX () {
        if(m_sFXSource.isPlaying && m_sFXSource.clip == m_gameOverSoundFX) {
            return;
        }
        m_sFXSource.PlayOneShot(m_gameOverSoundFX);
    }

    /// <summary>
    /// Reset music speed. Plays the music for the game over menu screen.
    /// Music loop is enabled.
    /// </summary>
    public void PlayGameOverMenuMusic () {
        if (m_musicSource.isPlaying && m_musicSource.clip == m_gameOverMusic) {
            return;
        }
        ResetMusicSpeed();
        m_musicSource.loop = true;
        m_musicSource.clip = m_gameOverMusic;
        m_musicSource.Play();
    }

    /// <summary>
    /// Reset music speed. Plays the pause menu background music.
    /// Music loop is enabled.
    /// </summary>
    public void PlayPauseMusic () {
        if(m_musicSource.isPlaying && m_musicSource.clip == m_pauseMusic) {
            return;
        }
        ResetMusicSpeed();
        m_musicSource.loop = true;
        m_musicSource.clip = m_pauseMusic;
        m_musicSource.Play();
    }

    /// <summary>
    /// Reset music speed. Plays the start game music.
    /// Music loop is disabled.
    /// </summary>
    public void PlayStartGameMusic () {
        if(m_musicSource.isPlaying && m_musicSource.clip == m_startGameMusic) {
            return;
        }
        ResetMusicSpeed();
        m_musicSource.loop = false;
        m_musicSource.clip = m_startGameMusic;
        m_musicSource.Play();
    }


    /// <summary>
    /// Plays the start game music and then automatically transitions to gameplay music.
    /// Reset music speed before playing.
    /// Music loop is disabled for start game music.
    /// </summary>
    public void PlayStartGameMusicAndThenGameplay () {
        if(m_musicSource.isPlaying && m_musicSource.clip == m_startGameMusic) {
            return;
        }
        ResetMusicSpeed();
        m_musicSource.loop = false;
        m_musicSource.clip = m_startGameMusic;
        m_musicSource.Play();
        StartCoroutine(PlayGameplayAfterStartGame());
    }

    /// <summary>
    /// Plays the button click sound effect if SFX is enabled.
    /// </summary>
    public void PlayButtonClickSound () {
        if (isSfxOn && m_buttonClickSound != null) {
            m_sFXSource.PlayOneShot(m_buttonClickSound);
        }
    }

    /// <summary>
    /// Coroutine that waits for the start music to finish, then plays gameplay music.
    /// </summary>
    private System.Collections.IEnumerator PlayGameplayAfterStartGame () {
        yield return new WaitForSeconds(m_startGameMusic.length);
        PlayGameplayMusic();
    }

    /// <summary>
    /// Loads saved user audio preferences from PlayerPrefs.
    /// </summary>
    private void LoadPreferences () {
        isMusicOn = PlayerPrefs.GetInt("musicOn", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("sfxOn", 1) == 1;
        m_musicSource.volume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        m_sFXSource.volume = PlayerPrefs.GetFloat("sfxVolume", 0.5f);
    }

    /// <summary>
    /// Applies mute settings based on loaded preferences.
    /// </summary>
    private void ApplyAudioSettings () {
        m_musicSource.mute = !isMusicOn;
        m_sFXSource.mute = !isSfxOn;
    }

    /// <summary>
    /// Updates Audio UI elements such as toggle text and volume sliders based on current settings.
    /// </summary>
    private void UpdateUI () {
        m_musicToggleText.text = isMusicOn ? "ON" : "OFF";
        m_sFXToggleText.text = isSfxOn ? "ON" : "OFF";
        m_musicVolumeSlider.value = m_musicSource.volume;
        m_sFXVolumeSlider.value = m_sFXSource.volume;
    }
}