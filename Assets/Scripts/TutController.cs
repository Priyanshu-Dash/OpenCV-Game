using UnityEngine;
using UnityEngine.Events;

public class TutController : MonoBehaviour
{
    [Header("Animator Components")]
    public Animator animator1;
    public Animator animator2;
    
    [Header("Animation End Events")]
    public UnityEvent OnAnimator1AnimationEnd;
    public UnityEvent OnAnimator2AnimationEnd;
    
    [Header("Animation Settings")]
    [SerializeField] private string animator1EndTrigger = "AnimationEnd";
    [SerializeField] private string animator2EndTrigger = "AnimationEnd";
    [SerializeField] private float eventDelaySeconds = 3.0f;
    
    // Timer variables for delayed events
    private float animator1EndTime = -1f;
    private float animator2EndTime = -1f;
    private bool animator1EventTriggered = false;
    private bool animator2EventTriggered = false;
    
    private void Start()
    {
        // Validate animator references
        if (animator1 == null)
        {
            Debug.LogWarning("Animator1 is not assigned in TutController!");
        }
        
        if (animator2 == null)
        {
            Debug.LogWarning("Animator2 is not assigned in TutController!");
        }
    }
    
    private void Update()
    {
        // Check if animator1 animation has ended
        if (animator1 != null && IsAnimationFinished(animator1))
        {
            if (animator1EndTime < 0f) // Animation just ended
            {
                animator1EndTime = Time.time;
                animator1EventTriggered = false;
            }
        }
        
        // Check if animator2 animation has ended
        if (animator2 != null && IsAnimationFinished(animator2))
        {
            if (animator2EndTime < 0f) // Animation just ended
            {
                animator2EndTime = Time.time;
                animator2EventTriggered = false;
            }
        }
        
        // Check if it's time to trigger animator1 delayed event
        if (animator1EndTime > 0f && !animator1EventTriggered && 
            Time.time - animator1EndTime >= eventDelaySeconds)
        {
            OnAnimator1AnimationEnd?.Invoke();
            animator1EventTriggered = true;
        }
        
        // Check if it's time to trigger animator2 delayed event
        if (animator2EndTime > 0f && !animator2EventTriggered && 
            Time.time - animator2EndTime >= eventDelaySeconds)
        {
            OnAnimator2AnimationEnd?.Invoke();
            animator2EventTriggered = true;
        }
    }
    
    /// <summary>
    /// Checks if the current animation state has finished playing
    /// </summary>
    /// <param name="animator">The animator to check</param>
    /// <returns>True if animation has finished</returns>
    private bool IsAnimationFinished(Animator animator)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime >= 1.0f && !animator.IsInTransition(0);
    }
    
    /// <summary>
    /// Manually trigger animator1 animation end event (with delay)
    /// </summary>
    public void TriggerAnimator1End()
    {
        animator1EndTime = Time.time;
        animator1EventTriggered = false;
    }
    
    /// <summary>
    /// Manually trigger animator2 animation end event (with delay)
    /// </summary>
    public void TriggerAnimator2End()
    {
        animator2EndTime = Time.time;
        animator2EventTriggered = false;
    }
    
    /// <summary>
    /// Immediately trigger animator1 animation end event (bypasses delay)
    /// </summary>
    public void TriggerAnimator1EndImmediate()
    {
        OnAnimator1AnimationEnd?.Invoke();
    }
    
    /// <summary>
    /// Immediately trigger animator2 animation end event (bypasses delay)
    /// </summary>
    public void TriggerAnimator2EndImmediate()
    {
        OnAnimator2AnimationEnd?.Invoke();
    }
    
    /// <summary>
    /// Play animation on animator1
    /// </summary>
    /// <param name="animationName">Name of the animation to play</param>
    public void PlayAnimator1Animation(string animationName)
    {
        if (animator1 != null)
        {
            animator1.Play(animationName);
            // Reset timer for new animation
            animator1EndTime = -1f;
            animator1EventTriggered = false;
        }
        else
        {
            Debug.LogWarning("Animator1 is not assigned!");
        }
    }
    
    /// <summary>
    /// Play animation on animator2
    /// </summary>
    /// <param name="animationName">Name of the animation to play</param>
    public void PlayAnimator2Animation(string animationName)
    {
        if (animator2 != null)
        {
            animator2.Play(animationName);
            // Reset timer for new animation
            animator2EndTime = -1f;
            animator2EventTriggered = false;
        }
        else
        {
            Debug.LogWarning("Animator2 is not assigned!");
        }
    }
    
    /// <summary>
    /// Set trigger on animator1
    /// </summary>
    /// <param name="triggerName">Name of the trigger to set</param>
    public void SetAnimator1Trigger(string triggerName)
    {
        if (animator1 != null)
        {
            animator1.SetTrigger(triggerName);
            // Reset timer for new animation
            animator1EndTime = -1f;
            animator1EventTriggered = false;
        }
        else
        {
            Debug.LogWarning("Animator1 is not assigned!");
        }
    }
    
    /// <summary>
    /// Set trigger on animator2
    /// </summary>
    /// <param name="triggerName">Name of the trigger to set</param>
    public void SetAnimator2Trigger(string triggerName)
    {
        if (animator2 != null)
        {
            animator2.SetTrigger(triggerName);
            // Reset timer for new animation
            animator2EndTime = -1f;
            animator2EventTriggered = false;
        }
        else
        {
            Debug.LogWarning("Animator2 is not assigned!");
        }
    }
}
