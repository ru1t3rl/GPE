using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VRolijk.DayNight.Module
{
    public class SkyboxModule : DayNightModuleBase
    {
        [SerializeField] UnityEngine.Gradient skyColor, groundColor;        

        public override void UpdateModule(float intensity)
        {
            RenderSettings.skybox.SetColor("_SkyTint", skyColor.Evaluate(intensity));
            RenderSettings.skybox.SetColor("_GroundColor", groundColor.Evaluate(intensity));
        }
    }
}
