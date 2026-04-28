using Godot;
using Wormtide.components.scripts.interfaces;

public partial class Valuable : Area3D
{
    [Export] public float Value = 0;
    [Export] public Removal? RemovalType { get; set; }

    public bool IsCollected;

    [Signal]
    public delegate void CollectedEventHandler();

    public void OnCollected(Node3D collector)
    {
        if (IsCollected)
            return;

        IsCollected = true;

        if (RemovalType != null)
            RemovalType.Begin(collector);
        else
            QueueFree();

        EmitSignal(SignalName.Collected);
    }
}