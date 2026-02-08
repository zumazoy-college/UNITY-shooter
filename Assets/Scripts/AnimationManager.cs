using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    //аниматор прост создаем
    private Animator _Animator;

    private void Start()
    {
        //ссылку присваиваем геткампонент
        _Animator = GetComponent<Animator>();
    }

    public void SetAnimationIdle()
    {
        if (_Animator == null)
        {
            Debug.LogWarning("Animator is null, cannot set animation");
            return;
        }
        //обращаемся к аниматору, устанавливаем целочисленное значение,
        //указываем к какому параметру я хочу прикрутить.
        //соблюдаем регист аниматора и 0 
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

    public void SetAnimationFire()
    {
        _Animator.SetTrigger("Fire");
    }

    public void SetAnimationReload()
    {
        _Animator.SetTrigger("Reload");
    }

}