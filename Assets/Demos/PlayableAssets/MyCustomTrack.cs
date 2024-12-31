using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.25f, 0.5f, 1.0f)]
[TrackClipType(typeof(MyCustomPlayableAsset))]  // �������PlaybleAsset�����ڴ�Track��ʹ��
public class MyCustomTrack : TrackAsset
{
    // Track����Ϊ�������������
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var scriptPlayable = ScriptPlayable<MyCustomPlayableBehaviour>.Create(graph, inputCount);
        return scriptPlayable;
    }
}
