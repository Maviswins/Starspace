using Godot;
using System;
using System.Drawing;

public partial class BuildModeShipCollection : CompCollection
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node n in GetChildren())
		{
			if (n is Component root)
			{
				RootComponent = root;
				root.SetAsRootComponent();
				root.BM = (BuildMode)GetParent();
			}
		}
		Components = new();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
