using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

public partial class BuildMode : Node2D
{
	private ComponentMenu _compMenu;
	private HolderCollection _holder;
	private BuildModeShipCollection _ship;

	public ConnectionPoint CurrentHolderSelection;

	public override void _Ready()
	{
		foreach (Node n in GetChildren())
		{
			if (n is HolderCollection holder)
			{
				_holder = holder;
				//GD.Print("registered " + holder + " as holder collection.");
			}
			if (n is ComponentMenu compMenu)
			{
				_compMenu = compMenu;
				_compMenu.buildMode = this;
				//GD.Print("registered " + compMenu + " as component menu.");
			}
			if (n is BuildModeShipCollection ship)
			{
				_ship = ship;
				//GD.Print("registered " + ship + " as ship collection.");
			}
		}
		_holder.Position = new Vector2(200, 100);
		_ship.Position = new Vector2(400, 100);
	}

	public override void _Process(double delta)
	{
	}

	public void SetHolderComponent(string scenePath)
	{
		//Instantiates object and loads into holder.
		CurrentHolderSelection = null;
		ClearHolderComponent();
        PackedScene scene = (PackedScene)GD.Load(scenePath);
        Node instance = scene.Instantiate();
        if (instance is Component c)
        {
            _holder.AddComponent(c);
			_holder.RootComponent = c;
			_holder.CurrentComponentScenePath = scenePath;
            //GD.Print("Added Component " + c);
        }
	}

	public void ClearHolderComponent()
	{
		foreach (Node n in _holder.GetChildren())
		{
			if (n is Component c)
			{
				_holder.RemoveComponent(c);
				_holder.RootComponent = null;
				_holder.CurrentComponentScenePath = null;
				GD.Print("Removed " + c + " from holder collection.");
				c.Dispose();
			}
		}
	}

	public void AddShipComponent(ConnectionPoint holderPoint, ConnectionPoint shipPoint)
	{
		//GD.Print(_ship.GetChildren().Count);
		PackedScene scene = (PackedScene)GD.Load(_holder.CurrentComponentScenePath);
        Node instance = scene.Instantiate();
		if (instance is Component newComponent)
        {
			newComponent.BM = this;
			_ship.AddChild(newComponent);
			if (_ship.RootComponent is null)
			{
				_ship.RootComponent = newComponent;
			}
			else
			{
				if (!_ship.Components.Contains(newComponent))
				{
					_ship.Components.Add(newComponent);
				}
			}
			try
			{
				PositioningLogic((Component)shipPoint.GetParent(), newComponent, shipPoint, GetNewInstanceConnectionPoint(newComponent, CurrentHolderSelection));
			}
			catch{}
			_ship.ValidateAllConnectionPoints();
		}
	}

	//Come back to this with a fresh mind because fuck.
	public void PositioningLogic(Component currentComponent, Component newComponent, ConnectionPoint currentPoint, ConnectionPoint newPoint)
	{
		float crot1 = RBS(currentComponent.RotationDegrees + currentPoint.RotationDegrees);
		float nrot1 = RBS(newComponent.RotationDegrees + newPoint.RotationDegrees);
		float nrot2 = RBS(crot1 - nrot1);
		float nrot3 = RBS(nrot2 + 180);
		newComponent.RotationDegrees = nrot3;

		newComponent.Position = currentComponent.Position;

		Vector2 npadj = newComponent.GlobalPosition - newPoint.GlobalPosition;
		newComponent.Position += npadj;
		Vector2 cpadj = currentComponent.GlobalPosition - currentPoint.GlobalPosition;
		newComponent.Position -= cpadj;
	}

	public float RBS(float input)
	{
		string report = "";
		//GD.Print();
		report += input;
		report += " to ";
		while (input >= 360 || input < 0)
		{
			if (input >= 360)
			{
				input -= 360;
			}
			else if (input < 0)
			{
				input += 360;
			}
		}
		double round = Math.Round(input, 0);
		input = (float)round;
		report += input;
		//GD.Print(report);
		return input;
	}

	public ConnectionPoint GetNewInstanceConnectionPoint(Component newComponent, ConnectionPoint currentPoint)
	{
		foreach (Node n in newComponent.GetChildren())
		{
			if (n is ConnectionPoint candidate)
			{
				if (candidate.Name == currentPoint.Name)
				{
					return candidate;
				}
			}
		}
		return null;
	}

	public void LeftClick(ConnectionPoint subject)
	{
		if (ValidConnectionStatus(subject))
		{
			UpdateSelectionStatus(subject);
		}
	}

	public void RightClick(ConnectionPoint subject)
	{
		subject.SetAsUnSelected();
	}

	public void RightClick(Component subject)
	{
		RemoveShipComponents(_ship, subject);
	}

	public void RemoveShipComponents(CompCollection ship, Component source)
	{
		//Confirm source is in ship.
		//Get list from utils.splitcollection.
		//iteratively remove resulting components from collection.
			//validate connections.
			//Add handling to remove connections already present.
	}

	/// <summary>
	/// Manage the selection status.
	/// </summary>
	public void UpdateSelectionStatus(ConnectionPoint subject)
	{
		//If not already set as current selection:
		if (CurrentHolderSelection != subject)
		{
			//If current selection is not null:
			if (subject != null)
			{
				//If it's a holder point update current selection.
				if (subject.GetParent().GetParent() == _holder)
				{
					if (CurrentHolderSelection != null)
					{
						CurrentHolderSelection.SetAsUnSelected();
					}
					CurrentHolderSelection = subject;
					subject.SetAsSelected();
					//GD.Print("Current selection is now " + CurrentHolderSelection);
				}

				//If subject is a ship point:
				else if (subject.GetParent().GetParent() == _ship)
				{
					//If current selection isn't null.
					if (CurrentHolderSelection != null)
					{
						AddShipComponent(CurrentHolderSelection, subject);
					}
				}

			}
			//If subject  is null:
			else if (subject == null)
			{
				CurrentHolderSelection = subject;
			}	
		}
	}

	/// <summary>
	/// Return true if a connection point can currently be set as selected.
	/// </summary>
	public bool ValidConnectionStatus(ConnectionPoint subject)
	{
		//Has current selection:
		if (CurrentHolderSelection != null)
		{
			//Permit holder.
			if (subject.GetParent().GetParent() == _holder)
			{
				//GD.Print("parent is holder and current selection not null ");
				return true;
			}
			//Permit ship.
			else if (subject.GetParent().GetParent() == _ship)
			{
				//GD.Print("parent is ship and current selection not null");
				return true;
			}
		}
		//No current selection:
		else if (CurrentHolderSelection == null)
		{
			//Permit holder.
			if (subject.GetParent().GetParent() == _holder)
			{
				//GD.Print("parent is ship and current selection is null");
				return true;
			}
			//Disallow ship.
			else if (subject.GetParent().GetParent() == _ship)
			{
				//GD.Print("parent is ship and current selection is null");
				return false;
			}
		}
		return false;
	}
}
