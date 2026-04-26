using Godot;

public partial class Coin : Node3D
{
	[Export] public int Coins = 5;
	[Export] public string PLayerName = "robot";
	[Export] public double Hearts = 3.5;
	[Export] public float RotationSpeed = 0.5f;
	
	public override void _Process(double delta)
	{
		RotateY(RotationSpeed * (float)delta);
	}
	
	private void OnValuableCollected()
	{
		
	}
}
