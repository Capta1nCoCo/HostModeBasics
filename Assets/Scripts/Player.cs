using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    private NetworkCharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(_movementSpeed * data.direction * Runner.DeltaTime);
        }
    }
}