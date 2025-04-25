using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/* 参考サイト
 * https://xrdnk.hateblo.jp/entry/urp_renderer_feature_fade
 * https://soramamenatan.hatenablog.com/entry/2022/03/27/132040
 */
public class Grayscale : ScriptableRendererFeature
{
    [SerializeField]
    public Material GrayscaleMaterial;

    public static Grayscale Instance { get; private set; }
    public Grayscale()
    {
        if (Instance == null)
        {
            Instance = this;
        } 
    }
    
    [System.Serializable]
    public class GrayscaleSetting
    {
        // レンダリングの実行タイミング
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    /// <summary>
    /// Grayscale実行Pass
    /// </summary>
    public class GrayScalePass : ScriptableRenderPass
    {
        private readonly string profilerTag = "GrayScale Pass";

        public Material RuntimeGrayscaleMaterial; // グレースケール計算用マテリアル

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;

            // コマンドバッファ
            var cmd = CommandBufferPool.Get(profilerTag);

            // マテリアル実行
            cmd.Blit(cameraColorTarget, cameraColorTarget, RuntimeGrayscaleMaterial);

            context.ExecuteCommandBuffer(cmd);
        }
    }

    [SerializeField] private GrayscaleSetting settings = new GrayscaleSetting();
    private GrayScalePass scriptablePass;

    public override void Create()
    {
        scriptablePass = new GrayScalePass();
        scriptablePass.RuntimeGrayscaleMaterial = GrayscaleMaterial;
        scriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (scriptablePass != null && scriptablePass.RuntimeGrayscaleMaterial != null)
        {
            renderer.EnqueuePass(scriptablePass);
        }
    }
}