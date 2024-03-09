using Core;
using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = nameof(EnemyPrefabsConfig), menuName = "Configs/Enemies/" + nameof(EnemyPrefabsConfig))]
    public class EnemyPrefabsConfig : PrefabConfigBase<EnemyID, EnemyBase>
    {

    }
}