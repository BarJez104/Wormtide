using Godot;

namespace Wormtide.scripts;

public partial class Coin : Node3D
{
	[Export] public float RotationSpeed = 0.5f;
	
	public override void _Process(double delta)
	{
		RotateY(RotationSpeed * (float)delta);
	}
}
