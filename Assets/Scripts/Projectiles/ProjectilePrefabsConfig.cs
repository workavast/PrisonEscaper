using Core;
using UnityEngine;

namespace Projectiles
{
    [CreateAssetMenu(fileName = nameof(ProjectilePrefabsConfig), menuName = "Configs/" + nameof(ProjectilePrefabsConfig))]
    public class ProjectilePrefabsConfig : PrefabsConfigBase<ProjectileId, ProjectileBase>
    {
        
    }
}