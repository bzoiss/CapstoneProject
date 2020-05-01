using Godot;
using System;

public class Grass : KinematicBody, Agent
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
	public double maxHP = 5.0;
	
	public Timer clock;
	
	private int Children = 0;
	
	public override void _Ready()
	{
		type = "Grass";
		satisfaction = 0.0;
		health = 5.0;
		
		clock = GetNode<Timer>("SunCollection");
		Random rand = new Random();
		clock.WaitTime = (float)rand.NextDouble() * 5 + 2;
		
		clock.Start();
	}
	
	public void Reproduce()
	{
		if (Children < 5)
		{
			// add a new grass to the scene tree
			// based on the location of this one spawn one offset coordinate wise
			GetNode<Root>("/root/Root").AddGrass(Translation);
			Children += 1;
		}
	}
	
	public void Die()
	{
		GetNode<Root>("/root/Root").grassList.Remove(this);
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
		// call behavior
		Behavior(delta);
		// grass is sessile so no movement needed
	}
	
	public void Behavior(float delta)
	{
		// gather sun on a given interval
		if (clock.IsStopped())
		{
			Random rand = new Random();
			if (health < maxHP)
			{
				ChangeHealth(rand.NextDouble() * 1.5);
			}
			else
			{
				ChangeSatisfaction(rand.NextDouble() * 1.5);
			}
			clock.Start();
		}
		// if health is not full heal by a random amount
		// otherwise add to satisfaction
		// grass cannot lose satisfaction
	}
}
