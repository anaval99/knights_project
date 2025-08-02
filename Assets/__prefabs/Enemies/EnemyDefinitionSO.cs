using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDefinitionSO", menuName = "Scriptable Objects/EnemyDefinitionSO")]
public class EnemyDefinitionSO : ScriptableObject
{
    public AnimationClip IdleAnimation;
    public AnimationClip OnHitAnimation;
    public AnimationClip DeathAnimation;
}
