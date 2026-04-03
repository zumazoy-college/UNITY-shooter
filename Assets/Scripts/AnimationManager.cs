using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator _Animator;

    private void Start()
    {
        _Animator = GetComponent<Animator>();
    }

    public void SetAnimationIdle()
    {
        if (_Animator == null)
        {
            Debug.LogWarning("Animator is null, cannot set animation");
            return;
        }
        _Animator.SetInteger("Animation", 0);
    }

    public void SetAnimationWalk()
    {
        _Animator.SetInteger("Animation", 1);
    }

    public void SetAnimationRun()
    {
        _Animator.SetInteger("Animation", 2);
    }

    public void SetAnimationFire(float desiredDuration = 0f)
    {
        if (desiredDuration > 0f)
        {
            float clipLength = GetClipLength("fire");
            if (clipLength > 0f)
                _Animator.speed = clipLength / desiredDuration;
        }
        _Animator.SetTrigger("Fire");
    }

    public void ResetAnimatorSpeed()
    {
        if (_Animator != null)
            _Animator.speed = 1f;
    }

    public float GetClipLength(string clipName)
    {
        if (_Animator == null || _Animator.runtimeAnimatorController == null)
            return 0f;

        foreach (var clip in _Animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.StartsWith(clipName))
                return clip.length;
        }
        return 0f;
    }

    public void SetAnimationReload()
    {
        _Animator.SetTrigger("Reload");
    }

    public void SetBossDie()
    {
        _Animator.SetTrigger("Die");
    }
}