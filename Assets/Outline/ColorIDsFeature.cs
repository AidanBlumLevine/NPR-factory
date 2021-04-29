using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorIDsFeature : ScriptableRendererFeature
{
    class IDColorsPass : ScriptableRenderPass
    {
        private RenderTargetHandle depthAttachmentHandle { get; set; }
        internal RenderTextureDescriptor descriptor { get; private set; }

        private Material mat = null;
        private FilteringSettings m_FilteringSettings;
        string m_ProfilerTag = "idColors Prepass";
        ShaderTagId m_ShaderTagId = new ShaderTagId("SRPDefaultUnlit");

        public IDColorsPass(RenderQueueRange renderQueueRange, LayerMask layerMask, Material material)
        {
            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            mat = material;
        }

        public void Setup(RenderTextureDescriptor baseDescriptor, RenderTargetHandle depthAttachmentHandle)
        {
            this.depthAttachmentHandle = depthAttachmentHandle;
            baseDescriptor.colorFormat = RenderTextureFormat.ARGB32;
            descriptor = baseDescriptor;
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in an performance manner.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(depthAttachmentHandle.id, descriptor, FilterMode.Point);
            ConfigureTarget(depthAttachmentHandle.Identifier());
            ConfigureClear(ClearFlag.All, Color.white);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
            var drawSettings = CreateDrawingSettings(m_ShaderTagId, ref renderingData, sortFlags);
            drawSettings.perObjectData = PerObjectData.None;

            drawSettings.overrideMaterial = mat;

            context.DrawRenderers(renderingData.cullResults, ref drawSettings,
                ref m_FilteringSettings);

            cmd.SetGlobalTexture("_CameraIDColorsTexture", depthAttachmentHandle.id);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        CullingResults Cull(Camera camera, ScriptableRenderContext context)
        {
            if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
            {
                return context.Cull(ref p);
            }
            throw new System.Exception("culling parameters couldnt be fetched");
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (depthAttachmentHandle != RenderTargetHandle.CameraTarget)
            {
                cmd.ReleaseTemporaryRT(depthAttachmentHandle.id);
                depthAttachmentHandle = RenderTargetHandle.CameraTarget;
            }
        }
    }


    IDColorsPass idColorsPass;
    RenderTargetHandle idColorsTexture;

    [System.Serializable]
    public class IdColorsSettings
    {
        public Material idColorsMaterial = null;
        public RenderPassEvent rpe = RenderPassEvent.BeforeRenderingTransparents;
    }

    public IdColorsSettings settings = new IdColorsSettings();

    public override void Create()
    {
        idColorsPass = new IDColorsPass(RenderQueueRange.opaque, -1, settings.idColorsMaterial);
        idColorsPass.renderPassEvent = settings.rpe;
        idColorsTexture.Init("_CameraIDColorsTexture");
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.idColorsMaterial == null)
        {
            Debug.LogWarningFormat("Missing id colors Material");
            return;
        }
        idColorsPass.Setup(renderingData.cameraData.cameraTargetDescriptor, idColorsTexture);
        renderer.EnqueuePass(idColorsPass);
    }
}
