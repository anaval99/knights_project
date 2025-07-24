using UnityEngine;

public class UnderGlow : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem underGlowParticleSystem;
    [SerializeField]
    private Color underGlowColor = Color.blue;
    [SerializeField]
    private float size = 6f;


    void OnValidate()
    {
        var mainModule = this.underGlowParticleSystem.main;
        var startColor = mainModule.startColor;
        startColor.color = underGlowColor;
        mainModule.startColor = startColor;
        mainModule.startSize = size;
    }

}
