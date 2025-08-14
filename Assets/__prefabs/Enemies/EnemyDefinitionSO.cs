using System;
using UnityEngine;

[Serializable]
public class GenericEnemyAttack
{
    [SerializeField]
    public string AttackName;
    [SerializeField]
    public AnimationClip Animation;
    [SerializeField]
    public string Description;
    [SerializeField]
    public int Damage;
    [SerializeField]
    public int AP;
    [SerializeField]
    public bool IsMelee = true;
    [SerializeField]
    public float MeleeOffset = 0.1f;
    [SerializeField]
    public ProjectileType ProjectileType = ProjectileType.None;
}

[CreateAssetMenu(fileName = "EnemyDefinitionSO", menuName = "Scriptable Objects/EnemyDefinitionSO")]
public class EnemyDefinitionSO : ScriptableObject
{
    public AnimationClip IdleAnimation;
    public AnimationClip OnHitAnimation;
    public AnimationClip DeathAnimation;
    public AnimationClip DashAnimation;
    public AnimationClip BackAnimation;
    public GenericEnemyAttack[] GenericEnemyAttacks;
}
