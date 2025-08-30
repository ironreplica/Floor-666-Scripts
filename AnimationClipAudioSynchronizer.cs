using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationClipAudioSynchronizer : StateMachineBehaviour
{
    public AudioClip clip;
    private AudioSource audioSource;
    public bool hasAnimationClip = true;
    private ElevatorSoundPlay elevatorSoundPlay; // Monobehavior to start audio couroutine

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSource = GameObject.Find("ElevatorAudioSource").GetComponent<AudioSource>();
        elevatorSoundPlay = GameObject.Find("Elevator").GetComponent<ElevatorSoundPlay>();
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;

            if (hasAnimationClip)
            {
                float clipLength = audioSource.clip.length;
                float animationLength = stateInfo.length;
                float speedMultiplier = animationLength / clipLength;
                animator.speed = speedMultiplier;
                audioSource.Play();
            }
            else
            {
                elevatorSoundPlay.StartCoroutine(PlayAudioAndTransition(animator));
            }
        }
    }

    private IEnumerator PlayAudioAndTransition(Animator animator)
    {
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        animator.SetTrigger("NextState"); // Assume "NextState" is the trigger to move to the next state
    }

}
