using System.Collections.Generic;
using UnityEngine;
using AnimationSystem;

public class PlayerController : MonoBehaviour
{
    private AnimationStateManager m_animation;
    [SerializeField] AnimationClip m_attackOverrideAnimationClip;
    [SerializeField] AudioClip m_attackOverrideAudioClip;
    [SerializeField] List<AnimationSystem.AnimationEvent> m_attackOverrideAnimationEvents;

    private void Awake()
    {
        m_animation = GetComponentInChildren<AnimationStateManager>();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            m_animation.Play("Attack", 0.1f);
        }
        else if(Input.GetButtonDown("Fire2"))
        {
            AnimationSystem.AnimationState attackOverrideState = new();
            attackOverrideState.animation.clip = m_attackOverrideAnimationClip;
            attackOverrideState.audio.clip = m_attackOverrideAudioClip;
            attackOverrideState.animationEvents = m_attackOverrideAnimationEvents;

            m_animation.PlayOverride("Attack", attackOverrideState, 0.1f);
        }

        if(!m_animation.IsPlaying("Attack"))
        {
            if(GetMoveInput().sqrMagnitude > 0)
            {
                m_animation.Play("Move", 0.1f);
            }
            else
            {
                m_animation.Play("Idle", 0.1f);
            }
        }
    }

    private Vector2 GetMoveInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public void ApplyDamage(int damage)
    {
        Debug.Log($"dealt {damage} damage");
    }
}
