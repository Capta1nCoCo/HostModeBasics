using UnityEngine;
using Fusion;

public abstract class Ball : NetworkBehaviour
{
    [SerializeField] protected float _velocity = 5.0f;
    [SerializeField] protected float _lifeTimeInSeconds = 5.0f;
    [Networked] protected TickTimer lifeTimer {  get; set; }
    public abstract void Init();
}
