using UnityEngine;
using Fusion;

public class KinematicBall : Ball
{
    public override void Init()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, _lifeTimeInSeconds);
    }

    public override void FixedUpdateNetwork()
    {
        if (lifeTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            transform.position += _velocity * transform.forward * Runner.DeltaTime;
        }
    }
}