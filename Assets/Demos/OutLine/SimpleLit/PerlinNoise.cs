using System.IO;
using UnityEditor;
using UnityEngine;

public class NoiseGenerator : EditorWindow
{
    int x;
    int y;
    string texName;
    int scale = 1;
    int step = 1;

    [MenuItem("Tools/��������ͼ")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(NoiseGenerator)).Show();
    }

    void GenerateNoiseImage(int x, int y)
    {
        int size = Mathf.Min(x, y);
        Texture2D tex = new Texture2D(x, y, TextureFormat.RGB24, false);
        Color[] pixel = new Color[x * y];
        for (int i = 0; i < x; i = i + step)
        {
            pixel[i] = new Color(1, 1, 1);
        }
        for (int i = 0; i < x; i = i + step)
        {
            for (int j = 0; j < x; j = j + step)
            {
                float sample = Mathf.PerlinNoise((float)i / x * scale, (float)j / y * scale);
                sample = Mathf.Clamp(sample, 0f, 1f);
                pixel[i * size + j] = new Color(sample, sample, sample);
            }
        }
        tex.SetPixels(pixel);
        tex.Apply();

        File.WriteAllBytes(System.Environment.CurrentDirectory + "\\Assets\\" + texName + ".png", tex.EncodeToPNG());
        EditorUtility.DisplayDialog("�ɹ�", "����ͼ\"" + texName + "\"" + "����AssetsĿ¼�����ɣ�", "ȷ��", "ȡ��");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        texName = EditorGUILayout.TextField("ͼƬ��: ", texName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        y = EditorGUILayout.IntField("ͼƬ��: ", y);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        x = EditorGUILayout.IntField("ͼƬ��: ", x);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        scale = EditorGUILayout.IntField("Scale: ", scale);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        step = EditorGUILayout.IntField("Step: ", step);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("����"))
        {
            if (x < 1 || y < 1)
            {
                Debug.LogError("ͼƬ�����������1");
                return;
            }
            if (texName == null || texName.Length < 1)
            {
                Debug.LogError("��ΪͼƬ����");
                return;
            }
            if (step >= x || step >= y)
            {
                Debug.LogError("�ܶȲ��ܳ���ͼƬ�ĳ����");
                return;
            }
            else if (step < 1)
            {
                step = 1;
            }
            GenerateNoiseImage(x, y);
        }
        EditorGUILayout.EndHorizontal();
    }
}