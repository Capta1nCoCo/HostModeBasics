using UnityEngine;
using Fusion;

public class PlayerColor : NetworkBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Color _changedColor = Color.white;
    [Networked] private bool spawnedProjectile { get; set; }

    private Color _startingColor;
    private ChangeDetector _changeDetector;
    private Material _material;

    private void Awake()
    {
        _material = _meshRenderer.material;
        _startingColor = _material.color;
    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(spawnedProjectile):
                    _material.color = _changedColor;
                    break;
            }
        }
        _material.color = Color.Lerp(_material.color, _startingColor, Time.deltaTime);
    }

    public void TriggerColorChange()
    {
        spawnedProjectile = !spawnedProjectile;
    }
}