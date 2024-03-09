using System;

namespace Enemies
{
    public abstract class EnemyBase : CharacterBase
    {
        public abstract event Action<EnemyBase> OnEndDie;
    
        public abstract EnemyID EnemyID { get; }

        public abstract void HandleUpdate(float deltaTime);
    
        public abstract void HandleFixedUpdate(float fixedDeltaTime);
    }
}