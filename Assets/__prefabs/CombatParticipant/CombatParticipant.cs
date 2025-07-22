using R3;
using UnityEngine;

public class CombatParticipant : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    public int MaxHealth = 100;
    [SerializeField]
    public int CurrentHealth = 100;
    [SerializeField]
    public int Defense = 10;
    [SerializeField]
    public int Damage = 20;
}
