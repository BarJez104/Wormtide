using Godot;

namespace Wormtide.components.scripts.interfaces;

public abstract partial class Removal : Node3D
{
    public abstract void Begin(Node3D collector);
}
