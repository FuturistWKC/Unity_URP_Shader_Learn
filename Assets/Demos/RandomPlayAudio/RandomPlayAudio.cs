using System.Collections;
using UnityEngine;

public class RandomPlayAudio : MonoBehaviour
{
    public AudioClip[] clips;
    AudioSource m_AudioSource;
    Coroutine m_Coroutine;

    void OnEnable()
    {
        StopPlay();
        m_Coroutine = StartCoroutine(Play());
    }
    void OnDisable()
    {
        StopPlay();
    }
    void StopPlay()
    {
        if (m_Coroutine != null)
            StopCoroutine(m_Coroutine);
    }
    bool CheckAudioSource()
    {
        if (m_AudioSource == null)
        {
            m_AudioSource = GetComponent<AudioSource>();
            if (m_AudioSource == null)
                m_AudioSource = gameObject.AddComponent<AudioSource>();
        }
        return m_AudioSource != null;
    }
    IEnumerator Play()
    {
        if (!CheckAudioSource())
        {
            Debug.LogError("RandomPlayAudio: ��ȡAudioʧ�ܣ�");
            yield break;
        }
        while (true)
        {
            if (clips.Length <= 0)
            {
                Debug.LogError("RandomPlayAudio: �������Ƶ��clips��");
                yield break;
            }
            var audioClipIndex = Random.Range(0, clips.Length);
            var audioClip = clips[audioClipIndex];
            var waitTime = 0f;
            if (audioClip != null)
            {
                m_AudioSource.clip = audioClip;
                m_AudioSource.Play();
                waitTime = audioClip.length;
            }
            else
            {
                Debug.Log($"<color=red>RandomPlayAudio: clips��IndexΪ{audioClipIndex}����Ƶ�ļ�Ϊ�գ�</color>");
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
}