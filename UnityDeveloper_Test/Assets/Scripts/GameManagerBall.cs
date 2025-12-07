using UnityEngine;
using UnityEngine.UI;

public class BowlingGameManager : MonoBehaviour
{
    [Header("References")]
    public BowlingSystem bowler;
    public Slider skillSlider; 
    public Toggle fastBowlingToggle; 

    [Header("Throw")]
    public SwingType swingDirection = SwingType.Outswing;
    public SpinType spinDirection = SpinType.LegSpin;

    public float sliderSpeed = 2f;
    public float maxBallSpeed = 28f;
    
    public float maxSwingForce = 15f; 
    public float maxSpinForce = 8f;

    public enum SwingType { Outswing, Inswing }
    public enum SpinType { LegSpin, OffSpin }

    private bool isBowling = false;

    void Start()
    {

    }

    void Update()
    {
        
        if (!isBowling && skillSlider != null)
        {
            float val = Mathf.PingPong(Time.time * sliderSpeed, 2f) - 1f;
            skillSlider.value = val;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CalculateAndThrow();
            }
        }
    }

    void CalculateAndThrow()
    {
        isBowling = true;
        
        float stopPosition = skillSlider.value; 
        float accuracy = 1f - Mathf.Abs(stopPosition);
        
        float finalSwing = 0f;
        float finalSpin = 0f;
        
        if (fastBowlingToggle.isOn)
        {
            
            float directionMultiplier = (swingDirection == SwingType.Inswing) ? 1f : -1f;
            
            finalSwing = directionMultiplier * maxSwingForce * accuracy;
        }
        else
        {
            
            float directionMultiplier = (spinDirection == SpinType.LegSpin) ? 1f : -1f;
            finalSpin = directionMultiplier * maxSpinForce * accuracy;
        }

        bowler.ThrowBall(maxBallSpeed, finalSwing, finalSpin);
    }

    public void OnResetClicked()
    {
        isBowling = false;
        bowler.ResetSystem();
    }
}