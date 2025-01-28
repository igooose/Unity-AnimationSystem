using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine.Playables;

namespace AnimationSystem
{
    public class AnimationSystem
    {
        private AnimationState m_currentAnimationState;
        private PlayableGraph m_playableGraph;

        private AnimationClipPlayable m_currentAnimationClipPlayableDebug;
        private AnimationClipPlayable m_currentAnimationClipPlayable;
        private AnimationClipPlayable m_nextAnimationClipPlayable;
        private AnimationMixerPlayable m_animationMixerPlayable;
        private AnimationPlayableOutput m_animationPlayableOutput;
        private float m_transitionDuration;
        private float m_transitionProgress;
        private bool m_isTransitioning;

        private AudioClipPlayable m_currentAudioClipPlayable;
        private AudioPlayableOutput m_audioPlayableOutput;

        public AnimationSystem(Animator animator, AudioSource audioSource)
        {
            m_playableGraph = PlayableGraph.Create($"{animator.gameObject.name}_PlayableGraph");
            m_playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            m_animationMixerPlayable = AnimationMixerPlayable.Create(m_playableGraph, 2);
            m_animationPlayableOutput = AnimationPlayableOutput.Create(m_playableGraph, "Animation", animator);
            m_animationPlayableOutput.SetSourcePlayable(m_animationMixerPlayable);

            m_audioPlayableOutput = AudioPlayableOutput.Create(m_playableGraph, "Audio", audioSource);
            m_audioPlayableOutput.SetSourcePlayable(m_currentAudioClipPlayable);

            m_playableGraph.Play();
        }

        public void Update()
        {
            if(m_playableGraph.IsValid())
            {
                if (m_currentAnimationState != null)
                {
                    HandleEventTrigger();
                    HandleLoop();
                }

                HandleTransition();
            }
        }

        private void HandleTransition()
        {
            if (m_isTransitioning)
            {
                m_transitionProgress += Time.deltaTime / m_transitionDuration;
                float nextWight = Mathf.Clamp01(m_transitionProgress);
                float currentWeight = 1f - nextWight;
                m_animationMixerPlayable.SetInputWeight(0, currentWeight);
                m_animationMixerPlayable.SetInputWeight(1, nextWight);

                if (m_transitionProgress >= 1f)
                {
                    m_isTransitioning = false;

                    m_animationMixerPlayable.DisconnectInput(0);
                    m_animationMixerPlayable.DisconnectInput(1);

                    m_animationMixerPlayable.ConnectInput(0, m_nextAnimationClipPlayable, 0);
                    m_animationMixerPlayable.SetInputWeight(0, 1f);
                    m_animationMixerPlayable.SetInputWeight(1, 0f);

                    m_currentAnimationClipPlayable = m_nextAnimationClipPlayable;
                    m_nextAnimationClipPlayable = default;
                }
            }
        }

        private void HandleLoop()
        {
            if (m_currentAnimationState.animation.loop)
            {
                if (GetCurrentNormalizeTime() >= 1f)
                {
                    m_currentAnimationClipPlayable.SetTime(0f);
                    m_currentAnimationClipPlayableDebug.SetTime(0f);

                    // reset event invoked
                    ResetEventInvoked();
                }
            }
        }

        private void ResetEventInvoked()
        {
            foreach (var animationEvent in m_currentAnimationState.animationEvents)
            {
                animationEvent.ResetInvoke();
            }
        }

        private void HandleEventTrigger()
        {
            foreach (var animationEvent in m_currentAnimationState.animationEvents)
            {
                if (GetCurrentNormalizeTime() >= animationEvent.time)
                {
                    animationEvent.InvokeEvent();
                }
            }
        }

        public void Destroy()
        {
            if(m_playableGraph.IsValid())
            {
                m_playableGraph.Destroy();
            }
        }

        public void Play(AnimationState animationState, float transitionDuration)
        {
            if(m_currentAnimationClipPlayable.IsValid() && !m_isTransitioning)
            {
                if(m_currentAnimationClipPlayable.GetAnimationClip() != animationState.animation.clip && !m_isTransitioning)
                {
                    // reset event invoked
                    if(m_currentAnimationState != null)
                    {
                        ResetEventInvoked();
                    }

                    // reset audio
                    if(m_currentAudioClipPlayable.IsValid())
                    {
                        m_currentAudioClipPlayable.Destroy();
                    }

                    m_nextAnimationClipPlayable = AnimationClipPlayable.Create(m_playableGraph, animationState.animation.clip);
                    m_animationMixerPlayable.ConnectInput(1, m_nextAnimationClipPlayable, 0);
                    m_animationMixerPlayable.SetInputWeight(1, 0f);

                    m_transitionDuration = transitionDuration;
                    m_transitionProgress = 0f;
                    m_isTransitioning = true;

                    SetAudio(animationState);
                    
                    m_currentAnimationClipPlayableDebug = m_nextAnimationClipPlayable;
                    m_currentAnimationState = animationState;
                }
            }
            else
            {
                m_currentAnimationClipPlayable = AnimationClipPlayable.Create(m_playableGraph, animationState.animation.clip);
                m_animationMixerPlayable.ConnectInput(0, m_currentAnimationClipPlayable, 0);
                m_animationMixerPlayable.SetInputWeight(0, 1f);

                SetAudio(animationState);

                m_currentAnimationClipPlayableDebug = m_currentAnimationClipPlayable;
                m_currentAnimationState = animationState;
            }
        }

        private void SetAudio(AnimationState animationState)
        {
            if (animationState.audio != null)
            {
                m_currentAudioClipPlayable = AudioClipPlayable.Create(m_playableGraph, animationState.audio.clip, animationState.animation.loop);
                m_audioPlayableOutput.SetSourcePlayable(m_currentAudioClipPlayable);
            }
        }

        public double GetCurrentNormalizeTime()
        {
            if(m_currentAnimationClipPlayableDebug.IsValid())
            {
                return m_currentAnimationClipPlayableDebug.GetTime() / m_currentAnimationState.animation.clip.length;
            }
            else
            {
                return 0f;
            }
        }

        public bool IsTransitioning()
        {
            return m_isTransitioning;
        }
    }
}