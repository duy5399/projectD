using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Background")]
    [SerializeField] private AudioClip[] homepageBGClip;
    [SerializeField] private AudioClip[] antCaveBGClip;
    [SerializeField] private AudioClip[] bugouCastleBGClip;
    [SerializeField] private AudioClip[] evilTribeeBGClip;

    [Header("Player")]
    [SerializeField] private AudioClip playerAttack1SFX;
    [SerializeField] private AudioClip playerAttack2SFX;
    [SerializeField] private AudioClip playerAttack3SFX;
    [SerializeField] private AudioClip playerDashSFX;
    [SerializeField] private AudioClip playerJumpSFX;
    [SerializeField] private AudioClip playerTakeDamageSFX;

    [Header("Character System")]
    [SerializeField] private AudioClip equipGearSFX;
    [SerializeField] private AudioClip unequipGearSFX;

    [Header("Upgrade System")]
    [SerializeField] private AudioClip upgradeSuccessSFX;
    [SerializeField] private AudioClip upgradeFailureSFX;

    [Header("Shop System")]
    [SerializeField] private AudioClip buyItemSFX;

    [Header("BOSS System")]
    [SerializeField] private AudioClip stopBattleSFX;

    [Header("UI")]
    [SerializeField] private AudioClip clickSFX;

    [Header("Mob")]
    [SerializeField] private AudioClip mobMove01SFX;
    [SerializeField] private AudioClip mobMove02SFX;
    [SerializeField] private AudioClip mobAttackSFX;
    [SerializeField] private AudioClip mobDieSFX;

    [Header("Pandora")]
    [SerializeField] private AudioClip pandoraBeatASFX;
    [SerializeField] private AudioClip pandoraBeatB_DSFX;
    [SerializeField] private AudioClip pandoraBeatCSFX;
    [SerializeField] private AudioClip pandoraBeatESFX;
    [SerializeField] private AudioClip pandoraDieSFX;

    [Header("Baroff")]
    [SerializeField] private AudioClip baroffBeatASFX;
    [SerializeField] private AudioClip baroffBeatBSFX;
    [SerializeField] private AudioClip baroffBeatCSFX;
    [SerializeField] private AudioClip baroffDieSFX;

    [Header("Midas")]
    [SerializeField] private AudioClip midasBeatASFX;
    [SerializeField] private AudioClip midasBeatBSFX;
    [SerializeField] private AudioClip midasBeatCSFX;
    [SerializeField] private AudioClip midasDieSFX;

    [Header("Cthulhu")]
    [SerializeField] private AudioClip cthulhuBeatA_BSFX;
    [SerializeField] private AudioClip cthulhuBeatCSFX;
    [SerializeField] private AudioClip cthulhuCallASFX;
    [SerializeField] private AudioClip cthulhuCallBSFX;
    [SerializeField] private AudioClip cthulhuDieSFX;

    [Header("Totem")]
    [SerializeField] private AudioClip totemBornSFX;
    [SerializeField] private AudioClip totemDieSFX;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void OnEnable()
    {
        Debug.Log("OnEnable called");
        //SceneManager.sceneLoaded += OnSceneLoaded;
        PlayBackgroundMusic(SceneManager.GetActiveScene());
    }

    // called when the game is terminated
    void OnDisable()
    {
        //Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        PlayBackgroundMusic(_scene);
    }

    public void PlayBackgroundMusic(Scene _scene)
    {
        switch (_scene.name)
        {
            case "Homepage":
                musicSource.clip = homepageBGClip[Random.Range(0, homepageBGClip.Length)];
                musicSource.Play();
                Debug.Log("PlayBackgroundMusic Homepage");
                break;
            case "AntCave":
                musicSource.clip = antCaveBGClip[Random.Range(0, antCaveBGClip.Length)];
                musicSource.Play();
                Debug.Log("PlayBackgroundMusic AntCave");
                break;
            case "BugouCastle":
                musicSource.clip = bugouCastleBGClip[Random.Range(0, bugouCastleBGClip.Length)];
                musicSource.Play();
                break;
            case "EvilTribe":
                musicSource.clip = evilTribeeBGClip[Random.Range(0, evilTribeeBGClip.Length)];
                musicSource.Play();
                break;
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    #region PlayerSFX
    public void PlayerAttackSFX(AudioSource _playerAS, int _attackCount)
    {
        switch (_attackCount)
        {
            case 0:
                _playerAS.clip = playerAttack1SFX;
                _playerAS.Play();
                break;
            case 1:
                _playerAS.clip = playerAttack2SFX;
                _playerAS.Play();
                break;
            case 2:
                _playerAS.clip = playerAttack3SFX;
                _playerAS.Play();
                break;
        }
    }

    public void PlayerDashSFX(AudioSource _playerAS)
    {
        _playerAS.PlayOneShot(playerDashSFX);
    }

    public void PlayerJumpSFX(AudioSource _playerAS)
    {
        _playerAS.PlayOneShot(playerJumpSFX);
    }

    public void PlayerTakeDamageSFX(AudioSource _playerAS)
    {
        _playerAS.clip = playerTakeDamageSFX;
        _playerAS.Play();
    }

    #endregion

    #region Character System
    public void EquipGearSFX()
    {
        sfxSource.clip = equipGearSFX;
        sfxSource.Play();
    }

    public void UnequipGearSFX()
    {
        sfxSource.clip = unequipGearSFX;
        sfxSource.Play();
    }
    #endregion

    #region Upgrade System
    public void UpgradeSuccessSFX()
    {
        sfxSource.clip = upgradeSuccessSFX;
        sfxSource.Play();
    }

    public void UpgradeFailureSFX()
    {
        sfxSource.clip = upgradeFailureSFX;
        sfxSource.Play();
    }

    #endregion

    #region Shop System
    public void BuyitemSuccessSFX()
    {
        sfxSource.clip = buyItemSFX;
        sfxSource.Play();
    }
    #endregion

    #region BOSS System
    public void StopBattleSuccessSFX()
    {
        sfxSource.clip = stopBattleSFX;
        sfxSource.Play();
    }
    #endregion

    #region UI
    public void ClickSuccessSFX()
    {
        sfxSource.clip = clickSFX;
        sfxSource.Play();
    }
    #endregion

    #region Mob
    public void MobMove01SFX(AudioSource _audioSource)
    {
        _audioSource.clip = mobMove01SFX;
        _audioSource.Play();
    }
    public void MobMove02SFX(AudioSource _audioSource)
    {
        _audioSource.clip = mobMove02SFX;
        _audioSource.Play();
    }
    public void MobAttackSFX(AudioSource _audioSource)
    {
        _audioSource.clip = mobAttackSFX;
        _audioSource.Play();
    }
    public void MobDieSFX(AudioSource _audioSource)
    {
        _audioSource.clip = mobDieSFX;
        _audioSource.Play();
    }
    #endregion

    #region Pandora
    public void PandoraBeatASFX(AudioSource _audioSource)
    {
        _audioSource.clip = pandoraBeatASFX;
        _audioSource.Play();
    }
    public void PandoraBeatB_DSFX(AudioSource _audioSource)
    {
        _audioSource.clip = pandoraBeatB_DSFX;
        _audioSource.Play();
    }
    public void PandoraBeatCSFX(AudioSource _audioSource)
    {
        _audioSource.clip = pandoraBeatCSFX;
        _audioSource.Play();
    }
    public void PandoraBeatESFX(AudioSource _audioSource)
    {
        _audioSource.clip = pandoraBeatESFX;
        _audioSource.Play();
    }
    public void PandoraDieSFX(AudioSource _audioSource)
    {
        _audioSource.clip = pandoraDieSFX;
        _audioSource.Play();
    }
    #endregion

    #region Baroff
    public void BaroffBeatASFX(AudioSource _audioSource)
    {
        _audioSource.clip = baroffBeatASFX;
        _audioSource.Play();
    }
    public void BaroffBeatBSFX(AudioSource _audioSource)
    {
        _audioSource.clip = baroffBeatBSFX;
        _audioSource.Play();
    }
    public void BaroffBeatCSFX(AudioSource _audioSource)
    {
        _audioSource.clip = baroffBeatCSFX;
        _audioSource.Play();
    }
    public void BaroffDieSFX(AudioSource _audioSource)
    {
        _audioSource.clip = baroffDieSFX;
        _audioSource.Play();
    }
    #endregion

    #region Midas
    public void MidasBeatASFX(AudioSource _audioSource)
    {
        _audioSource.clip = midasBeatASFX;
        _audioSource.Play();
    }
    public void MidasBeatBSFX(AudioSource _audioSource)
    {
        _audioSource.clip = midasBeatBSFX;
        _audioSource.Play();
    }
    public void MidasBeatCSFX(AudioSource _audioSource)
    {
        _audioSource.clip = midasBeatCSFX;
        _audioSource.Play();
    }
    public void MidasDieSFX(AudioSource _audioSource)
    {
        _audioSource.clip = midasDieSFX;
        _audioSource.Play();
    }
    #endregion

    #region Cthulhu
    public void CthulhuBeatA_BSFX(AudioSource _audioSource)
    {
        _audioSource.clip = cthulhuBeatA_BSFX;
        _audioSource.Play();
    }
    public void CthulhuBeatCSFX(AudioSource _audioSource)
    {
        _audioSource.clip = cthulhuBeatCSFX;
        _audioSource.Play();
    }
    public void CthulhuCallASFX(AudioSource _audioSource)
    {
        _audioSource.clip = cthulhuCallASFX;
        _audioSource.Play();
    }

    public void CthulhuCallBSFX(AudioSource _audioSource)
    {
        _audioSource.clip = cthulhuCallBSFX;
        _audioSource.Play();
    }

    public void CthulhuDieSFX(AudioSource _audioSource)
    {
        _audioSource.clip = cthulhuDieSFX;
        _audioSource.Play();
    }
    #endregion

    #region Totem
    public void TotemBornSFX(AudioSource _audioSource)
    {
        _audioSource.clip = totemBornSFX;
        _audioSource.Play();
    }
    public void TotemDieSFX(AudioSource _audioSource)
    {
        _audioSource.clip = totemDieSFX;
        _audioSource.Play();
    }
    #endregion

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
