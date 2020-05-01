using Godot;
using System;

public class Wolf : KinematicBody, Agent
{
	private string Type;
	public string type
	{
		get { return Type; }
		set { Type = value; }
	}
	
	private double Satisfaction;
	public double satisfaction
	{
		get { return Satisfaction; }
		set { Satisfaction = value; }
	}
	public double minSat = 0.0;
	public double maxSat = 30.0;
	
	private double Health;
	public double health
	{
		get { return Health; }
		set { Health = value; }
	}
	public double minHP = 0.0;
	public double maxHP = 40.0;
	
	private Vector3 velocity = new Vector3();
	private float gravity = -50f;
	private float walkingSpeed = 500f;
	private float runningSpeed = 900f;
	
	private bool hungry = false;
	private Rabbit target;
	
	public Vector3 home = new Vector3();
	public Vector3 homeWander = new Vector3();
	
	private Timer hunger;
	private Timer wander;
	private Timer starve;
	
	public override void _Ready()
	{
		type = "Wolf";
		satisfaction = minSat;
		health = maxHP;
		
		hunger = GetNode<Timer>("Hunger");
		wander = GetNode<Timer>("Wander");
		starve = GetNode<Timer>("Starving");
		
		Random r = new Random();
		hunger.WaitTime = (float)r.NextDouble() * 12f + 10f;
		
		hunger.Start();
	}
	
	public void Reproduce()
	{
		GetNode<Root>("/root/Root").AddWolf(Translation, this);
	}
	
	public void Die()
	{
		GetNode<Root>("/root/Root").wolfList.Remove(this);
		QueueFree();
	}
	
	public void ChangeSatisfaction(double s)
	{
		satisfaction += s;
		if (satisfaction > maxSat)
		{
			Reproduce();
			satisfaction = minSat;
		}
		else if (satisfaction < minSat)
		{
			satisfaction = minSat;
		}
	}
	
	public void ChangeHealth(double h)
	{
		health += h;
		if (health > maxHP)
		{
			health = maxHP;
		}
		else if (health < minHP)
		{
			Die();
		}
	}
	
	public override void _PhysicsProcess(float delta)
	{
		Behavior(delta);
	}
	
	public void Behavior(float delta)
	{
		// the wolf will around their "home"
		// when the wolf is hungry it will run after a rabbit and pursue it until it kills it
		velocity.x = 0;
		velocity.z = 0;
		
		if (hunger.IsStopped() && !hungry)
		{
			GetTarget();
			hungry = true;
			starve.Start();
		}
		
		if (hungry)
		{
			if (IsInstanceValid(target))
			{
				LookAt(target.Translation, new Vector3(0, 1, 0));
				SetRotation(new Vector3(0, Rotation.y, 0));
				float dist = Translation.DistanceTo(target.Translation);
				if (dist <= 3.5f)
				{
					// eat it
					target.ChangeHealth(-30);
					
					if (health < maxHP)
					{
						ChangeHealth(5);
					}
					else
					{
						ChangeSatisfaction(10);
					}
					
					hunger.Start();
					starve.Stop();
					hungry = false;
				}
				else
				{
					velocity += -runningSpeed * delta * Transform.basis.z;
					if (starve.IsStopped())
					{
						ChangeHealth(-5);
						starve.Start();
					}
				}
			}
			else
			{
				hunger.Start();
				starve.Start();
				hungry = false;
			}
		}
		else
		{
			if (wander.IsStopped())
			{
				Vector3 pos = new Vector3();
				Terrain t = GetNode<Terrain>("/root/Root/Surface/Terrain");
				Random r = new Random();
				pos.x = (float)r.NextDouble() * 50f - 25f;
				pos.z = (float)r.NextDouble() * 50f - 25f;
				
				homeWander = home + pos;
				
				LookAt(homeWander, new Vector3(0, 1, 0));
				SetRotation(new Vector3(0, Rotation.y, 0));
				
				wander.Start();
			}
			else
			{
				float dist = Translation.DistanceTo(homeWander);
				if (dist >= 3f)
				{
					velocity += -walkingSpeed * delta * Transform.basis.z;
				}
			}
		}
		
		if (IsOnFloor())
		{
			velocity.y = 0;
		}
		else
		{
			velocity.y += gravity * delta;
		}
		
		MoveAndSlide(velocity, new Vector3(0, 1, 0), false, 4, (float)Math.PI);
	}
	
	public void GetTarget()
	{
		float minDist = 999f;
		foreach (Rabbit r in GetNode<Root>("/root/Root").rabbitList)
		{
			float dist = Translation.DistanceTo(r.Translation);
			if (dist <= minDist)
			{
				minDist = dist;
				target = r;
			}
		}
	}
}
