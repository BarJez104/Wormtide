using Godot;
using System.Collections.Generic;
using Wormtide;

namespace Wormtide.components.scripts;

public partial class SpellCaster : Node3D
{
    private const int MaxOrbs = 5;
    private const float OrbitRadius = 1.5f;
    private const float OrbitSpeed = 1.5f;
    private const float OrbitHeight = 0.5f;
    private const float LaunchDuration = 0.5f;
    private const float PhaseAdjustSpeed = 8.0f;

    [Export] public float OrbTiltStep = 10f;

    private static readonly string[] SpellActions =
    {
        UiActions.FireSpell,
        UiActions.WaterSpell,
        UiActions.EarthSpell,
        UiActions.LightningSpell,
        UiActions.ArcaneSpell,
    };

    private static readonly Color[] ElementColors =
    {
        new Color(1.0f, 0.35f, 0.0f),  // Fire
        new Color(0.15f, 0.5f, 1.0f),  // Water
        new Color(0.3f, 0.8f, 0.2f),   // Earth
        new Color(1.0f, 0.95f, 0.1f),  // Lightning
        new Color(0.7f, 0.1f, 1.0f),   // Arcane
    };

    private record struct OrbEntry(MeshInstance3D Mesh, float LaunchProgress, float PhaseOffset, float TargetPhaseOffset, Basis OrbitBasis);

    private readonly List<OrbEntry> _orbs = new();
    private float _angle;

    public override void _Process(double delta)
    {
        for (int i = 0; i < SpellActions.Length; i++)
        {
            if (Input.IsActionJustPressed(SpellActions[i]))
                TryAddOrb(i);
        }

        _angle += OrbitSpeed * (float)delta;
        UpdateOrbPositions((float)delta);
    }

    private void TryAddOrb(int elementIndex)
    {
        if (_orbs.Count >= MaxOrbs)
            return;

        var material = new StandardMaterial3D();
        material.AlbedoColor = ElementColors[elementIndex];
        material.EmissionEnabled = true;
        material.Emission = ElementColors[elementIndex];
        material.EmissionEnergyMultiplier = 3.0f;

        var sphere = new SphereMesh();
        sphere.Radius = 0.12f;
        sphere.Height = 0.24f;

        var orb = new MeshInstance3D();
        orb.Mesh = sphere;
        orb.MaterialOverride = material;
        orb.Position = new Vector3(0, OrbitHeight, 0);
        AddChild(orb);

        // New orb starts at its target phase so only the launch animation plays, not an angular sweep.
        float newStep = Mathf.Tau / (_orbs.Count + 1);
        float newOrbPhase = newStep * _orbs.Count;
        float maxTilt = Mathf.DegToRad(OrbTiltStep);
        var basis = Basis.FromEuler(new Vector3(
            (float)GD.RandRange(-maxTilt, maxTilt),
            (float)GD.RandRange(-maxTilt, maxTilt),
            (float)GD.RandRange(-maxTilt, maxTilt)
        ));
        _orbs.Add(new OrbEntry(orb, 0f, newOrbPhase, newOrbPhase, basis));

        // Redistribute target phases for all orbs including the one just added.
        for (int i = 0; i < _orbs.Count; i++)
            _orbs[i] = _orbs[i] with { TargetPhaseOffset = i * newStep };
    }

    private void UpdateOrbPositions(float delta)
    {
        if (_orbs.Count == 0)
            return;

        float lerpWeight = Mathf.Clamp(PhaseAdjustSpeed * delta, 0f, 1f);

        for (int i = 0; i < _orbs.Count; i++)
        {
            var entry = _orbs[i];

            entry = entry with
            {
                LaunchProgress = Mathf.Min(1f, entry.LaunchProgress + delta / LaunchDuration),
                PhaseOffset = Mathf.LerpAngle(entry.PhaseOffset, entry.TargetPhaseOffset, lerpWeight),
            };
            _orbs[i] = entry;

            float a = _angle + entry.PhaseOffset;
            var basePos = new Vector3(Mathf.Cos(a) * OrbitRadius, 0f, Mathf.Sin(a) * OrbitRadius);
            var orbitPos = entry.OrbitBasis * basePos + new Vector3(0f, OrbitHeight, 0f);

            _orbs[i].Mesh.Position = Vector3.Zero.Lerp(orbitPos, EaseOut(entry.LaunchProgress));
        }
    }

    private static float EaseOut(float t) => 1f - Mathf.Pow(1f - t, 3f);
}
