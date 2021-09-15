// https://www.alanzucconi.com/2015/07/08/screen-shaders-and-postprocessing-effects-in-unity3d/
//http://catlikecoding.com/unity/tutorials/advanced-rendering/bloom/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostProcessingEffect_MultiPass : MonoBehaviour {

    public Material material;
	[Range(1, 64)]
	public int iterations = 1;

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {

		int width = source.width;
		int height = source.height;
		RenderTextureFormat format = source.format;

		//for the UVs in the shader
		material.SetFloat("_Width", width);
		material.SetFloat("_Height", height);

		RenderTexture currentDestination = RenderTexture.GetTemporary (width, height, 0, format);

		Graphics.Blit (source, currentDestination);
		RenderTexture currentSource = currentDestination;

		for (int i = 0; i < iterations; i++) {
			currentDestination = RenderTexture.GetTemporary (width, height, 0, format);
			Graphics.Blit (currentSource, currentDestination, material);
			RenderTexture.ReleaseTemporary (currentSource);
			currentSource = currentDestination;
		}

		Graphics.Blit (currentSource, destination);
	}
}
