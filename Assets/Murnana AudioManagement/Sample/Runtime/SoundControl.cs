// Copyright (c) Murnana
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Murnana.AudioManagement.Sample
{
    [DisallowMultipleComponent]
    public sealed class SoundControl : MonoBehaviour
    {
        [Header(header: "Audio")]
        [SerializeField]
        private AssetReferenceT<AudioClip> m_AudioClipReference = null!;

        [SerializeField] private bool m_Loop;

        [Header(header: "Mixer")]
        [SerializeField]
        private AudioMixer m_AudioMixer = null!;

        [SerializeField] private AudioMixerGroup m_OutputMixerGroup = null!;
        [SerializeField] private string          m_VolumeParameter  = "";

        [Header(header: "UI")]
        [SerializeField]
        private Button m_OnOffButton = null!;

        [SerializeField] private TextMeshProUGUI m_ButtonLabel  = null!;
        [SerializeField] private Slider          m_SliderVolume = null!;

        private AudioSource                     m_AudioSource = null!;
        private AsyncOperationHandle<AudioClip> m_Handle;
        private bool                            m_IsPlaying;
        private Coroutine?                      m_PlaybackCoroutine;

        private void Awake()
        {
            m_AudioSource                       = gameObject.AddComponent<AudioSource>();
            m_AudioSource.playOnAwake           = false;
            m_AudioSource.loop                  = m_Loop;
            m_AudioSource.outputAudioMixerGroup = m_OutputMixerGroup;

            m_SliderVolume.minValue = 0.0001f;
            m_SliderVolume.maxValue = 1f;
            m_SliderVolume.value    = 1f;

            SetMixerVolume(linearValue: 1f);

            m_ButtonLabel.text = "Play";
        }

        private void OnEnable()
        {
            m_OnOffButton.onClick.AddListener(call: OnClickToggle);
            m_SliderVolume.onValueChanged.AddListener(call: OnSliderValueChanged);
        }

        private void OnDisable()
        {
            m_OnOffButton.onClick.RemoveListener(call: OnClickToggle);
            m_SliderVolume.onValueChanged.RemoveListener(call: OnSliderValueChanged);
        }

        private void OnDestroy()
        {
            if(m_Handle.IsValid())
            {
                if(m_Handle.IsDone)
                {
                    Addressables.Release(handle: m_Handle);
                }
                else
                {
                    m_Handle.Completed -= OnAudioClipLoaded;
                    m_Handle.Completed += handler => { handler.Release(); };
                }

                m_Handle = default;
            }
        }

        private void OnClickToggle()
        {
            if(m_IsPlaying)
            {
                StopPlayback();
            }
            else
            {
                StartPlayback();
            }
        }

        private void StartPlayback()
        {
            m_IsPlaying        = true;
            m_ButtonLabel.text = "Stop";

            if(m_Handle.IsValid() && m_Handle.IsDone)
            {
                PlayAudio(clip: m_Handle.Result);
            }
            else
            {
                m_Handle           =  Addressables.LoadAssetAsync<AudioClip>(key: m_AudioClipReference);
                m_Handle.Completed += OnAudioClipLoaded;
            }
        }

        private void StopPlayback()
        {
            m_IsPlaying        = false;
            m_ButtonLabel.text = "Play";

            if(m_PlaybackCoroutine != null)
            {
                StopCoroutine(routine: m_PlaybackCoroutine);
                m_PlaybackCoroutine = null;
            }

            m_AudioSource.Stop();
        }

        private void OnAudioClipLoaded(AsyncOperationHandle<AudioClip> handle)
        {
            if(handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError(message: $"Failed to load AudioClip: {handle.OperationException}", context: this);
                m_IsPlaying        = false;
                m_ButtonLabel.text = "Play";
                return;
            }

            if(m_IsPlaying)
            {
                PlayAudio(clip: handle.Result);
            }
        }

        private void PlayAudio(AudioClip clip)
        {
            m_AudioSource.clip = clip;
            m_AudioSource.Play();

            if(!m_Loop)
            {
                m_PlaybackCoroutine = StartCoroutine(routine: WaitForPlaybackEnd(duration: clip.length));
            }
        }

        private IEnumerator WaitForPlaybackEnd(float duration)
        {
            yield return new WaitForSeconds(seconds: duration);

            if(m_IsPlaying)
            {
                m_IsPlaying        = false;
                m_ButtonLabel.text = "Play";
            }

            m_PlaybackCoroutine = null;
        }

        private void OnSliderValueChanged(float value)
        {
            SetMixerVolume(linearValue: value);
        }

        private void SetMixerVolume(float linearValue)
        {
            var dB = Mathf.Log10(f: Mathf.Max(a: linearValue, b: 0.0001f)) * 20f;
            m_AudioMixer.SetFloat(name: m_VolumeParameter, value: dB);
        }
    }
}
