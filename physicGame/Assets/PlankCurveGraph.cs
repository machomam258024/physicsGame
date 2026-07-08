using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlankCurveGraph : MonoBehaviour
{
   [Header("연결 설정")]
    public BlackBodyRadiation blackBody; // 기존에 만든 흑체 복사 스크립트를 연결합니다.

    [Header("그래프 설정")]
    public int resolution = 100;         // 곡선을 부드럽게 만들기 위한 점의 개수
    public float graphWidth = 10f;       // 그래프의 가로 길이 (파장)
    public float graphHeight = 5f;       // 그래프의 세로 최대 높이 (세기)
    
    // 이전 프레임의 온도를 저장하여, 온도가 바뀔 때만 연산하도록 최적화
    private float lastTemperature = -1f; 
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
        
        // 그래프 선의 두께 설정 (필요에 따라 인스펙터에서 수정 가능)
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        
        // 라인 렌더러가 UI처럼 월드 좌표의 영향을 받지 않도록 로컬 공간 사용
        lineRenderer.useWorldSpace = false;
    }

    void Update()
    {
        if (blackBody == null) return;

        // 온도가 변했을 때만 그래프를 다시 그립니다 (최적화)
        if (Mathf.Abs(blackBody.temperature - lastTemperature) > 0.1f)
        {
            DrawPlanckCurve(blackBody.temperature);
            lastTemperature = blackBody.temperature;
        }
    }

    void DrawPlanckCurve(float temperature)
    {
        float[] intensities = new float[resolution];
        float maxIntensity = 0f;

        // 1. 파장(x)에 따른 플랑크 곡선의 세기(y) 계산
        for (int i = 0; i < resolution; i++)
        {
            // 파장을 0.1 ~ 15.0 범위로 매핑 (0은 분모가 되므로 제외)
            float wavelength = Mathf.Lerp(0.1f, 15.0f, (float)i / (resolution - 1));
            
            // 실험실 온도(30~400)에서 그래프가 역동적으로 움직이도록 비례 상수(1000)를 적용한 플랑크 공식 근사치
            float c2 = 1000f; 
            float intensity = (1f / Mathf.Pow(wavelength, 5)) / (Mathf.Exp(c2 / (wavelength * temperature)) - 1f);
            
            intensities[i] = intensity;

            // 정점(Peak)을 찾기 위해 가장 큰 세기 값을 기록
            if (intensity > maxIntensity)
            {
                maxIntensity = intensity;
            }
        }

        // 2. 계산된 값들을 Line Renderer의 좌표로 변환하여 점 찍기
        for (int i = 0; i < resolution; i++)
        {
            float xPos = Mathf.Lerp(0, graphWidth, (float)i / (resolution - 1));
            
            float yPos = 0f;
            if (maxIntensity > 0)
            {
                // 화면 밖으로 그래프가 뚫고 나가지 않도록 최대 높이에 맞춰 정규화(Normalize)
                yPos = (intensities[i] / maxIntensity) * graphHeight;
            }

            // (x축: 파장, y축: 세기) 좌표에 점을 배치
            lineRenderer.SetPosition(i, new Vector3(xPos, yPos, 0));
        }
    }
}
