using Godot;
using System;
using System.Collections.Generic;

public class Root : Spatial
{
	
	public Random rand = new Random();
	
	private PackedScene grass = (PackedScene)ResourceLoader.Load("res://Imports/Grass.tscn");
	private PackedScene rabbit = (PackedScene)ResourceLoader.Load("res://Imports/Rabbit.tscn");
	private PackedScene wolf = (PackedScene)ResourceLoader.Load("res://Imports/Wolf.tscn");
	
	public List<Grass> grassList = new List<Grass>();
	public List<Rabbit> rabbitList = new List<Rabbit>();
	public List<Wolf> wolfList = new List<Wolf>();
	
	[Export]
	public int numGrass = 400;
	[Export]
	public int numRabbit = 40;
	[Export]
	public int numWolf = 5;
	
	public override void _Ready()
	{
		for (int i = 0; i < numGrass; i++)
		{
			AddGrass();
		}
		for (int i = 0; i < numRabbit; i++)
		{
			AddRabbit();
		}
		for (int i = 0; i < numWolf; i++)
		{
			AddWolf();
		}
	}
	
	public void AddGrass()
	{
		Vector3 pos = new Vector3();
		Terrain terrain = GetNode<Terrain>("/root/Root/Surface/Terrain");
		pos.x = (float)rand.NextDouble() * (float)terrain.size - (float)terrain.size / 2f;
		pos.z = (float)rand.NextDouble() * (float)terrain.size - (float)terrain.size / 2f;
		pos.y = terrain.getHeight(pos.x, pos.z) + .5f;
		
		Grass grassInstance = (Grass)grass.Instance();
		grassInstance.Translation = pos;
		AddChild(grassInstance);
		grassList.Add(grassInstance);
	}
	
	public void AddRabbit()
	{
		Vector3 pos = new Vector3();
		Terrain terrain = GetNode<Terrain>("/root/Root/Surface/Terrain");
		pos.x = (float)rand.NextDouble() * (float)terrain.size - (float)terrain.size / 2f;
		pos.z = (float)rand.NextDouble() * (float)terrain.size - (float)terrain.size / 2f;
		pos.y = terrain.getHeight(pos.x, pos.z) + 1;
		
		Rabbit rabbitInstance = (Rabbit)rabbit.Instance();
		rabbitInstance.Translation = pos;
		AddChild(rabbitInstance);
		rabbitList.Add(rabbitInstance);
	}
	
	public void AddGrass(Vector3 pos)
	{
		Terrain terrain = GetNode<Terrain>("/root/Root/Surface/Terrain");
		pos.x += (float)rand.NextDouble() * 4 - 2f;
		pos.z += (float)rand.NextDouble() * 4 - 2f;
		pos.y = terrain.getHeight(pos.x, pos.z) + .5f;
		
		Grass grassInstance = (Grass)grass.Instance();
		grassInstance.Translation = pos;
		AddChild(grassInstance);
		grassList.Add(grassInstance);
	}
	
	public void AddRabbit(Vector3 pos)
	{
		Terrain terrain = GetNode<Terrain>("/root/Root/Surface/Terrain");
		pos.x += (float)rand.NextDouble() * 4 - 2f;
		pos.z += (float)rand.NextDouble() * 4 - 2f;
		pos.y = terrain.getHeight(pos.x, pos.z) + 1f;
		
		Rabbit rabbitInstance = (Rabbit)rabbit.Instance();
		rabbitInstance.Translation = pos;
		AddChild(rabbitInstance);
		rabbitList.Add(rabbitInstance);
	}
	
	public void AddWolf()
	{
		Vector3 pos = new Vector3();
		Terrain terrain = GetNode<Terrain>("/root/Root/Surface/Terrain");
		pos.x = (float)rand.NextDouble() * ((float)terrain.size - 50) - ((float)terrain.size - 50) / 2f;
		pos.z = (float)rand.NextDouble() * ((float)terrain.size - 50) - ((float)terrain.size - 50) / 2f;
		pos.y = terrain.getHeight(pos.x, pos.z) + 3;
		
		Wolf wolfInstance = (Wolf)wolf.Instance();
		wolfInstance.Translation = pos;
		AddChild(wolfInstance);
		wolfList.Add(wolfInstance);
		wolfInstance.home = pos;
	}
	
	public void AddWolf(Vector3 pos, Wolf w)
	{
		Terrain terrain = GetNode<Terrain>("/root/Root/Surface/Terrain");
		pos.x += (float)rand.NextDouble() * 4 - 2f;
		pos.z += (float)rand.NextDouble() * 4 - 2f;
		pos.y = terrain.getHeight(pos.x, pos.z) + 3;
		
		Wolf wolfInstance = (Wolf)wolf.Instance();
		wolfInstance.Translation = pos;
		AddChild(wolfInstance);
		wolfList.Add(wolfInstance);
		wolfInstance.home = w.home;
	}
}
