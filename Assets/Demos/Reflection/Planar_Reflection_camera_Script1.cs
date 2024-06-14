using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Planar_Reflection_camera_Script1 : MonoBehaviour
{
    [ExecuteAlways]
    public Camera Main_camera;
    public Camera Reflection_camera;
    public Camera Scene_Reflection_camera;
    public GameObject Plane;

    private readonly RenderTexture _Reflection_camera_RT;//���巴��RT
    private int _Reflection_camera_RT_ID;//����������_MainTex�������Ƶ�ID
    public Material Reflection_material;//�������

    private readonly RenderTexture _Scene_Reflection_camera_RT;
    private int _Scene_Reflection_camera_RT_ID;

    public Shader shader;

    void Start()//Start�����ڽű����п�ʼ��ʱ��ִ��
    {
        Debug.Log("Planar Reflection succes!");
        if (this.Reflection_camera == null)
        {
            var R_gameobject = new GameObject("Reflection camera");//���������
            this.Reflection_camera = R_gameobject.AddComponent<Camera>();//��ȡCamera���͵����
        }

        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;//�����¼�

    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == this.Reflection_camera)
        {
            Update_camera(this.Reflection_camera);
            camera.clearFlags = CameraClearFlags.SolidColor;//����ճ�ʼ����Ϣ��Reflection_camera�е�DepthBuffer��ColorBuffer,��Background������ɫ���
            camera.backgroundColor = Color.clear;//�����������ɫ
            camera.cullingMask = LayerMask.GetMask("Reflection");//ȷ�����������Ⱦ��

            var Reflection_camera_M = CalculateReflectionCameraMatrix(this.Plane.transform.up, this.Plane.transform.position);//�����������
            Reflection_camera.worldToCameraMatrix = Reflection_camera.worldToCameraMatrix * Reflection_camera_M;//�ڽ�VP�任֮ǰ
            GL.invertCulling = true;//���ü�˳��ת��ȥ����Ϊ�������ı仯������ü�˳��ı仯

            //���������׶��ü�
            Vector4 viewPlane = new Vector4(this.Plane.transform.up.x,
                this.Plane.transform.up.y,
                this.Plane.transform.up.z,
                -Vector3.Dot(this.Plane.transform.position, this.Plane.transform.up));//����ά������ʾƽ��
            viewPlane = Reflection_camera.worldToCameraMatrix.inverse.transpose * viewPlane;//������ռ��е�ƽ���ʾת��������ռ��е�ƽ���ʾ

            var ClipMatrix = Reflection_camera.CalculateObliqueMatrix(viewPlane);//��ȡ�Է���ƽ��Ϊ��ƽ���ͶӰ����
            Reflection_camera.projectionMatrix = ClipMatrix;//��ȡ�µ�ͶӰ����

            UniversalRenderPipeline.RenderSingleCamera(context, camera);//�������ʼ��Ⱦ

            RenderTexture Reflection_camera_temporary_RT = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);//������ʱ��RT����ͼ���棩

            _Reflection_camera_RT_ID = Shader.PropertyToID("_Reflection_camera_RT");//�Ȼ�ȡ��ɫ����������_Reflection_camera_RT��Ψһ��ʶ��_Reflection_camera_RT_ID 
            Shader.SetGlobalTexture(_Reflection_camera_RT_ID, Reflection_camera_temporary_RT);//������_Reflection_camera_RT_ID��Reflection_camera_temporary_RTΪ������ɫ������һ��ȫ������

            camera.targetTexture = Reflection_camera_temporary_RT;//�����Զ������Ⱦ����Ϊ�������Ŀ������������ɺ󣬷������������ͻ������������

            Reflection_material.SetTexture(_Reflection_camera_RT_ID, Reflection_camera_temporary_RT);//����ͼ��������

            RenderTexture.ReleaseTemporary(Reflection_camera_temporary_RT);//�ͷŵ���ʱ����

        }

        else
        {
            GL.invertCulling = false;
        }
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;//ȡ���¼�����
    }

    private void Update_camera(Camera Reflection_camera)//ͬ����������������ݣ��൱�ڷ��������ʼ��
    {
        if (Reflection_camera == null || this.Main_camera == null)
            return;
        //��ͬ����������������ݣ���ʼ����Reflection_camera�����̱�����ɫ����ȵ������Ȼ�������������ʼ��Ⱦǰ�ĸ�������
        int target_display = Reflection_camera.targetDisplay;
        Reflection_camera.CopyFrom(this.Main_camera);
        Reflection_camera.targetDisplay = target_display;

    }


    private Matrix4x4 CalculateReflectionCameraMatrix(Vector3 N, Vector3 plane_position)//���㷵�ط������
    {
        //������㷴�������������ռ�����
        Matrix4x4 Reflection_camera_M = Matrix4x4.identity;//��ʼ���������
        float d = -Vector3.Dot(plane_position, N);//d = -dot(P, N),P��ƽ���ϵ�����һ�㣬N��ƽ��ķ�����

        Reflection_camera_M.m00 = 1 - 2 * N.x * N.x;
        Reflection_camera_M.m01 = -2 * N.x * N.y;
        Reflection_camera_M.m02 = -2 * N.x * N.z;
        Reflection_camera_M.m03 = -2 * N.x * d;

        Reflection_camera_M.m10 = -2 * N.x * N.y;
        Reflection_camera_M.m11 = 1 - 2 * N.y * N.y;
        Reflection_camera_M.m12 = -2 * N.y * N.z;
        Reflection_camera_M.m13 = -2 * N.y * d;

        Reflection_camera_M.m20 = -2 * N.x * N.z;
        Reflection_camera_M.m21 = -2 * N.y * N.z;
        Reflection_camera_M.m22 = 1 - 2 * N.z * N.z;
        Reflection_camera_M.m23 = -2 * N.z * d;

        Reflection_camera_M.m30 = 0;
        Reflection_camera_M.m31 = 0;
        Reflection_camera_M.m32 = 0;
        Reflection_camera_M.m33 = 1;

        return Reflection_camera_M;
    }
}

