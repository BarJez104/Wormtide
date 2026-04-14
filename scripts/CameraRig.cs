using Godot;

public partial class CameraRig : Node3D
{
	private Camera3D? _camera;

	[Export] public float MinSize { get; set; } = 4f;
	[Export] public float MaxSize { get; set; } = 40f;
	[Export] public float ZoomStep { get; set; } = 1.5f;

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		if (_camera != null)
			_camera.Size = Mathf.Clamp(_camera.Size, MinSize, MaxSize);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (_camera == null)
			return;
		if (@event is InputEventMouseButton mb && mb.Pressed)
		{
			switch (mb.ButtonIndex)
			{
				case MouseButton.WheelUp:
					_camera.Size = Mathf.Clamp(_camera.Size - ZoomStep, MinSize, MaxSize);
					break;
				case MouseButton.WheelDown:
					_camera.Size = Mathf.Clamp(_camera.Size + ZoomStep, MinSize, MaxSize);
					break;
			}
		}
	}
}
