using UnityEngine;

public class ExperimentSelector : MonoBehaviour
{
  [Header("실험 설정")]
    public string experimentName = "새로운 실험";
    public Transform cameraViewPoint; // 카메라가 날아갈 목표 지점
    public GameObject experimentUI;   // 이 실험을 시작할 때 켜질 전용 UI
    public GameObject Graph;
    public GameObject Ex1;
    public GameObject Ex2;
    public GameObject Ex3;
    public GameObject Ex4;
    public bool OnOff;
    
    private CameraController cameraController;
    private FreeFallExperiment freeFallExperiment;
    private PhotoelectricEffect photoelectricEffect;
    private BlackBodyRadiation blackBodyRadiation;
    private MagnetFallExperiment magnetFallExperiment;

    

    void Start()
    {
        // 씬에 있는 Main Camera에서 CameraController 스크립트를 자동으로 찾아옵니다.
        if (Camera.main != null)
        {
            cameraController = Camera.main.GetComponent<CameraController>();
            freeFallExperiment = Ex1.GetComponent<FreeFallExperiment>();
            photoelectricEffect = Ex2.GetComponent<PhotoelectricEffect>();
            blackBodyRadiation = Ex3.GetComponent<BlackBodyRadiation>();
            magnetFallExperiment = Ex4.GetComponent<MagnetFallExperiment>();
        }

        if (cameraController == null)
        {
            Debug.LogError($"[{experimentName}] 메인 카메라나 CameraController를 찾을 수 없습니다!");
        }

        // 처음에는 실험 UI를 꺼둡니다.
        if (experimentUI != null)
        {
            experimentUI.SetActive(false);
            Graph.SetActive(false);
            OnOff = false;
        }
    }

    // ⭐️ 매 프레임 T 키 입력을 감지하는 기능을 추가했습니다.
    void Update()
    {
        // 중요: 이 실험 UI가 켜져 있을 때(즉, 유저가 이 실험을 하고 있을 때)만 T 키 입력을 받습니다.
        if (experimentUI != null && experimentUI.activeSelf)
        {
            // 키보드의 T 키가 눌렸는지 감지
            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log($"[{experimentName}] 실험을 종료하고 메인 메뉴로 돌아갑니다.");
                
                // 1. 카메라 컨트롤러에게 메인 메뉴 위치로 돌아가라고 명령합니다.
                if (cameraController != null)
                {
                    cameraController.ReturnToMenu();
                    freeFallExperiment.ResetExperiment();
                    photoelectricEffect.ResetExperiment();
                    blackBodyRadiation.ResetExperiment();
                    magnetFallExperiment.ResetExperiment();
                }

                // 2. 켜져 있던 현재 실험의 UI를 다시 숨깁니다.
                experimentUI.SetActive(false);
                Graph.SetActive(false);
                OnOff = false;
            }
        }
    }

    // 마우스로 이 오브젝트(콜라이더)를 클릭했을 때 (실험 진입)
    void OnMouseDown()
    {
        Debug.Log($"[{experimentName}] 실험 오브젝트가 클릭되었습니다.");

        if (cameraController != null && cameraViewPoint != null)
        {
            cameraController.MoveToTarget(cameraViewPoint);
        }

        if (experimentUI != null)
        {
            experimentUI.SetActive(true);
            Graph.SetActive(true);
            OnOff = true;
        }
    }
}