using Godot;
using System;
using System.Collections.Generic;

public class Rabbit : KinematicBody, Agent
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
	public double maxSat = 10.0;
	
	private double Health;
	public double health
	{
		get { return Health; }
		set { Health = value; }
	}
	public double minHP = 0.0;
	public double maxHP = 20.0;
	
	public Timer hunger;
	public Timer wander;
	public Timer starving;
	
	private Vector3 velocity = new Vector3();
	private float gravity = -50f;
	private float speed = 600f;
	
	private bool foraging = false;
	private Grass target;
	
	private Vector3 wanderTarget = new Vector3();
	
	public override void _Ready()
	{
		type = "Rabbit";
		satisfaction = minSat;
		health = maxHP;
		
		hunger = GetNode<Timer>("Hunger");
		hunger.Start();
		
		wander = GetNode<Timer>("Wander");
		
		starving = GetNode<Timer>("Starving");
	}
	
	public void Reproduce()
	{
		GetNode<Root>("/root/Root").AddRabbit(Translation);
	}
	
	public void Die()
	{
		GetNode<Root>("/root/Root").rabbitList.Remove(this);
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
		// rabbits move
		Behavior(delta);
	}
	
	public void Behavior(float delta)
	{
		// driven by hunger
		// if hungry will find grass and eat
		// grass will fill health if empty and satisfaction otherwise
		// the rabbit cannot lose satisfaction and eating takes away the health of the grass
		// otherwise it will just wander
		velocity.x = 0;
		velocity.z = 0;
		
		if (foraging)
		{
			// if distance from target is within a certain range then eat and flip the state of foraging
			// otherwise move towards the target
			if (IsInstanceValid(target))
			{
				float dist = Translation.DistanceTo(target.Translation);
				if (dist < 3.5f)
				{
					velocity.x = 0;
					velocity.z = 0;
					Random rand = new Random();
					double damage = rand.NextDouble() * 5 + 3;
					target.ChangeHealth(-damage);
					if (health < maxHP)
					{
						ChangeHealth(damage - 3);
					}
					else
					{
						ChangeSatisfaction(damage);
					}
					foraging = false;
					hunger.Start();
				}
				else
				{
					velocity += -speed * delta * Transform.basis.z;
					if (starving.IsStopped())
					{
						ChangeHealth(-2);
						starving.Start();
					}
				}
			}
			else
			{
				starving.Stop();
				hunger.Stop();
				hunger.Start();
				foraging = false;
			}
		}
		else if (hunger.IsStopped())
		{
			GetTarget();
			starving.Start();
		}
		else
		{
			// wander
			if (wander.IsStopped())
			{
				// pick a point on the terrain
				// same method used to look in the direction of grass
				Terrain terrain = GetNode<Terrain>("/root/Root/Surface/Terrain");
				Random rand = new Random();
				wanderTarget.x = (float)rand.NextDouble() * 50f - 25f;
				wanderTarget.z = (float)rand.NextDouble() * 50f - 25f;
				
				wanderTarget += Translation;
				
				if (wanderTarget.x > 250f)
				{
					wanderTarget.x = 250f;
				}
				else if (wanderTarget.x < -250f)
				{
					wanderTarget.x = -250f;
				}
				
				if (wanderTarget.z > 250f)
				{
					wanderTarget.z = 250f;
				}
				else if (wanderTarget.z < -250f)
				{
					wanderTarget.z = -250f;
				} 
				
				LookAt(wanderTarget, new Vector3(0, 1, 0));
				SetRotation(new Vector3(0, Rotation.y, 0));
				wander.Start();
			}
			else
			{
				float dist = Translation.DistanceTo(wanderTarget);
				if (dist <= 3f)
				{
					velocity.x = 0;
					velocity.y = 0;
				}
				else
				{
					velocity += -speed * delta * Transform.basis.z;
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
		// get the closest grass from the root and turn in that direction
		foraging = true;
		List<Grass> grass = GetNode<Root>("/root/Root").grassList;
		float minDist = Translation.DistanceTo(grass[0].Translation);
		foreach (Grass g in grass)
		{
			float dist = Translation.DistanceTo(g.Translation);
			if (dist <= minDist)
			{
				minDist = dist;
				target = g;
			}
		}
		LookAt(target.Translation, new Vector3(0, 1, 0));
		SetRotation(new Vector3(0, Rotation.y, 0));
	}
}
