using Fusion;
using System;
using TMPro;
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
    private TMP_Text _messages;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        _forward = Vector3.forward;
        _color = GetComponent<PlayerColor>();
    }

    private void Update()
    {
        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
        {
            RPC_SendMessage("Hello Mate!");
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        RPC_RelayMessage(message, info.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void RPC_RelayMessage(string message, PlayerRef messageSource)
    {
        if (_messages == null)
            _messages = FindObjectOfType<TMP_Text>();

        if (messageSource == Runner.LocalPlayer)
        {
            message = $"You said: {message}\n";
        }
        else
        {
            message = $"Some other player said: {message}\n";
        }

        _messages.text += message;
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