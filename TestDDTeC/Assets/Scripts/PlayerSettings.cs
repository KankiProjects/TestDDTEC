using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Player/Settings")]
public class PlayerSettings : ScriptableObject
{
    [SerializeField, Min(0f)] private float jumpForce = 5f;
    [SerializeField, Min(0f)] private float maxChargeBonus = 8f;
    [SerializeField, Min(0.01f)] private float chargeTime = 1f;
    [SerializeField, Range(0.5f, 1f)] private float squashScaleY = 0.75f;
    [SerializeField, Min(0f)] private float airControl = 8f;
    [SerializeField, Min(0f)] private float maxAirSpeed = 8f;

    public float JumpForce => jumpForce;
    public float MaxChargeBonus => maxChargeBonus;
    public float ChargeTime => chargeTime;
    public float SquashScaleY => squashScaleY;
    public float AirControl => airControl;
    public float MaxAirSpeed => maxAirSpeed;
}
