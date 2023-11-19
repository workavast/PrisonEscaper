using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeProfileController : MonoBehaviour
{
    [SerializeField] private VolumeProfile profile;

    private void Awake() => ToggleDepthOfField(false);

    public void ToggleDepthOfField(bool enable)
    {
        if (profile.TryGet<DepthOfField>(out var dof))
        {
            if(enable && dof.active) return;
            if(!enable && !dof.active) return;

            dof.active = enable;
        }
        else
        {
            throw new Exception("Depth Of Field dont present in volume profile");
        }
    }
}
