using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AnimationSystem
{
    [Serializable]
    public class AnimationState
    {
        public string name;
        public Animation animation = new();
        public Audio audio = new();
        public List<AnimationEvent> animationEvents = new();
    }

    [Serializable]
    public class Animation
    {
        public AnimationClip clip;
        public bool loop;
    }

    [Serializable]
    public class AnimationEvent
    {
        public string name;
        [Range(0f, 1f)] public float time;
        public UnityEvent unityEvent;
        private bool m_isInvoked;

        public void InvokeEvent()
        {
            if(!m_isInvoked)
            {
                unityEvent?.Invoke();
                m_isInvoked = true;
            }
        }

        public void ResetInvoke()
        {
            m_isInvoked = false;
        }
    }

    [Serializable]
    public class Audio
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }
}
