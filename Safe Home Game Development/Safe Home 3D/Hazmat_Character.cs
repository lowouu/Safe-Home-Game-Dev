using Godot;
using System;

public partial class Hazmat_Character : CharacterBody3D
{

    public const float MouseSensitivity = 0.05f;
    public const float Speed = 5.0f;
    public const float JumpVelocity = 4.5f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    private Node3D _neck;
    private Camera3D _camera;
    public override void _Ready()
    {
        _neck = GetNode<Node3D>("Neck");
        _camera = _neck.GetNode<Camera3D>("Camera3D");
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            // Rotate the neck (up/down)
            _neck.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * MouseSensitivity));
            _neck.RotationDegrees = new Vector3(
                Mathf.Clamp(_neck.RotationDegrees.X, -90, 90),
                _neck.RotationDegrees.Y,
                _neck.RotationDegrees.Z);

            // Rotate the player (left/right)
            RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * MouseSensitivity));
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y -= gravity * (float)delta;

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            velocity.Y = JumpVelocity;

        // Get the input direction and handle the movement/deceleration.
        Vector3 direction = Vector3.Zero;

        if (Input.IsActionPressed("left"))
            direction -= Transform.Basis.X;
        if (Input.IsActionPressed("right"))
            direction += Transform.Basis.X;
        if (Input.IsActionPressed("forward"))
            direction -= Transform.Basis.Z;
        if (Input.IsActionPressed("back"))
            direction += Transform.Basis.Z;

        direction = direction.Normalized();

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