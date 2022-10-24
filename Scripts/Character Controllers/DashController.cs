using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace CharacterScripts
{
    public class DashController : MonoBehaviour
    {
        [Header("Dash variables")]
        [Tooltip("The maximum number of dashes the player can perform before needing to wait to regain more")]
        public int maxNumberOfDashes = 2;
        [Tooltip("The distance covered by the dash")]
        public float distance = 10f;
        [Tooltip("Time the dash takes to complete")]
        public float dashTime = .25f;
        [Tooltip("Time after the player dashes before the dash pool is refilled")]
        public float timeToMoreDashes = 1f;

        [Header("Buff variables")]
        [Tooltip("Duration of the player buffed state")]
        public float buffDuration = .5f;

        public AudioSource dashSound;
        public CanvasGroup dashEffectImage;
        

        // internal variables
        float gravity;
        bool dashing;
        private int dashesRemaining;
        float dashTimer;
        public bool dashBuffed;
        VolumeProfile postProc;
        ChromaticAberration chromaticAberration;
        public float defaultChromAbber;

        // script references
        FirstPersonMovementController controller;
        PlayerInputs input;

        private void Awake()
        {
            controller = GetComponent<FirstPersonMovementController>();
            input = GetComponent<PlayerInputs>();
            gravity = controller.gravity;
            dashesRemaining = maxNumberOfDashes - 1;
            postProc = GameObject.Find("Post Processing Volume").GetComponent<Volume>().profile;
            postProc.TryGet(out chromaticAberration);
            defaultChromAbber = (float)chromaticAberration.intensity;
            dashEffectImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// Checks for dash input and dash number reset
        /// </summary>
        void Update()
        {
            if(dashesRemaining == 0)
            {
                input.dash = false;
            }
            AddNewDash();
            if(input.dash && !dashing && dashesRemaining > 0)
            {
                StartCoroutine(Dash());
                input.dash = false;
                dashSound.Play();
                dashing = true;
                dashesRemaining -= 1;
                TutorialManager.instance.FinishTut4();
            }
        }

        /// <summary>
        /// Moves the player a set distance in a set period of time in their current movement direction, while disabling gravity and allowing the application of buffs.
        /// </summary>
        /// <returns></returns>
        IEnumerator Dash()
        {
            float startTime = Time.time;
            float speed = distance / dashTime;
            controller.gravity = 0;
            dashBuffed = true;
            CharacterCombatController.instance.dashInvulnerable = true;
            chromaticAberration.intensity.Override(1.0f);
            dashEffectImage.gameObject.SetActive(true);
            dashEffectImage.alpha = 1;
            LeanTween.alphaCanvas(dashEffectImage, 0, dashTime);

            while (Time.time < startTime + dashTime)
            {
                // Unsure why this needs to be multiplied by .5 to achieve the correct distance, but it does
                controller.controller.Move(controller.targetDirection.normalized * speed * Time.deltaTime * .5f);

                yield return new WaitForEndOfFrame();
            }
            controller.gravity = gravity;
            yield return new WaitForSeconds(.05f);
            chromaticAberration.intensity.Override(defaultChromAbber);
            dashing = false;
            yield return new WaitForSeconds(buffDuration);
            dashBuffed = false;
            CharacterCombatController.instance.dashInvulnerable = false;
            dashEffectImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// Sets the current available dashes variable equal to the max dashes after a certain amount of time has passed, since the player using a dash.
        /// </summary>
        void AddNewDash()
        {
            if(dashesRemaining < maxNumberOfDashes)
            {
                if (dashTimer < 0)
                {
                    dashesRemaining = maxNumberOfDashes;
                    dashTimer = timeToMoreDashes;
                }
                dashTimer -= Time.deltaTime;
            }
        }
    }

}