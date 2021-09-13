using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRolijk.DayNight.Module
{
    public class SunModule : DayNightModuleBase
    {
        [SerializeField] Light sun;
        [SerializeField] UnityEngine.Gradient sunColor;
        [SerializeField] float baseIntensity, intensityVariation;
        [SerializeField] float minActiveIntensity;
        [SerializeField] float atmoshpereLerpSpeed = 0.1f;
        [SerializeField, Range(0f, 5f)] float atmosphereThickness;
        [SerializeField, Range(0f, 1f)] float diskSize = 0.04f;
        [SerializeField, Range(0f, 10f)] float convergence = 5f;

        public override void UpdateModule(float intensity)
        {
            sun.color = sunColor.Evaluate(intensity);
            sun.intensity = minActiveIntensity + (baseIntensity + intensity * intensityVariation) * intensity;

            if (intensity >= minActiveIntensity)
            {
                sun.gameObject.SetActive(true);
                RenderSettings.sun = sun;

                RenderSettings.skybox.SetFloat("_AtmosphereThickness", Mathf.Lerp(RenderSettings.skybox.GetFloat("_AtmosphereThickness"), atmosphereThickness, atmoshpereLerpSpeed));                
                RenderSettings.skybox.SetFloat("_SunSize", diskSize);
                RenderSettings.skybox.SetFloat("_SunSizeConvergence", convergence);
            }
            else if (sun.gameObject.activeSelf)
            {
                sun.gameObject.SetActive(false);
            }
        }
    }
}
