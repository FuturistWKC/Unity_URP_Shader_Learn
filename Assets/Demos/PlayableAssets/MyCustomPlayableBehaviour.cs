using UnityEngine;
using UnityEngine.Playables;

public class MyCustomPlayableBehaviour : PlayableBehaviour
{
    public float someValue;

    // �ڲ���ʱ����
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);

        // ����������Կ�����Ľű������ʱ�����в���
        Debug.Log("Time: " + playable.GetTime() + " Value: " + someValue);
    }
}
