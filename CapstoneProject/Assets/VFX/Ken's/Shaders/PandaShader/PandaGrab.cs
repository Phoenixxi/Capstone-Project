using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PandaGrab : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {

        static string rt_name="_PandaGrabTex";
        private RTHandle rt_Handle;
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner

        // Constructor to initialize the handle (call this when creating the pass instance)
        public CustomRenderPass()
        {
            rt_Handle = RTHandles.Alloc(Vector2.one, name: rt_name);  // Initial alloc; will be resized later
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Get descriptor from camera for dynamic resolution matching
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;  // No depth needed for color grab
            descriptor.colorFormat = RenderTextureFormat.DefaultHDR;  // Match your needs

            // Reallocate if needed (handles creation/resizing/reuse)
            RenderingUtils.ReAllocateIfNeeded(ref rt_Handle, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: rt_name);

            ConfigureTarget(rt_Handle);
            ConfigureClear(ClearFlag.Color, Color.black);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("PandaPass");
            cmd.Blit(renderingData.cameraData.renderer.cameraColorTargetHandle, rt_Handle);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);  // Use pool release instead of cmd.Release()
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // No per-frame release needed; handle is reused. Release in Dispose if required.
        }

        // Optional: Add a method to dispose when the feature is disabled
        public void Dispose()
        {
            rt_Handle?.Release();
        }
    }

    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


