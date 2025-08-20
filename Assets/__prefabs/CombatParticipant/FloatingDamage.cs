using UnityEngine;
using TMPro; // Make sure to add this for TextMeshProUGUI

/// <summary>
/// A MonoBehaviour script to handle floating damage text animations.
/// It makes a TextMeshProUGUI element float upwards while fading out.
/// </summary>
public class FloatingDamage : MonoBehaviour
{
    // The TextMeshProUGUI component that will display the damage value.
    // Assign this in the Inspector.
    [Tooltip("The TextMeshProUGUI component to animate.")]
    public TextMeshProUGUI damageText;

    // The speed at which the text floats upwards.
    [Tooltip("The speed at which the text floats upwards.")]
    public float floatSpeed = 1f;

    // The total time the animation will run.
    [Tooltip("The total duration of the float and fade animation.")]
    public float floatTime = 1f;

    // Internal variable to keep track of the remaining time for the animation.
    private float timeRemaining;

    // The starting position of the text, used to reset its position for each animation.
    private Vector3 initialPosition;

    /// <summary>
    /// Initializes the component.
    /// </summary>
    private void Awake()
    {
        if (damageText == null)
        {
            damageText = GetComponent<TextMeshProUGUI>();
        }

        // Store the initial state to reset for each animation.
        initialPosition = transform.localPosition;
        
        // Hide the text initially.
        damageText.enabled = false;
    }

    /// <summary>
    /// Starts the floating and fading animation.
    /// This method should be called externally (e.g., when an enemy takes damage).
    /// </summary>
    [ContextMenu("StartFloat()")]
    public void StartFloat(int damage, bool isCritical, Color color)
    {
        // Reset the text position, color, and enable the component.
        damageText.SetText(damage.ToString());
        transform.localPosition = initialPosition;
        damageText.color = color;
        damageText.enabled = true;
        
        // Set the timer for the animation.
        timeRemaining = floatTime;
    }
    
    /// <summary>
    /// This Update method handles the animation over time.
    /// </summary>
    private void Update()
    {
        // Only run the animation logic if the text is currently enabled.
        if (damageText.enabled)
        {
            // Calculate the time delta.
            float deltaTime = Time.deltaTime;

            // Move the text up.
            transform.Translate(Vector3.up * floatSpeed * deltaTime);
            
            // Calculate the new alpha value for the fading effect.
            // The alpha fades from 1 to 0 over the duration of the animation.
            float newAlpha = Mathf.Lerp(0f, 1f, timeRemaining / floatTime);
            
            // Set the new color with the calculated alpha.
            Color newColor = damageText.color;
            newColor.a = newAlpha;
            damageText.color = newColor;

            // Decrease the time remaining.
            timeRemaining -= deltaTime;

            // If the animation is complete, disable the text.
            if (timeRemaining <= 0)
            {
                damageText.enabled = false;
            }
        }
    }
}
