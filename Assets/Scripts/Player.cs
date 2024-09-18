using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float _movementSpeed = 5.0f;
    [SerializeField] private float _spawnDelayInSeconds = 0.5f;
    [SerializeField] private Ball _prefabBall;

    [Networked] private TickTimer _delay { get; set; }

    private Vector3 _forward;
    private NetworkCharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        _forward = Vector3.forward;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(_movementSpeed * data.direction * Runner.DeltaTime);
            ChangeForwardDirection(data);
            SpawnBallForward(data);
        }
    }

    private void ChangeForwardDirection(NetworkInputData data)
    {
        if (data.direction.sqrMagnitude > 0)
            _forward = data.direction;
    }

    private void SpawnBallForward(NetworkInputData data)
    {
        if (HasStateAuthority && _delay.ExpiredOrNotRunning(Runner))
        {
            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            {
                _delay = TickTimer.CreateFromSeconds(Runner, _spawnDelayInSeconds);
                Runner.Spawn(_prefabBall,
                    transform.position + _forward, Quaternion.LookRotation(_forward),
                    Object.InputAuthority, (runner, o) =>
                    {
                        // Initialize the Ball before synchronizing it
                        o.GetComponent<Ball>().Init();
                    });
            }
        }
    }
}