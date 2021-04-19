
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(AROcclusionManager))]
public class AROcclusionQualityController : MonoBehaviour
{ 
    private AROcclusionManager arOcclusionManager;

    void Awake()
    {
        arOcclusionManager = GetComponent<AROcclusionManager>();
    }

    public void ChangeQualityTo(TMP_Dropdown option)
	{
        switch(option.value)
		{
            case 0:
                ChangeQualityTo(EnvironmentDepthMode.Fastest);
                break;

            case 1:
                ChangeQualityTo(EnvironmentDepthMode.Medium);
                break;

            case 2:
                ChangeQualityTo(EnvironmentDepthMode.Best);
                break;

            default:
                break;
		}
	}

    public void ChangeQualityTo(EnvironmentDepthMode environmentDepthMode)
    {
        arOcclusionManager.requestedEnvironmentDepthMode = environmentDepthMode;
    }

    public EnvironmentDepthMode GetCurrentDepthMode()
    {
        return arOcclusionManager.requestedEnvironmentDepthMode;
    }
}