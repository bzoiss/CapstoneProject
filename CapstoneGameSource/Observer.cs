using Godot;
using System;

public class Observer : KinematicBody
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	private Vector3 velocity = new Vector3(0, 0, 0);
	private const float gravity = -50.0f;
	private const float speed = 300.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.SetMouseMode(Input.MouseMode.Captured);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		bool forward = Input.IsActionPressed("ui_up");
		bool backward = Input.IsActionPressed("ui_down");
		bool left = Input.IsActionPressed("ui_left");
		bool right = Input.IsActionPressed("ui_right");
		bool exit = Input.IsActionPressed("ui_cancel");
		
		velocity.x = 0;
		velocity.z = 0;
		
		if (forward)
		{
			velocity += speed * delta * Transform.basis.x;
		}
		if (backward)
		{
			velocity += -speed * delta * Transform.basis.x;
		}
		if (left)
		{
			velocity += -speed * delta * Transform.basis.z;
		}
		if (right)
		{
			velocity += speed * delta * Transform.basis.z;
		}
		
		// do physics
		if (IsOnFloor())
		{
			velocity.y = 0;
		}
		else
		{
			velocity.y += gravity * delta;
		}
		
		MoveAndSlide(velocity, new Vector3(0, 1, 0));
		Update();
		
		if (exit)
		{
			GetTree().Quit();
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
		{
			InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;
			RotateY(Mathf.Deg2Rad(-mouseEvent.Relative.x * .05f));
			Camera c = GetNode<Camera>("Camera");
			float rotX = Mathf.Deg2Rad(mouseEvent.Relative.y * .05f);
			if (c.RotationDegrees.x < -70)
			{
				c.SetRotationDegrees(new Vector3(-69, c.RotationDegrees.y, c.RotationDegrees.z));
			}
			else if (c.RotationDegrees.x > 70)
			{
				c.SetRotationDegrees(new Vector3(69, c.RotationDegrees.y, c.RotationDegrees.z));
			}
			else
			{
				c.RotateZ(-rotX);
			}
		}
	}
	
	public void Update()
	{
		Root r = GetNode<Root>("/root/Root");
		RichTextLabel l = GetNode<RichTextLabel>("Camera/Panel/Label");
		
		l.Text = "Grass: " + r.grassList.Count + "\nRabbits: " + r.rabbitList.Count + "\nWolves: " + r.wolfList.Count;
	}
}
