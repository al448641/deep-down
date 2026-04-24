using System.Security;
using System.Xml.Schema;
using UnityEngine;

public class Bubbles : MonoBehaviour
{
    [SerializeField] private ParticleSystem bubbles;
    [SerializeField] private float normalStrenght;
    [SerializeField] private float chargeJumpStrenght;
    private ParticleSystem.EmissionModule particleEmision;


    void Start()
    {
        particleEmision = bubbles.emission;
        Player.JumpIsCharging += ChargingBubbles;
    }

    private void onDestroy()
    {
        Player.JumpIsCharging -= ChargingBubbles;

    }

    private  void ChargingBubbles(float charge)
    {
        particleEmision.rateOverTime = Mathf.Lerp(normalStrenght,chargeJumpStrenght,charge);
    }
}
