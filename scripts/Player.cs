using Godot;

public partial class Player : CharacterBody3D
{
	[Export] public float Speed { get; set; } = 10f;
	[Export] public float StopDistance { get; set; } = 0.12f;

	private float _gravity;

	public override void _Ready()
	{
		_gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	}

	public override void _PhysicsProcess(double delta)
	{
		var camera = GetViewport().GetCamera3D();
		if (camera == null)
			return;

		float dt = (float)delta;
		var mouse = GetViewport().GetMousePosition();
		var from = camera.ProjectRayOrigin(mouse);
		var dir = camera.ProjectRayNormal(mouse);

		if (Mathf.IsZeroApprox(dir.Y))
		{
			float vy = Velocity.Y;
			if (!IsOnFloor())
				vy -= _gravity * dt;
			else
				vy = 0f;
			Velocity = new Vector3(0f, vy, 0f);
			MoveAndSlide();
			return;
		}

		float t = -from.Y / dir.Y;
		var target = from + dir * t;

		if (!IsOnFloor())
			Velocity = new Vector3(Velocity.X, Velocity.Y - _gravity * dt, Velocity.Z);
		else
			Velocity = new Vector3(Velocity.X, 0f, Velocity.Z);

		var toTarget = target - GlobalPosition;
		toTarget.Y = 0f;
		float dist = toTarget.Length();
		if (dist <= StopDistance)
			Velocity = new Vector3(0f, Velocity.Y, 0f);
		else
		{
			var moveDir = toTarget / dist;
			Velocity = new Vector3(moveDir.X * Speed, Velocity.Y, moveDir.Z * Speed);
		}

		MoveAndSlide();
	}
}
