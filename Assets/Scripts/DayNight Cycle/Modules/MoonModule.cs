using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRolijk.DayNight.Module
{
    public class MoonModule : DayNightModuleBase
    {
        [SerializeField] Light moon;
        [SerializeField] UnityEngine.Gradient moonColor;
        [SerializeField] float baseIntensity, intensityVariation;
        [SerializeField] float minActiveIntensity;
        [SerializeField] float atmoshpereLerpSpeed = 0.0f;
        [SerializeField, Range(0f, 5f)] float atmosphereThickness;
        [SerializeField, Range(0f, 1f)] float diskSize = 0.04f;
        [SerializeField, Range(0f, 10f)] float convergence = 5f;

        public override void UpdateModule(float intensity)
        {
            moon.color = moonColor.Evaluate(1 - intensity);
            moon.intensity = baseIntensity + intensity * intensityVariation;

            if (1 - intensity >= minActiveIntensity)
            {
                moon.gameObject.SetActive(true);
                RenderSettings.sun = moon;

                try
                {
                    RenderSettings.skybox.SetFloat("_AtmosphereThickness", Mathf.Lerp(RenderSettings.skybox.GetFloat("_AtmosphereThickness"), atmosphereThickness, atmoshpereLerpSpeed));
                    RenderSettings.skybox.SetFloat("_SunSize", diskSize);
                    RenderSettings.skybox.SetFloat("_SunSizeConvergence", convergence);
                }
                catch (NullReferenceException) { };
            }
            else if (moon.gameObject.activeSelf)
            {
                moon.gameObject.SetActive(false);
            }
        }
    }
}
