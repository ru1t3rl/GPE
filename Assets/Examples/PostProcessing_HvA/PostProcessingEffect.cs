// https://www.alanzucconi.com/2015/07/08/screen-shaders-and-postprocessing-effects-in-unity3d/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostProcessingEffect : MonoBehaviour {

    public Material material;

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if(material == null)
            return;

        material.SetFloat("_Width", source.width);
		material.SetFloat("_Height", source.height);

		//apply our material to the ouput
        Graphics.Blit(source, destination, material);
    }
}
