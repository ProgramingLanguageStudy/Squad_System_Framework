using UnityEngine;

public class IntroScene : MonoBehaviour
{
    [SerializeField] IntroSceneView _introSceneView;

    private void Awake()
    {

    }

    private void Start()
    {
        _introSceneView.Initialize();
    }
}
