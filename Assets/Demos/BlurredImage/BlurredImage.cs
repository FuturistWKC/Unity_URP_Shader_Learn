using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.UI
{
    //-----------------------------------------------------------------Image-----------------------------------------------------------------//
    public partial class BlurredImage : Image
    {
        [SerializeField]
        int m_BlurSize = 2;
        public int blurSize
        {
            get => m_BlurSize;
            set
            {
                m_BlurSize = value;
                m_RenderPass.BlurSize = m_BlurSize / 2f;
                SetVerticesDirty();
            }
        }
        BlurredImageRenderPass m_RenderPass;
        protected override void Awake()
        {
            base.Awake();
            CreatePass();
            material = new Material(Shader.Find("URP Shader/Blurred Image"));
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            CreatePass();
            RenderPipelineManager.beginCameraRendering += beginCameraRendering;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            RenderPipelineManager.beginCameraRendering -= beginCameraRendering;
            if (m_RenderPass != null)
                m_RenderPass.ReleaseRT();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            RenderPipelineManager.beginCameraRendering -= beginCameraRendering;
            if (m_RenderPass != null)
                m_RenderPass.ReleaseRT();
        }
        void beginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera == null || !camera.isActiveAndEnabled || !camera.CompareTag("MainCamera")) return;
            var data = camera.GetUniversalAdditionalCameraData();
            if (data == null) return;
            if (m_RenderPass != null)
                data.scriptableRenderer.EnqueuePass(m_RenderPass);
        }
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);
        }
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            CreatePass();
            material = new Material(Shader.Find("URP Shader/Blurred Image"));
            blurSize = 2;
        }
#endif
        void CreatePass()
        {
            if (m_RenderPass == null)
            {
                m_RenderPass = new BlurredImageRenderPass();
                m_RenderPass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
            }
        }
    }
    //-----------------------------------------------------------------Image-----------------------------------------------------------------//

    //------------------------------------------------------------------Pass------------------------------------------------------------------//
    public class BlurredImageRenderPass : ScriptableRenderPass
    {
        const float renderScale = 0.3f;
        Material m_BlurredImageMaterial;
        Material BlurredImageMaterial
        {
            get
            {
                if (m_BlurredImageMaterial == null)
                    m_BlurredImageMaterial = new Material(Shader.Find("URP Shader/Blur Texture"));
                return m_BlurredImageMaterial;
            }
        }
        public float BlurSize
        {
            set => BlurredImageMaterial.SetFloat("_Size", value);
        }
        int renderTextureWidth => Mathf.NextPowerOfTwo((int)(Screen.width * renderScale));
        int renderTextureHeight => Mathf.NextPowerOfTwo((int)(Screen.height * renderScale));
        RTHandle m_BlurredImageRTHandle;
        RTHandle BlurredImageRTHandle
        {
            get
            {
                if (m_BlurredImageRTHandle == null)
                    m_BlurredImageRTHandle = RTHandles.Alloc(renderTextureWidth, renderTextureHeight, filterMode: FilterMode.Bilinear);
                return m_BlurredImageRTHandle;
            }
        }
        RTHandle m_BlurredImageRTHandleTemp;
        RTHandle BlurredImageRTHandleTemp
        {
            get
            {
                if (m_BlurredImageRTHandleTemp == null)
                    m_BlurredImageRTHandleTemp = RTHandles.Alloc(renderTextureWidth, renderTextureHeight, filterMode: FilterMode.Bilinear);
                return m_BlurredImageRTHandleTemp;
            }
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
            ConfigureInput(ScriptableRenderPassInput.Color);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (renderingData.cameraData.cameraType == CameraType.SceneView
                || renderingData.cameraData.cameraType == CameraType.Preview)
                return;
#endif

            var source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            CommandBuffer cmd = CommandBufferPool.Get("Blurred Image Pass");
            cmd.Clear();

            Blit(cmd, source, BlurredImageRTHandle);
            Blit(cmd, BlurredImageRTHandle, BlurredImageRTHandleTemp, BlurredImageMaterial, 0);
            Blit(cmd, BlurredImageRTHandleTemp, BlurredImageRTHandle, BlurredImageMaterial, 1);

            cmd.SetGlobalTexture("_BlurredImageRT", BlurredImageRTHandle);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }
        public void ReleaseRT()
        {
            if (m_BlurredImageRTHandle != null)
            {
                m_BlurredImageRTHandle.Release();
                m_BlurredImageRTHandle = null;
            }
            if (m_BlurredImageRTHandleTemp != null)
            {
                m_BlurredImageRTHandleTemp.Release();
                m_BlurredImageRTHandleTemp = null;
            }
        }
    }
    //------------------------------------------------------------------Pass------------------------------------------------------------------//
}