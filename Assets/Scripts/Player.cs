using Fusion;
using UnityEngine;

[RequireComponent(typeof(PlayerColor))]
public class Player : NetworkBehaviour
{
    [SerializeField] private float _movementSpeed = 5.0f;
    [SerializeField] private float _spawnDelayInSeconds = 0.5f;
    [SerializeField] private KinematicBall _prefabKinematicBall;
    [SerializeField] private PhysxBall _prefabPhysxBall;

    [Networked] private TickTimer delayTimer { get; set; }

    private Vector3 _forward;
    private NetworkCharacterController _cc;
    private PlayerColor _color;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        _forward = Vector3.forward;
        _color = GetComponent<PlayerColor>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(_movementSpeed * data.direction * Runner.DeltaTime);
            CheckForwardDirection(data);
            ProcessPlayerInput(data);
        }
    }

    private void CheckForwardDirection(NetworkInputData data)
    {
        if (data.direction.sqrMagnitude > 0)
            _forward = data.direction;
    }

    private void ProcessPlayerInput(NetworkInputData data)
    {
        if (HasStateAuthority && delayTimer.ExpiredOrNotRunning(Runner))
        {
            LaunchBallByInput(data);
        }
    }

    private void LaunchBallByInput(NetworkInputData data)
    {
        if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
        {
            SpawnBall(_prefabKinematicBall);
        }
        else if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1))
        {
            SpawnBall(_prefabPhysxBall);
        }
    }

    private void SpawnBall(Ball prefabBall)
    {
        delayTimer = TickTimer.CreateFromSeconds(Runner, _spawnDelayInSeconds);
        Runner.Spawn(prefabBall,
            transform.position + _forward, Quaternion.LookRotation(_forward),
            Object.InputAuthority,
            InitBallBeforeSync());
        _color.TriggerColorChange();
    }

    private NetworkRunner.OnBeforeSpawned InitBallBeforeSync()
    {
        return (runner, obj) =>
        {
            obj.GetComponent<Ball>().Init();
        };
    }
}