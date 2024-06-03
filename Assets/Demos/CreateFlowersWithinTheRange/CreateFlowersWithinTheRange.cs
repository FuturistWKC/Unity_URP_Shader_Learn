using System.Collections;
using UnityEngine;

public class CreateFlowersWithinTheRange : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Ԥ����")]
    GameObject flowerPrefab;
    [SerializeField]
    [Tooltip("���ɷ�Χ��x����y���ң�z��ǰ")]
    Vector3 m_GenerationRange = new Vector3(-5, 5, 10);
    [SerializeField]
    [Tooltip("��ת��Χ")]
    Vector2 m_RotationRange = new Vector2(10f, 30f);
    [SerializeField]
    [Tooltip("���ŷ�Χ��0-180")]
    Vector2 m_ScaleRange = new Vector2(1, 2);
    [SerializeField]
    [Tooltip("����")]
    int count = 100;
    [SerializeField]
    [Tooltip("����ʱ��")]
    float duration = 3;

    GameObject[] m_Objs;
    Coroutine m_Coroutine;
    void OnEnable()
    {
        m_Objs = new GameObject[count];
        StopCoroutine();
        m_Coroutine = StartCoroutine(Generate());
    }
    void OnDisable()
    {
        StopCoroutine();
        Destroy();
    }
    IEnumerator Generate()
    {
        var generationCount = 0;
        var countPerSecond = (int)(count / duration) * Time.fixedDeltaTime;
        var forwardDistance = 0f;
        var forwardStepPerSecond = m_GenerationRange.z / duration * Time.fixedDeltaTime;

        while (generationCount < count)
        {
            for (int i = 0; i < countPerSecond; i++)
            {
                var obj = Instantiate(flowerPrefab);
                obj.transform.SetParent(transform, false);

                var pX = Random.Range(m_GenerationRange.x, m_GenerationRange.y);
                obj.transform.position = transform.position + new Vector3(pX, 0, forwardDistance);

                var dX = Random.Range(m_RotationRange.x / 180, m_RotationRange.y / 180);
                var dZ = Random.Range(m_RotationRange.x / 180, m_RotationRange.y / 180);
                obj.transform.rotation = Quaternion.LookRotation(Vector3.up + new Vector3(dX, 0, dZ));

                var scale = Random.Range(m_ScaleRange.x, m_ScaleRange.y);
                obj.transform.localScale = Vector3.one * scale;

                m_Objs[generationCount++] = obj;

                if (generationCount >= count - 1) break;
            }
            forwardDistance += forwardStepPerSecond;
            yield return new WaitForFixedUpdate();
        }
    }
    void StopCoroutine()
    {
        if (m_Coroutine != null)
            StopCoroutine(m_Coroutine);
    }
    void Destroy()
    {
        if (m_Objs != null)
        {
            foreach (var obj in m_Objs)
                Destroy(obj);
            m_Objs = null;
        }
    }
}