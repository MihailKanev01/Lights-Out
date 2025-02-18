using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    public static PostProcessingManager Instance { get; private set; }

    [SerializeField] private PostProcessVolume postProcessVolume;
    private DepthOfField depthOfField;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        
        if (postProcessVolume.profile.TryGetSettings(out depthOfField))
        {
            depthOfField.active = false;
        }
    }

    public void EnableBlur(bool enable)
    {
        if (depthOfField != null)
        {
            depthOfField.active = enable;
        }
    }
}
