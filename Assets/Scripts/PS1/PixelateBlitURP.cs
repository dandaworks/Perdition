using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace piqey.PS1
{
	public class PixelateRendererFeature : ScriptableRendererFeature
	{
		[System.Serializable]
		public class Settings
		{
			public RenderPassEvent RenderEvent = RenderPassEvent.AfterRendering;
			/// <remarks>
			/// Some PS1 framebuffer resolutions:
			/// <list type="bullet">
			///   <item>
			///     <term>Silent Hill</term>
			///     <description>319×223 px</description>
			///   </item>
			///   <item>
			///     <term>Spyro</term>
			///     <description>297×217 px</description>
			///   </item>
			///   <item>
			///     <term>Metal Gear Solid</term>
			///     <description>320×200 px</description>
			///   </item>
			/// </list>
			/// </remarks>
			public Vector2Int DownSampleResolution = new(319, 223);
			public Material PixelateMaterial;
		}

		[SerializeField] private Settings _settings = new();
		private PixelateRenderPass _pixelatePass;

		public override void Create()
		{
			if (!_settings.PixelateMaterial)
				return;

			_pixelatePass = new PixelateRenderPass(_settings, _settings.PixelateMaterial)
			{
				renderPassEvent = _settings.RenderEvent
			};
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (_pixelatePass == null)
				return;

			CameraType camType = renderingData.cameraData.cameraType;
			if (camType == CameraType.Game || camType == CameraType.SceneView)
				renderer.EnqueuePass(_pixelatePass);
		}
	}

	public class PixelateRenderPass : ScriptableRenderPass
	{
		private static readonly int PS1ParamsID = Shader.PropertyToID("_PS1_PixelBlitParams");
		// private static readonly int BlitMatTexID = Shader.PropertyToID("_BlitTexture");
		private const string k_DownsamplePassName = "_PS1_Pixelate_Downsample";
		private const string k_UpsamplePassName = "_PS1_Pixelate_Upsample";
		private const string k_DownsamplePassTempTexName = "_PS1_Pixelate_Temp";

		private readonly PixelateRendererFeature.Settings _settings;
		private readonly Material _material;

		public PixelateRenderPass(PixelateRendererFeature.Settings settings, Material material)
		{
			_settings = settings;
			_material = material;
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

			if (resourceData.isActiveTargetBackBuffer)
				return;

			// Source = current camera color texture
			TextureHandle src = resourceData.activeColorTexture;

			// Create a low-res temp texture
			TextureDesc desc = src.GetDescriptor(renderGraph);
			desc.name = k_DownsamplePassTempTexName;
			desc.width = _settings.DownSampleResolution.x;
			desc.height = _settings.DownSampleResolution.y;
			desc.depthBufferBits = 0;
			desc.msaaSamples = MSAASamples.None;
			desc.useMipMap = false;
			desc.autoGenerateMips = false;

			TextureHandle dst = renderGraph.CreateTexture(desc);

			if (!src.IsValid() || !dst.IsValid())
				return;

			// Update material with down-sample parameters
			UpdateMaterialParameters();

			// Down-sample (pass 0)
			RenderGraphUtils.BlitMaterialParameters downParams = new(src, dst, _material, 0);
			renderGraph.AddBlitPass(downParams, k_DownsamplePassName);

			// Upsample + pixelate (pass 1)
			RenderGraphUtils.BlitMaterialParameters upParams = new(dst, src, _material, 1);
			renderGraph.AddBlitPass(upParams, k_UpsamplePassName);
		}

		private void UpdateMaterialParameters()
		{
			// Enable keyword and update the global params on the material
			Shader.EnableKeyword("_PS1_PIXELBLIT");
			Shader.SetGlobalVector(
				PS1ParamsID,
				new Vector4(
					_settings.DownSampleResolution.x,
					_settings.DownSampleResolution.y,
					1.0f + 1.0f / _settings.DownSampleResolution.x,
					1.0f + 1.0f / _settings.DownSampleResolution.y
				)
			);
		}
	}
}
