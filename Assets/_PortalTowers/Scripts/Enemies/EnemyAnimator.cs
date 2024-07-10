using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{

    [Header("Elements")] 
    [SerializeField] private Animator animator;
    private string runParameter = "Run";

    public void PlayRunAnimation()
    {
        animator.SetBool(runParameter,true);
    }
    
    
}
