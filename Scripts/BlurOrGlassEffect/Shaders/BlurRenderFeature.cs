using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRenderFeature : ScriptableRendererFeature
{
	[System.Serializable]
	public class BlurSettings
	{
		public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRendering;
		public Material blurMaterial = null;

		[Range(2, 15)]
		public int blurPasses = 6;

		[Range(1, 4)]
		public int downsample = 2;
		public bool copyToFramebuffer;
		public string targetName = "_blurTexture";
	}

	public BlurSettings settings = new BlurSettings();

	class CustomRenderPass : ScriptableRenderPass
	{
		public Material blurMaterial;
		public int passes;
		public int downsample;
		public bool copyToFramebuffer;
		public string targetName;
		string profilerTag;

		int tmpId1;
		int tmpId2;

		RenderTargetIdentifier tmpRT1;
		RenderTargetIdentifier tmpRT2;

		private RenderTargetIdentifier source { get; set; }

		public void Setup(RenderTargetIdentifier source)
		{
			this.source = source;
		}

		public CustomRenderPass(string profilerTag)
		{
			this.profilerTag = profilerTag;
		}

		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			var width = cameraTextureDescriptor.width / downsample;
			var height = cameraTextureDescriptor.height / downsample;

			tmpId1 = Shader.PropertyToID("tmpBlurRT1");
			tmpId2 = Shader.PropertyToID("tmpBlurRT2");
			cmd.GetTemporaryRT(tmpId1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
			cmd.GetTemporaryRT(tmpId2, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);

			tmpRT1 = new RenderTargetIdentifier(tmpId1);
			tmpRT2 = new RenderTargetIdentifier(tmpId2);

			ConfigureTarget(tmpRT1);
			ConfigureTarget(tmpRT2);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

			RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
			opaqueDesc.depthBufferBits = 0;

			// first pass
			cmd.SetGlobalFloat("_offset", 1.5f);
			cmd.Blit(source, tmpRT1, blurMaterial);

			for (var i = 1; i < passes - 1; i++)
			{
				cmd.SetGlobalFloat("_offset", 0.5f + i);
				cmd.Blit(tmpRT1, tmpRT2, blurMaterial);

				// pingpong
				var rttmp = tmpRT1;
				tmpRT1 = tmpRT2;
				tmpRT2 = rttmp;
			}

			// final pass
			cmd.SetGlobalFloat("_offset", 0.5f + passes - 1f);
			if (copyToFramebuffer)
			{
				cmd.Blit(tmpRT1, source, blurMaterial);
			}
			else
			{
				cmd.Blit(tmpRT1, tmpRT2, blurMaterial);
				cmd.SetGlobalTexture(targetName, tmpRT2);
			}

			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();

			CommandBufferPool.Release(cmd);
		}

		public override void FrameCleanup(CommandBuffer cmd)
		{ }
	}

	CustomRenderPass scriptablePass;

	public override void Create()
	{
		scriptablePass = new CustomRenderPass("BlurPass")
		{
			blurMaterial = settings.blurMaterial,
			passes = settings.blurPasses,
			downsample = settings.downsample,
			copyToFramebuffer = settings.copyToFramebuffer,
			targetName = settings.targetName,
			renderPassEvent = settings.renderPassEvent
		};
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		var src = renderer.cameraColorTarget;
		scriptablePass.Setup(src);
		renderer.EnqueuePass(scriptablePass);
	}
}


