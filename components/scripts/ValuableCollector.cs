using Godot;

public partial class ValuableCollector : Area3D
{
	[Export] public float CollectedCoins;
	[Signal] public delegate void ValuableCollectedEventHandler();

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area3D area) => TryCollect(area);

	private void TryCollect(Area3D node)
	{
		if (node is Valuable valuable)
		{
			CollectedCoins += valuable.Value;
			valuable.OnCollected(this);
			EmitSignal(SignalName.ValuableCollected);
		}
	}
}
