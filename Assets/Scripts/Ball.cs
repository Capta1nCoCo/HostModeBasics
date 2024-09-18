using UnityEngine;
using Fusion;

public class Ball : NetworkBehaviour
{
    [SerializeField] private float _velocity = 5.0f;
    [SerializeField] private float _lifeTimeInSeconds = 5.0f;

    [Networked] private TickTimer _life { get; set; }

    public void Init()
    {
        _life = TickTimer.CreateFromSeconds(Runner, _lifeTimeInSeconds);
    }

    public override void FixedUpdateNetwork()
    {
        if (_life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            transform.position += _velocity * transform.forward * Runner.DeltaTime;
        }
    }
}
