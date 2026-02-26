using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] MapView _mapView;

    public void Initialize()
    {
        _mapView.Initialize();
    }

    public void RequestToggleMap()
    {
        _mapView.ToggleMap();
    }

    public void RequestScrollMap(Vector2 input)
    {
        _mapView.ScrollMap(input);
    }
}