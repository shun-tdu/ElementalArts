```mermaid
classDiagram
    class WeaponSystem{
        +void SetLockOnTarget
        +void OnTriggerDown
        +void OnTriggerHold
        +void OnTriggerUp
    }
    
    class WeaponBehavior{
        +void OnTriggerDown
        +void OnTriggerHold
        +void OnTriggerUp
    }
    
    
    namespace Projectile {
        class ProjectileWeaponBehavior{
            +void OnTriggerDown
            +void OnTriggerHold
            +void OnTriggerUp
            #void SpawnProjectile
        }
        
        class BaseProjectile{
            + void Initialize(impact)
            + void SetInitialVelocity(velocity)
        }
        
        class HomingProjectile{
            + void Initialize(impact, homingTarget, homingTurnSpeed)
        }
        
        %%衝突系
        class IProjectileImpact{
            <<interface>>
            +void OnImpact
        }
        
        class ClusterImpact{
            +OnImpact
        }
        
        class SingleHitImpact{
            +OnImpact
        }
        
        %%移動系
        class IProjectileMovement{
            <<interface>>
            +void SetUp(impact, target)
        }
        
        class HomingMovement{
            +void SetUp(impact, target)
            -void FindClosestEnemy
        }
        
        class StraightMovement{
            +void SetUp(impact, target)
        }
    }
    
    class IMagazineWeapon{
        <<interface>>
        +MagazineSize
        +ReloadTime
    }

    WeaponSystem *-- WeaponBehavior 
    ProjectileWeaponBehavior <|-- WeaponBehavior
    ProjectileWeaponBehavior <|..IMagazineWeapon
    
    %% Projectile衝突関連
    ClusterImpact <|.. IProjectileImpact
    SingleHitImpact <|.. IProjectileImpact
    
    %% Projectile移動関連
    HomingMovement <|.. IProjectileMovement
    StraightMovement <|.. IProjectileMovement
    
    %% Core関連
    HomingProjectile <|-- BaseProjectile
    
```