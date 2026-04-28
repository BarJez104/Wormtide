using Godot;
using Wormtide;

namespace Wormtide.scripts;

public partial class PlayerBody3D : CharacterBody3D
{
	[Export] public float Speed = 5.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public ValuableCollector ValuableCollector;
	[Export] public Label CoinsLabel;

	public override void _Ready()
	{
		ValuableCollector.ValuableCollected += OnValuableCollected;
		CoinsLabel.Text = "Coins: 0";
	}

	private void OnValuableCollected()
	{
		CoinsLabel.Text = $"Coins: {ValuableCollector.CollectedCoins}";
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed(GameActions.Jump) && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		Vector2 inputDir = Input.GetVector(GameActions.MoveLeft, GameActions.MoveRight, GameActions.MoveForward, GameActions.MoveBack);
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
