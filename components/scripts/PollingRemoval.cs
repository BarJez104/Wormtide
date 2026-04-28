using Godot;
using Wormtide.components.scripts.interfaces;

namespace Wormtide.components.scripts;

public partial class PollingRemoval : Removal
{
	[Export] public float PullSpeed = 10.0f;
	
	[Export] public Node3D? TargetOfDestruction { get; set; }

	private Vector3 _initialScale;
	private float _initialDistance;
	private Node3D? _collector;
	private bool Active => _collector is not null; 
	
	public override void _Ready()
	{
		TargetOfDestruction ??= GetParent<Node3D>();
	}

	public override void Begin(Node3D collector)
	{
		_collector = collector;
		_initialScale = TargetOfDestruction!.Scale;
		_initialDistance = TargetOfDestruction.GlobalPosition.DistanceTo(_collector.GlobalPosition);
	}

	public override void _Process(double delta)
	{
		if (!Active)
			return;

		var dist = TargetOfDestruction!.GlobalPosition.DistanceTo(_collector!.GlobalPosition);
		
		if (dist < 0.05f)
		{
			TargetOfDestruction.QueueFree();
			return;
		}

		TargetOfDestruction.GlobalPosition = TargetOfDestruction.GlobalPosition.MoveToward(_collector.GlobalPosition, PullSpeed * (float)delta);

		var progress = _initialDistance > 0f ? 1f - Mathf.Clamp(dist / _initialDistance, 0f, 1f) : 1f;
		
		TargetOfDestruction.Scale = _initialScale.Lerp(_initialScale * 0.5f, progress);
	}
}