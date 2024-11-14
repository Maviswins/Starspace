using Godot;
using System;

public partial class HolderCollection : CompCollection
{
	//Build-Mode specific. Only holds a root node and ignores list.

	public string CurrentComponentScenePath;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	//Swap out the root component only.
    public override void AddComponent(Component subject)
    {
        if (RootComponent != null)
		{
			if (RootComponent == subject)
			{
				//do nothing.
			}
			else
			{
				RemoveComponent(RootComponent);
			}
		}
		RootComponent = subject;
		AddChild(subject);
    }

	public override void RemoveComponent(Component subject)
	{
		RootComponent = null;
		Utilities.RemoveChildDeferred(subject);
	}

	//Check root component only.
	public override bool HasComponent(Component subject)
	{
		if (RootComponent == subject)
		{
			return true;
		}
		return false;
	}
}
