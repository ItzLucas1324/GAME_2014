using UnityEngine;
using Unity.Cinemachine;

public class CameraScript : CinemachineExtension
{
    private float minY;
    private bool initialized = false;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body && Application.isPlaying)
        {
            if (!initialized)
            {
                minY = vcam.transform.position.y;
                initialized = true;
            }

            Vector3 pos = state.RawPosition;
            pos.y = Mathf.Max(pos.y, minY);
            state.RawPosition = pos;
        }
    }
}
