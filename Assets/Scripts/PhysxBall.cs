using UnityEngine;
using Fusion;

public class PhysxBall : Ball
{
    public override void Init()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, _lifeTimeInSeconds);
        GetComponent<Rigidbody>().velocity = _velocity * transform.forward;
    }

    public override void FixedUpdateNetwork()
    {
        if (lifeTimer.Expired(Runner))
            Runner.Despawn(Object);
    }
}