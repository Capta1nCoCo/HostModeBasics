using UnityEngine;
using Fusion;

public class PhysxBall : NetworkBehaviour
{
    [SerializeField] private float _lifeTimeInSeconds = 5.0f;

    [Networked] private TickTimer _life { get; set; }

    public void Init(Vector3 forward)
    {
        _life = TickTimer.CreateFromSeconds(Runner, _lifeTimeInSeconds);
        GetComponent<Rigidbody>().velocity = forward;
    }

    public override void FixedUpdateNetwork()
    {
        if (_life.Expired(Runner))
            Runner.Despawn(Object);
    }
}