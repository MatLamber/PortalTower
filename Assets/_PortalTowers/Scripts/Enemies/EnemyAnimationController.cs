using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private string runParameter = "Run";
    private string cycleOffsetParameter = "CycleOffset";
    public Animator Animator => animator;
    private readonly int reactionLayer = 1;
    private readonly string reactParameter = "React";
    private readonly string attackParameter = "Attack";
    private bool isStuned;

    public bool IsStuned
    {
        get => isStuned;
        set => isStuned = value;
    }

    private void Start()
    {
        animator.SetFloat(cycleOffsetParameter, Random.Range(0f,1f));
    }

    public void PlayRunAnimation()
    {
        animator.SetBool(runParameter,true);
    }

    public void PlayReactionAnimation()
    {
        StartCoroutine(ReactionAnimationCoroutine());
    }


    public void PlayAttackAnimation()
    {
        StartCoroutine(AttackAnimationCoroutine());
    }
    
    


    IEnumerator ReactionAnimationCoroutine()
    {
        animator.SetLayerWeight(0,0);
        animator.SetLayerWeight(reactionLayer, 1);
        animator.SetTrigger(reactParameter);
        isStuned = true;
        yield return new WaitForSeconds(0.35f);
        isStuned = false;
        animator.SetLayerWeight(0,1);
        animator.SetLayerWeight(reactionLayer, 0);
        
    }
    
    IEnumerator AttackAnimationCoroutine()
    {
        animator.SetLayerWeight(0,0);
        animator.SetLayerWeight(reactionLayer, 1);
        animator.SetTrigger(attackParameter);
        yield return new WaitForSeconds(1.3f);
        animator.SetLayerWeight(0,1);
        animator.SetLayerWeight(reactionLayer, 0);
        
    }
}
