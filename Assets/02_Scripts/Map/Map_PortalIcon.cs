using System;
using UnityEngine;

public class Map_PortalIcon : Map_Icon
{
    private PortalModel _model;
    
    public event Action<PortalModel> OnPortalClicked;

    public void Initialize(PortalModel model)
    {
        _model = model;
    }

    protected override void OnIconClicked()
    {
        OnPortalClicked?.Invoke(_model);
    }
}