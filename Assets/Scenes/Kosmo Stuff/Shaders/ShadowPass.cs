using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShadowPass : ScriptableRenderPass
{
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("My Shadow Pass");

        // Przykład: ustawienie shadow mapy jako globalna tekstura
        cmd.SetGlobalTexture("_MyShadowMap", new RenderTargetIdentifier("_MainLightShadowmapTexture"));

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}