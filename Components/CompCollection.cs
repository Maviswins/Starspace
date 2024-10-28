using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CompCollection : Node2D
{
	public Component RootComponent;
	public List<Component> Components;
	
	public bool IsShip;
	public override void _Ready()
	{
		Components = new();
	}

	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// go therough and make sure all connection points are as they should be. Deselect them all too.
	/// </summary>
	public void ValidateAllConnectionPoints()
	{
		Queue<Component> checkList = GetAllComponents();
		if (checkList.Any())
		{
			while (checkList.Any())
			{
				Component current = checkList.Dequeue();
				List<Component> remainingComponents = checkList.ToList();
				List<ConnectionPoint> currentPoints = GetAllConnectionPoints(current);

				foreach (ConnectionPoint currentPoint in currentPoints)
				{
					Vector2 position = current.Position + currentPoint.Position;
					if (OtherCPAtPosition(position, remainingComponents))
					{
						currentPoint.SetAsAttached();
					}
					if (currentPoint.IsSelected)
					{
						currentPoint.IsSelected = false;
					}
				}
			}
		}
	}

	public bool OtherCPAtPosition(Vector2 checkPosition, List<Component> components)
	{
		foreach (Component component in components)
		{
			List<ConnectionPoint> conPoints = GetAllConnectionPoints(component);
			foreach (ConnectionPoint currentPoint in conPoints)
			{
				if (component.Position + currentPoint.Position == checkPosition)
				{
					currentPoint.SetAsAttached();
					return true;
				}
			}
		}
		return false;
	}

	//Add a component to this collection.
	public virtual void AddComponent(Component subject)
	{
		if (!HasComponent(subject))
		{
			Utilities.AddChildDeferred(subject, this);
			if (RootComponent is null)
			{
				RootComponent = subject;
			}
			else
			{
				Components.Add(subject);
			}
			GD.Print(this + " added " + subject);
		}
	}

	//Remove a component from this collection.
	public virtual void RemoveComponent(Component subject)
	{
		if (HasComponent(subject))
		{
			if (Components.Any())
			{
				if (Components.Contains(subject))
				{
					Components.Remove(subject);
				}
			}
			else if (RootComponent == subject)
			{
				RootComponent = null;
				if (Components.Any())
				{
					RootComponent = Components.First();
					Components.Remove(Components.First());
				}
			}
			Utilities.RemoveChildDeferred(subject);
			GD.Print(this + " removed " + subject);
		}
	}

	//Check if component is in this collection.
	public virtual bool HasComponent(Component subject)
	{
		//null check:
		if (subject != null)
		{
			//child node check:
			foreach (Node n in GetChildren())
			{
				if (n is Component c)
				{
					if (c == subject)
					{
						//root check:
						if (RootComponent == subject)
						{
							return true;
						}
						//list check:
						else if (Components.Any())
						{
							if (Components.Contains(subject))
							{
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	public Queue<Component> GetAllComponents()
	{
		//List of all components.
		Queue<Component> cs = new();
		foreach (Node n in GetChildren())
		{
			if (n is Component c)
			{
				//GD.Print(c);
				cs.Enqueue(c);
			}
		}
		return cs;
	}

	public List<ConnectionPoint> GetAllConnectionPoints(Component subject)
	{
		List<ConnectionPoint> cps = new();
		foreach (Node n in subject.GetChildren())
		{
			if (n is ConnectionPoint cp)
			{
				cps.Add(cp);
			}
		}
		return cps;
	}
}