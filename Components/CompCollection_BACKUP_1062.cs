using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CompCollection : Node2D
{
	public Component RootComponent;
	public List<Component> Components;
<<<<<<< HEAD
	public bool HasCockpit = false;
	public override void _Ready()
	{
=======
	
	public bool IsShip;
	public override void _Ready()
	{
		Components = new();
>>>>>>> p2pBuild
	}

	public override void _Process(double delta)
	{
	}

<<<<<<< HEAD
	/*
	point to point concept:
	- In build mode there are two collections:
		- The main ship collection with cockpit as root.
		- a holder collection that contains the currently chosen module.
	- Spawn modules to designated location in holder collection only.
		- On selecting a module from the menu or clicking a module already present in ship collection (not cockpit?)
			- Create new instance there.
	- Only unattached snap points recognise user input.
	- Snap point traffic light schema:
		- Red = no attachment.
		- Yellow = no attachment, currently selected.
		- Green = attached to collection. 
	- On mouseover of unattached snap point.
		- Increase scale 2x.
		- On click:
			- If in holder collection:
				- Change to yellow, mark as selected.
				- If another point is already selected, deselect that one.
			- If in ship collection:
				- Create instance of selected holder component and check for overlap of modules if an attachment is formed.
				- If no overlap issue:
					- Snap new instance of component to chosen ship component according to snap points. Add to that collection.
				- If no overlap issue:
					- For now do nothing, provide feedback in the future.
	- On mouseoff a snap point.
		- Return scale to normal.
	*/

	//Drag logic
	/*
	private bool _mouseIsOver;
	private bool _isBeingDragged;
	private bool _wantsToSnap;
	private bool _isSnapped;
	private int _snapThreshold;
	private Vector2 _lastPosition;

	public override void _Input(InputEvent @event)
	{
		//If left mouse is down:
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			//If the mouse is over this collection:
			if (_mouseIsOver)
			{
				//If the mouse is over the root of this collection:
				if (RootComponent.MouseIsOver)
				{
					//If the mouse is moving and this collection isn't being dragged:
					if (@event is InputEventMouseMotion && !_isBeingDragged)
					{
						_isBeingDragged = true;
						//Store position in case release location is invalid.
						_lastPosition = Position;
					}
					//If the mouse is moving and this collection is being dragged:
					else if (@event is InputEventMouseMotion && _isBeingDragged)
					{
						//If this collection is snapped to another collection right now:
						if (AnyComponentsSnappedToAnotherCollection())
						{
							//If the snap threshold for root node has been exceeded:
							if (RootComponent.GlobalPosition.DistanceTo(GetGlobalMousePosition()) > _snapThreshold)
							{
								//Move the collection position to the mouse, offset by the position of the root component.
								Position = GetGlobalMousePosition() + RootComponent.Position;
							}
						}
					}
					//If the mouse button is released (currently any mouse button will trigger this I think?)
					else if (!mouseEvent.Pressed)
					{
						_isBeingDragged = false;
						if (AnyComponentsSnappedToAnotherCollection())
						{
							List<CompCollection> comps = GetListOfSnappedCollections();
							if (comps.Any())
							{
								jhfgjf
							}
						}
					}
				}
				
				//If it's not the root component being dragged:
				else
				{
					//Hopefully find the component the mouse is over.
					try
					{
						Component subject = Components.Where(c => c.MouseIsOver == true).FirstOrDefault();
						if (subject is not null)
						{
							subject.IsBeingDragged = true;

							//Check if the cursor has exceeded the snap threshold for this component:
							//Maybe global position is correct?
							if (subject.IsBeingDragged == true && subject.GlobalPosition.DistanceTo(GetGlobalMousePosition()) > _snapThreshold)
							{
								//Split collection if so (will probably be fucked).
								Utilities.SplitCollection(this, subject);
							}
							//Reset bool if mouse-up is detected.
							else if (!mouseEvent.Pressed)
							{
								subject.IsBeingDragged = false;
							}
						}
					}
					catch {}
				}
			}
		}
		// //Snap logic
		// else if (@event is InputEventMouseMotion && IsBeingDragged)
		// {
		// 	//Will only freely track cursor position if it is not currently snapped to another
		// 	//module or exceeds the threshold to unsnap.
		// 	if (!IsSnapped)
		// 	{
		// 		Position = GetGlobalMousePosition() + _dragOffset;
		// 	}
		// 	else
		// 	{
		// 		if (Position.DistanceTo(GetGlobalMousePosition()) > _snapThreshold)
		// 		{
		// 			IsSnapped = false;
		// 		}
		// 	}
		// }
	}

	private bool AnyComponentsSnappedToAnotherCollection()
	{
		foreach (Component c in Components)
		{
			if (IsSnappedToAnotherCollection(c))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsSnappedToAnotherCollection(Component subject)
	{
		foreach (KeyValuePair<ConnectionPoint, ConnectionPoint> kvp in subject.ConDict)
		{
			if (kvp.Value is not null && !CheckForComponent((Component)kvp.Value.GetParent()))
			{
				return true;
			}
		}
		return false;
	}


	/// <summary>
	/// Pull a list of unique collections that components are currently snapped to.
	/// </summary>
	/// <returns></returns>
	private List<CompCollection> GetListOfSnappedCollections()
	{
		List<CompCollection> output = new();
		foreach (Component c in Components)
		{
			foreach (KeyValuePair<ConnectionPoint, ConnectionPoint> kvp in c.ConDict)
			{
				//If a connection exists and isn't this collection:
				if (kvp.Value is not null && !CheckForComponent((Component)kvp.Value.GetParent()))
				{
					//If not already in list try to add it.
					if (!output.Contains(kvp.Value.GetParent().GetParent()))
					{
						try
						{
							output.Add((CompCollection)kvp.Value.GetParent().GetParent());
						}
						catch{}
=======
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
				Component currentSubject = checkList.Dequeue();
				List<Component> remainingComponents = checkList.ToList();
				List<ConnectionPoint> currentPoints = GetAllConnectionPoints(currentSubject);

				foreach (ConnectionPoint currentPoint in currentPoints)
				{
					Vector2 pointPos = GetGlobalPointPos(currentSubject, currentPoint);
					//GD.Print("Checking " + pointPos);
					ConnectionPoint attachedPoint = OtherCPAtPosition(pointPos, remainingComponents);
					if (attachedPoint != null)
					{
						currentPoint.SetAsAttached(attachedPoint);
						attachedPoint.SetAsAttached(currentPoint);
					}
					if (currentPoint.IsSelected)
					{
						currentPoint.IsSelected = false;
>>>>>>> p2pBuild
					}
				}
			}
		}
<<<<<<< HEAD
		return output;
	}

    private void OnMouseEntered()
    {
        _mouseIsOver = true;
    }

    private void OnMouseExited()
    {
        _mouseIsOver = false;
    }
	*/
	public void SanitiseComponent(Component subject)
	{
		ValidateComponent(subject);
	}
	public void InitialiseComponent(Component subject)
	{
		ValidateComponent(subject);
	}

	private bool CheckForComponent(Component subject)
	{
		if (GetChildren().Contains(subject) && Components.Contains(subject) && CheckForComponentNode(subject))
		{
			return true;
		}
		return false;
	}

	private bool CheckForComponentNode(Component subject)
	{
		foreach (Component c in GetChildren())
		{
			if (c == subject)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Ensure a component is properly registered to this collection.
	/// If it is, it returns true. If it's a child with missing registries it'll fix that. If it's not a child it'll remove any registries.
	/// Will not affect node hierarchy.
	private bool ValidateComponent(Component subject)
	{
		//Obligatory null check.
		if (subject is null)
		{
			return false;
		}
		else
		{
			//If subject is a child node of collection:
			if (GetChildren().Contains(subject))
			{
				//If it can't be set as the root:
				if (!SetRootComponent(subject))
				{
					if (!Components.Contains(subject))
					{
						Components.Add(subject);
					}
					return true;
				}
				//Check if it's already the root if setting failed:
				else if (RootComponent == subject)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			//If component is not found as a child node of collection:
			else
			{
				//Strip any reference from list.
				if (Components.Contains(subject))
				{
					Components.Remove(subject);
					return false;
				}
				//Strip from root if applicable.
				else if (RootComponent == subject)
				{
					RootComponent = null;
					return false;
				}
			}
			return true;
		}
	}

	/// <summary>
	/// Attempt to set the collection's root component. Will not permit it to be set to null.
	/// Can force the root to be replaced by input component or top listed component if it's null.
	/// </summary>
	public bool SetRootComponent(Component newRoot, bool forceChange = false)
	{
		//Make sure nothing dumb is happening.
		if (newRoot != RootComponent)
		{
			//Forcibly add newRoot as root unless it's the cockpit.
			if (forceChange && newRoot is not null)
			{
				if (RootComponent is not null)
				{
					if (!RootComponent.IsCockpit)
					{
						Components.Add(RootComponent);
						RootComponent = newRoot;
						if (Components.Contains(newRoot))
						{
							Components.Remove(newRoot);
						}
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					RootComponent = newRoot;
					if (Components.Contains(newRoot))
					{
						Components.Remove(newRoot);
					}
					return true;
				}
			}
			//Try and set the first listed component as root if root is not null.
			else if (forceChange && newRoot is null)
			{
				if (RootComponent is not null)
				{
					return false;
				}
				else
				{
					if (Components.Any())
					{
						RootComponent = Components.First();
						Components.Remove(Components.First());
						return true;
					}
					else
					{
						//Empty collection - should probably be removed.
						return false;
					}
				}
			}
			//Set root as newRoot only if there isn't already one there
			else if (!forceChange && newRoot is not null)
			{
				if (RootComponent is null)
				{
					RootComponent = newRoot;
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (!forceChange && newRoot is null)
			{
				//Try and set a root from listing if possible.
				if (RootComponent is null)
				{
					if (Components.Any())
					{
						RootComponent = Components.First();
						Components.Remove(Components.First());
						return true;
					}
					else
					{
						//Empty collection - should be removed.
					}
				}
				else
				{
					return false;
				}
			}
		}
		//Presumably the newRoot is already the root so all good hopefully.
		return true;
=======
	}

	public ConnectionPoint OtherCPAtPosition(Vector2 comparer, List<Component> components)
	{
		foreach (Component subject in components)
		{
			List<ConnectionPoint> conPoints = GetAllConnectionPoints(subject);
			foreach (ConnectionPoint subjectPoint in conPoints)
			{
				// Vector2 check = GetGlobalPointPos(subject, subjectPoint);
				//string report = "";
				//report += "against ";
				//report += check;
				if (GetGlobalPointPos(subject, subjectPoint) == comparer)
				{
					//report += " true";
					//GD.Print(report);
					return subjectPoint;
				}
				//report += " false";
				//GD.Print(report);
			}
		}
		return null;
	}

	private Vector2 GetGlobalPointPos(Component subject, ConnectionPoint point)
	{
		Vector2 subjectPos = subject.GlobalPosition;
		Vector2 pointPos = point.GlobalPosition;
		Vector2 globalPointPos = subjectPos - pointPos;
		Vector2 output = subject.Position - globalPointPos;
		return output;
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
>>>>>>> p2pBuild
	}
}