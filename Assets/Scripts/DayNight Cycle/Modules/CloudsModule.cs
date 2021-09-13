using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRolijk.DayNight.Module
{
    public class CloudsModule : DayNightModuleBase
    {
        [SerializeField] MeshRenderer cloudsRenderer;
        [SerializeField] UnityEngine.Gradient cloudColor;
        [SerializeField] float emissionStrengthDay = 1f, emissionStrengthNight = .5f;
        private MaterialPropertyBlock mpb;

        private void Awake()
        {
            mpb = new MaterialPropertyBlock();
            cloudsRenderer.GetPropertyBlock(mpb);
        }

        public override void UpdateModule(float intensity)
        {
            mpb.SetColor("Color_03f1c5e416f944fba07ae6c0ab917c0f", cloudColor.Evaluate(intensity));
            mpb.SetFloat("Vector1_c2af3c63dba94c2e8ca23f9113a3e96f", emissionStrengthNight + (emissionStrengthDay - emissionStrengthNight) * (intensity));
            cloudsRenderer.SetPropertyBlock(mpb);
        }
    }
}