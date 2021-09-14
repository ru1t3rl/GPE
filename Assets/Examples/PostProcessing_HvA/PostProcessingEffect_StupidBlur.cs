// https://www.alanzucconi.com/2015/07/08/screen-shaders-and-postprocessing-effects-in-unity3d/
//http://catlikecoding.com/unity/tutorials/advanced-rendering/bloom/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostProcessingEffect_StupidBlur : MonoBehaviour {
	
	[Range(1, 16)]
	public int iterations = 1;

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        //apply our material to the ouput
		int width = source.width / 2;
		int height = source.height / 2;
		RenderTextureFormat format = source.format;

		RenderTexture currentDestination = RenderTexture.GetTemporary(width, height, 0, format);

		Graphics.Blit(source, currentDestination);
		RenderTexture currentSource = currentDestination;

		for (int i = 1; i < iterations; i++) {
			//sample half the resolution every iteration
			width /= 2;
			height /= 2;			

			if (height < 2) {
				break;
			}

			currentDestination = RenderTexture.GetTemporary(width, height, 0, format);
			Graphics.Blit(currentSource, currentDestination);
			RenderTexture.ReleaseTemporary(currentSource);
			currentSource = currentDestination;
		}

		Graphics.Blit(currentSource, destination);
    }
}
