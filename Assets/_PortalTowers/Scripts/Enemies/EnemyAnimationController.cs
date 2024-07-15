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
    private void Start()
    {
        animator.SetFloat(cycleOffsetParameter, Random.Range(0f,1f));
    }

    public void PlayRunAnimation()
    {
        animator.SetBool(runParameter,true);
    }
}
