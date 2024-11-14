using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	public override void _Ready()
	{
		Utilities.CurrentScene = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("AddThing"))
		{
			Utilities.InstantiateNewComponent();
		}
	}
}