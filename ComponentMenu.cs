using Godot;
using System;

public partial class ComponentMenu : Control
{
	//Container for list items.
    private ItemList _itemList;

    private HolderCollection _holder;

    public BuildMode buildMode;

    //Where to look for list items.
    private string _componentsPath = "res://Components/"; 
	
	public override void _Ready()
	{
		_itemList = GetNode<ItemList>("ItemList");
        _itemList.ItemSelected += OnItemListSelected;

		//Iteratively adds objects stored in components directory to the item list.
        using (DirAccess dir = DirAccess.Open(_componentsPath))
        {
            if (dir != null)
            {
                dir.ListDirBegin();

                string fileName = dir.GetNext();

                while (fileName != "")
                {
                    if (!dir.CurrentIsDir() && fileName.EndsWith(".tscn"))
                    {
                        _itemList.AddItem(fileName);
                    }

                    fileName = dir.GetNext();
                }

                dir.ListDirEnd();
            }
        }
	}

	public override void _Process(double delta)
	{
	}

    /// <summary>
    /// Fired when a list item is selected. Instantiates corresponding object into Main.
    /// </summary>
    private void OnItemListSelected(long longIndex)
    {
        int index = (int)longIndex;

        //Uses item text as a locator for the underlying object.
        string sceneName = _itemList.GetItemText(index);
        string scenePath = $"{_componentsPath}/{sceneName}";

        buildMode.SetHolderComponent(scenePath);
    }
}