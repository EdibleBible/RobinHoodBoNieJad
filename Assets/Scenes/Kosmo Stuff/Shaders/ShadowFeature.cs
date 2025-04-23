using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class ShadowFeature : ScriptableRendererFeature
{
    ShadowPass shadowPass;

    public override void Create()
    {
        shadowPass = new ShadowPass();
        shadowPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(shadowPass);
    }
}