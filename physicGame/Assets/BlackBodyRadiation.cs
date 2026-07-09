using UnityEngine;
using TMPro;

public class BlackBodyRadiation : MonoBehaviour
{
   [Header("온도 설정")]
    public float temperature = 100f;       // 초기 온도
    public float minTemp = 30f;            // 최소 온도
    public float maxTemp = 400f;           // 최대 온도
    public float tempChangeSpeed = 50f;    // 온도 변화 속도

    [Header("시각 효과 및 UI 연결")]
    public Light glowLight;                
    public TextMeshProUGUI tempText;       

    private Renderer objRenderer;
    private Material objMaterial;

    public GameObject Panel;
    private ExperimentSelector experimentSelector;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        experimentSelector = Panel.GetComponent<ExperimentSelector>();
        
        if (objRenderer != null)
        {
            objMaterial = objRenderer.material;
            objMaterial.EnableKeyword("_EMISSION"); 
        }
    }

    void Update()
    {
        if (experimentSelector.OnOff == true)
        {
        // 1. 화살표 위/아래 키로 온도 조절
        if (Input.GetKey(KeyCode.UpArrow)) temperature += tempChangeSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow)) temperature -= tempChangeSpeed * Time.deltaTime;
        
        // 온도가 30 ~ 400 사이를 벗어나지 않도록 고정
        temperature = Mathf.Clamp(temperature, minTemp, maxTemp);

        // 2. 온도 구간별 색상 업데이트
        UpdateRadiationColor();

        // 3. UI 텍스트 갱신
        if (tempText != null)
        {
            tempText.text = $"T: {temperature:F1}";
        }
        }
    }

    // ⭐️ 요청하신 온도 구간별 색상을 결정하는 핵심 함수
    Color GetColorFromTemperature(float temp)
    {
        if (temp >= 300f) 
            return new Color(0.2f, 0.5f, 3f); // 300 이상: 파란색 (너무 어둡지 않은 발광 파랑)
        
        else if (temp > 100f) 
            return new Color(0.7f, 0.85f, 3f); // 100 이상 ~ 300 미만: 청백색
        
        else if (temp >= 75f) 
            return Color.white;                // 75 이상 ~ 100 미만: 흰색
        
        else if (temp >= 60f) 
            return new Color(1f, 1f, 0.7f);    // 60 이상 ~ 75 미만: 황백색
        
        else if (temp >= 50f) 
            return Color.yellow;               // 50 이상 ~ 60 미만: 노란색
        
        else if (temp >= 35f) 
            return new Color(2.7f, 0.5f, 0f);    // 35 이상 ~ 50 미만: 주황색
        
        else 
            return Color.red;                  // 35 미만 (30 ~ 35): 빨간색
    }

    void UpdateRadiationColor()
    {
        if (objMaterial == null) return;

        // 위에서 작성한 함수를 통해 현재 온도에 맞는 색상을 가져옵니다.
        Color currentColor = GetColorFromTemperature(temperature);

        // 1. 물체 표면 색상 변경
        objMaterial.color = currentColor;

        // 2. 물체의 발광(Emission) 색상 변경
        float emissionIntensity = 2.0f; 
        objMaterial.SetColor("_EmissionColor", currentColor * emissionIntensity);

        // 3. 주변을 밝히는 조명 색상 변경
        if (glowLight != null)
        {
            glowLight.color = currentColor;
        }
    }

    public void ResetExperiment()
    {
        temperature = 100f; // 초기 온도로 복구
        UpdateRadiationColor();
        
        if (tempText != null)
            tempText.text = $"현재 온도: {temperature:F1}";
    }
}
