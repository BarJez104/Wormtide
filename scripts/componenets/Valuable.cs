using Godot;
using System;

public partial class Valuable : Area3D
{
	[Export] public float Value = 0;

	[Signal]
	public delegate void CollectedEventHandler();

	public void OnCollected()
	{
		EmitSignal(SignalName.Collected);
	}
}
