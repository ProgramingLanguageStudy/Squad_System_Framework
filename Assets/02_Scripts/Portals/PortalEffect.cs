using System.Collections;
using System.Linq;
using UnityEngine;

public class PortalEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color portalEffectColor = Color.cyan;
    [SerializeField] private float fadeInSpeed = 0.5f;
    [SerializeField] private float fadeOutSpeed = 0.8f;

    [Header("References")]
    [SerializeField] private Renderer portalRenderer;
    [SerializeField] private ParticleSystem[] effectsPartSystems;
    [SerializeField] private Light portalLight;
    [SerializeField] private Transform symbolTF;
    [SerializeField] private AudioSource portalAudio, flashAudio;

    private Material _portalMat;
    private Material _portalEffectMat;
    private float _currentTransitionF = 0f;
    private float _maxLightIntensity;
    private Vector3 _symbolStartPos;

    private Coroutine _activeTransitionCor;
    private Coroutine _symbolCor;
    private bool _isActivated;

    private void Awake()
    {
        // 머티리얼 초기화 (원본 수정을 방지하기 위해 인스턴스화된 머티리얼 사용)
        Material[] mats = portalRenderer.materials;
        _portalMat = mats[0];
        _portalEffectMat = mats[1];

        _symbolStartPos = symbolTF.localPosition;
        _maxLightIntensity = portalLight.intensity;

        // 초기 상태: 꺼짐
        SetEffectState(0f);
    }

    public void TogglePortal(bool activate)
    {
        if (_isActivated == activate) return;
        _isActivated = activate;

        // 기존 연출 중단
        if (_activeTransitionCor != null) StopCoroutine(_activeTransitionCor);

        // 새로운 연출 시작
        _activeTransitionCor = StartCoroutine(TransitionRoutine(activate));

        if (activate)
        {
            foreach (var ps in effectsPartSystems) ps.Play();
            if (flashAudio != null) flashAudio.Play();
            if (portalAudio != null) portalAudio.Play();

            if (_symbolCor != null) StopCoroutine(_symbolCor);
            _symbolCor = StartCoroutine(SymbolMovementRoutine());
        }
        else
        {
            foreach (var ps in effectsPartSystems) ps.Stop();
        }
    }

    private IEnumerator TransitionRoutine(bool activate)
    {
        float target = activate ? 1f : 0f;
        float speed = activate ? fadeInSpeed : fadeOutSpeed;

        while (!Mathf.Approximately(_currentTransitionF, target))
        {
            _currentTransitionF = Mathf.MoveTowards(_currentTransitionF, target, Time.deltaTime * speed);
            SetEffectState(_currentTransitionF);
            yield return null;
        }

        if (!activate)
        {
            if (portalAudio != null) portalAudio.Stop();
            if (_symbolCor != null) StopCoroutine(_symbolCor);
        }
    }

    private void SetEffectState(float f)
    {
        _portalMat.SetColor("_EmissionColor", portalEffectColor * f);
        _portalMat.SetFloat("_EmissionStrength", f);

        _portalEffectMat.SetColor("_ColorMain", portalEffectColor);
        _portalEffectMat.SetFloat("_PortalFade", f * 0.4f);

        portalLight.color = portalEffectColor;
        portalLight.intensity = _maxLightIntensity * f;

        if (portalAudio != null) portalAudio.volume = f * 0.8f;
    }

    private IEnumerator SymbolMovementRoutine()
    {
        Vector3 targetPos = _symbolStartPos;
        while (true)
        {
            targetPos = _symbolStartPos + new Vector3(0, Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f));
            float elapsed = 0;
            Vector3 startPos = symbolTF.localPosition;

            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * 2f;
                symbolTF.localPosition = Vector3.Lerp(startPos, targetPos, elapsed);
                yield return null;
            }
        }
    }
}