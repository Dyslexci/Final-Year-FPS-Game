using CharacterScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class CharacterCombatController : MonoBehaviour
{
    public static CharacterCombatController instance;
    public int currentHealth;
    public bool dashInvulnerable;

    public AudioSource damageAudioSource;
    public AudioSource deathTeleportAudio;
    public CanvasGroup damageImage;

    float invulnPeriod = .1f;
    float invulnTime = 0.0f;
    VolumeProfile postProc;
    ColorAdjustments colours;


    private void Awake()
    {
        instance = this;
        damageImage.gameObject.SetActive(true);
        damageImage.alpha = 0;
        postProc = GameObject.Find("Post Processing Volume").GetComponent<Volume>().profile;
        postProc.TryGet(out colours);
    }

    private void Start()
    {
        //currentHealth = CharacterStatisticsController.maxhealth;
        UIController.instance.healthBar.maxValue = CharacterStatisticsController.maxhealth;
        UIController.instance.healthBar.currentPercent = CharacterStatisticsController.currentHealth;
    }

    /// <summary>
    /// Removed the paramter value in health from the player, then determines whether extra actions need to be taken in the form of
    /// death, respawn, etc.
    /// </summary>
    /// <param name="damage"></param>
    public void DamageHealth(int damage)
    {
        if (Time.time < invulnTime || dashInvulnerable) return;
        invulnTime = Time.time + invulnPeriod;
        damageImage.alpha = 1;
        LeanTween.alphaCanvas(damageImage, 0, .75f);
        damageAudioSource.Play();
        if (CharacterStatisticsController.godMode) damage = Mathf.CeilToInt(damage * .1f);
        if(damage < CharacterStatisticsController.currentHealth)
        {
            CharacterStatisticsController.currentHealth -= damage;
            UIController.instance.UpdateHealthSlider(CharacterStatisticsController.currentHealth);
            return;
        }
        CharacterStatisticsController.currentHealth = 0;
        UIController.instance.UpdateHealthSlider(0);
        ManageDeath();
    }

    /// <summary>
    /// Heals the character - deprecated by the damage process, which allows healing as well.
    /// </summary>
    /// <param name="healAmount"></param>
    public void HealHealth(int healAmount)
    {
        if(healAmount + CharacterStatisticsController.currentHealth <= CharacterStatisticsController.maxhealth)
        {
            CharacterStatisticsController.currentHealth += healAmount;
            UIController.instance.UpdateHealthSlider(CharacterStatisticsController.currentHealth);
            return;
        }
        CharacterStatisticsController.currentHealth = CharacterStatisticsController.maxhealth;
        UIController.instance.UpdateHealthSlider(CharacterStatisticsController.currentHealth);
    }

    /// <summary>
    /// Calculates what to do in the event of death.
    /// </summary>
    void ManageDeath()
    {
        if(CharacterStatisticsController.currentLives - 1 <= 0)
        {
            GameController.instance.EndGame();
            return;
        }
        CharacterStatisticsController.currentLives -= 1;

    }

    public Animator deathAnimator;

    public void DeathAnim()
    {
        GetComponent<FirstPersonMovementController>().animator.SetBool("PutAway", true);
        GetComponent<FirstPersonMovementController>().animator.SetBool("Dead", true);
        deathAnimator.SetTrigger("Death");
        StartCoroutine(BoostExposure());
    }

    /// <summary>
    /// Boosts exposure up to high values during death, for some kind of visual effect representing death.
    /// </summary>
    /// <returns></returns>
    IEnumerator BoostExposure()
    {
        deathTeleportAudio.Play();
        yield return new WaitForSeconds(.5f);
        float exposure = 0;
        while(exposure < 25)
        {
            exposure += 25f * Time.deltaTime;
            colours.postExposure.Override(exposure);
            yield return new WaitForEndOfFrame();
        }
        GameController.instance.EndGameCalcs();
    }
}
