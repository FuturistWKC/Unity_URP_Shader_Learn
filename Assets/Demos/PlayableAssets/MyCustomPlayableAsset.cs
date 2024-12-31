using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class MyCustomPlayableAsset : PlayableAsset
{
    // ����������ﶨ��Ҫ���ŵ��κ����ݣ��������顢ʱ���
    public float someValue = 1.0f;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // ����������һ��PlayableBehaviourʵ��
        var playable = ScriptPlayable<MyCustomPlayableBehaviour>.Create(graph);

        MyCustomPlayableBehaviour behaviour = playable.GetBehaviour();
        behaviour.someValue = someValue;  // ���������ݸ�Behaviour

        return playable;
    }
}
