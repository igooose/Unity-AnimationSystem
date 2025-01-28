using System.Collections.Generic;
using UnityEngine;

namespace AnimationSystem
{
    [RequireComponent(typeof(Animator), typeof(AudioSource))]
    public class AnimationStateManager : MonoBehaviour
    {
        [SerializeField] List<AnimationState> m_states = new();
        private AnimationState m_currentState;
        private Animator m_animator;
        private AudioSource m_audioSource;
        private AnimationSystem m_animationSystem;

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            m_audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            m_animationSystem = new AnimationSystem(m_animator, m_audioSource);
        }

        private void Update()
        {
            if(m_currentState != null)
            {
                m_animationSystem.Update();
            }
        }

        private void OnDestroy()
        {
            m_animationSystem.Destroy();
        }

        private AnimationState GetState(string stateName)
        {
            foreach(AnimationState state in m_states)
            {
                if(state.name == stateName)
                {
                    return state;
                }
            }

            return null;
        }

        public void PlayOverride(string stateName, AnimationState animationStateOverride, float transitionDuration = 0f)
        {
            if(!IsPlaying(stateName) && !m_animationSystem.IsTransitioning())
            {
                m_currentState = GetState(stateName);

                if(m_currentState != null)
                {
                    if(animationStateOverride.animation.clip != null)
                    {
                        m_currentState = animationStateOverride;
                        m_currentState.name = stateName;
                    }
                    else
                    {
                        Debug.LogError($"Animation state override clip not found.");
                    }

                    m_animationSystem.Play(m_currentState, transitionDuration);
                }
                else
                {
                    Debug.LogError($"Animation state [{stateName}] not found.");
                }
            }
        }

        public void Play(string stateName, float transitionDuration = 0f)
        {
            if(!IsPlaying(stateName) && !m_animationSystem.IsTransitioning())
            {
                m_currentState = GetState(stateName);

                if(m_currentState != null)
                {
                    m_animationSystem.Play(m_currentState, transitionDuration);
                }
                else
                {
                    Debug.LogError($"Animation state [{stateName}] not found.");
                }
            }
        }

        public bool IsPlaying(string stateName)
        {
            return m_currentState?.name == stateName && m_animationSystem.GetCurrentNormalizeTime() < 1f;
        }
    }
}
