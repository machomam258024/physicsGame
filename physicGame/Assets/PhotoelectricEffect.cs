using UnityEngine;
using TMPro;
using System;

public class PhotoelectricEffect : MonoBehaviour
{
   [Header("레이저 설정")]
    public LineRenderer laserLine;
    public float maxDistance = 20f;
    public GameObject ray;

    [Header("효과 설정")]
    public ParticleSystem electronEmission;
    public float baseEmissionRate = 20f;

    [Header("UI 텍스트 연결")]
    public TextMeshProUGUI frequencyText;  // 인스펙터에서 연결
    public TextMeshProUGUI intensityText;  // 인스펙터에서 연결

    [Header("물리 변수")]
    [Range(0.5f, 3.0f)] public float frequency = 1.0f;
    [Range(0.5f, 2.0f)] public float lightIntensity = 1.0f;

    public GameObject Panel;
    private ExperimentSelector experimentSelector;
    private bool noray;

    void Start()
    {
        ray.SetActive(noray);
        if (electronEmission != null) electronEmission.Stop();
        experimentSelector = Panel.GetComponent<ExperimentSelector>();
    }

    void Update()
    {
        if (experimentSelector.OnOff == true)
        {
        if (noray == false)
        {
            ray.SetActive(true);
        }
        // 1. 입력 처리
        if (Input.GetKey(KeyCode.UpArrow)) frequency += 0.5f * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow)) frequency -= 0.5f * Time.deltaTime;
        frequency = Mathf.Clamp(frequency, 0.5f, 3.0f);

        if (Input.GetKey(KeyCode.RightArrow)) lightIntensity += 0.5f * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow)) lightIntensity -= 0.5f * Time.deltaTime;
        lightIntensity = Mathf.Clamp(lightIntensity, 0.5f, 2.0f);

        // 2. 로직 및 UI 업데이트
        UpdateParticleLogic();
        UpdateUI(); 
        DrawLaser();
        }
    }

    void UpdateParticleLogic()
    {
        if (electronEmission == null) return;
        var main = electronEmission.main;
        var emission = electronEmission.emission;

        if (frequency < 2.0f)
        {
            if (electronEmission.isPlaying) electronEmission.Stop();
        }
        else
        {
            if (!electronEmission.isPlaying) electronEmission.Play();
            main.startSpeed = (frequency - 2.0f) / 0.1f;
            emission.rateOverTime = baseEmissionRate * lightIntensity;
        }
    }

    // ⭐️ UI 갱신 함수
    void UpdateUI()
    {
        if (frequencyText != null)
            frequencyText.text = $"f: {frequency:F1} Hz";
        
        if (intensityText != null)
            intensityText.text = $"I: {lightIntensity:F1}";
    }

    void DrawLaser()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        laserLine.SetPosition(0, transform.position);

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            laserLine.SetPosition(1, hit.point);
            if (electronEmission != null)
            {
                electronEmission.transform.position = hit.point;
                electronEmission.transform.rotation = Quaternion.LookRotation(hit.normal);
            }
        }
        else
        {
            laserLine.SetPosition(1, transform.position + transform.forward * maxDistance);
        }
    }

    public void ResetExperiment()
    {
        frequency = 1.0f;
        lightIntensity = 1.0f;
        
        if (electronEmission != null) 
        {
            electronEmission.Stop();
            electronEmission.Clear(); // 뿜어져 나오던 입자 즉시 제거
        }
        
        UpdateUI();
    }
}
