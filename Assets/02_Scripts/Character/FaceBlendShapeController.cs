using UnityEngine;

/// <summary>
/// 얼굴 BlendShape(표정)를 스크립트로 제어. PicoChan 등 SkinnedMeshRenderer + BlendShape 있는 캐릭터용.
/// face_base 같은 오브젝트의 SkinnedMeshRenderer를 인스펙터에 넣고, Set~ / SetWeight(name, value) 호출.
/// </summary>
public class FaceBlendShapeController : MonoBehaviour
{
    [SerializeField] [Tooltip("BlendShape가 붙은 얼굴 메쉬 (예: PicoChan의 face_base)")]
    private SkinnedMeshRenderer _faceRenderer;

    /// <summary>이름으로 가중치 설정. 0~100. Inspector에 보이는 이름 그대로 (Blink, mouth_A, Joy 등).</summary>
    public void SetWeight(string blendShapeName, float value)
    {
        if (_faceRenderer == null || _faceRenderer.sharedMesh == null) return;
        int index = _faceRenderer.sharedMesh.GetBlendShapeIndex(blendShapeName);
        if (index < 0)
            index = _faceRenderer.sharedMesh.GetBlendShapeIndex("blendShape1." + blendShapeName);
        if (index >= 0)
            _faceRenderer.SetBlendShapeWeight(index, Mathf.Clamp01(value / 100f) * 100f);
    }

    /// <summary>한 번에 리셋 (모든 BlendShape 0).</summary>
    public void ResetAll()
    {
        if (_faceRenderer == null || _faceRenderer.sharedMesh == null) return;
        for (int i = 0; i < _faceRenderer.sharedMesh.blendShapeCount; i++)
            _faceRenderer.SetBlendShapeWeight(i, 0f);
    }

    // 편의 메서드 (PicoChan 기준 이름)
    public void SetBlink(float value) => SetWeight("Blink", value);
    public void SetMouthA(float value) => SetWeight("mouth_A", value);
    public void SetMouthI(float value) => SetWeight("mouth_I", value);
    public void SetMouthU(float value) => SetWeight("mouth_U", value);
    public void SetMouthE(float value) => SetWeight("mouth_E", value);
    public void SetMouthO(float value) => SetWeight("mouth_O", value);
    public void SetJoy(float value) => SetWeight("Joy", value);
    public void SetAngry(float value) => SetWeight("Angry", value);
    public void SetSorrow(float value) => SetWeight("Sorrow", value);
    public void SetFun(float value) => SetWeight("Fun", value);
}
