using System.Diagnostics;
using Godot;

public partial class ValuableCollector : Area3D
{
	[Export] public int CollectedCoins;
	[Signal] public delegate void ValuableCollectedEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		AreaEntered -= OnAreaEntered;
	}

	private void OnAreaEntered(Area3D area) => TryCollect(area);

	private void TryCollect(Node3D node)
	{
		if (node is Valuable valuable)
		{
			CollectedCoins += (int)valuable.Value;
			valuable.OnCollected();
			EmitSignal(SignalName.ValuableCollected);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
