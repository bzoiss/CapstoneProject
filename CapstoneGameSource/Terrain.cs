using Godot;
using System;

public class Terrain : MeshInstance
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	private SurfaceTool st = new SurfaceTool();
	private MeshDataTool mdt = new MeshDataTool();
	private OpenSimplexNoise noise = new OpenSimplexNoise();
	
	public int size = 500;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Random r = new Random();
		noise.SetSeed(r.Next());
		noise.SetOctaves(4);
		noise.SetPeriod(100.0f);
		noise.SetPersistence(0.7f);
		
		st.CreateFrom(this.Mesh, 0);
		ArrayMesh plane = st.Commit();
		
		mdt.CreateFromSurface(plane, 0);
		
		for(int i = 0; i < mdt.GetVertexCount(); i++)
		{
			var v = mdt.GetVertex(i);
			v.y = noise.GetNoise2d(v.x, v.z) * 30;
			mdt.SetVertex(i, v);
		}
		for(int i = 0; i < plane.GetSurfaceCount(); i++)
		{
			plane.SurfaceRemove(i);
		}
		mdt.CommitToSurface(plane);
		st.CreateFrom(plane, 0);
		st.GenerateNormals();
		this.Mesh = st.Commit();
		
		this.CreateTrimeshCollision();
	}
	
	public float getHeight(float x, float z)
	{
		return noise.GetNoise2d(x, z) * 30;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
