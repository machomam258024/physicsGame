using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [Header("카메라 세팅")]
    public Transform mainMenuPos; // 기본 메뉴 화면에서의 카메라 위치/회전
    public float transitionSpeed = 3f; // 카메라 이동 속도

    private Transform targetTransform; 
    private bool isMoving = false; 

    // ⭐️ 사용자님 말씀대로 게임이 '시작'될 때 유니티가 이 함수를 최초로 실행합니다!
    void Start()
    {
        if (mainMenuPos != null)
        {
            // [선택 1] 시작하자마자 메뉴 위치에 카메라를 '정확히 배치'하고 싶을 때 (추천)
            transform.position = mainMenuPos.position;
            transform.rotation = mainMenuPos.rotation;

            // [선택 2] 만약 시작할 때 카메라가 어딘가에서 메뉴 위치로 부드럽게 '날아오게' 하고 싶다면
            // 아래 두 줄의 주석(//)을 풀고 위 두 줄을 지우시면 됩니다.
            // targetTransform = mainMenuPos;
            // isMoving = true;
        }
        else
        {
            Debug.LogWarning("Main Camera의 인스펙터 창에서 'MainMenuPos'가 비어 있습니다! 오브젝트를 연결해 주세요.");
        }
    }

    // 마우스 클릭 시 다른 스크립트(ExperimentSelector)에서 이 함수를 부릅니다.
    public void MoveToTarget(Transform newTarget)
    {
        targetTransform = newTarget; 
        isMoving = true;             // Update의 이동 로직 스위치 ON
    }

    // 메인 메뉴로 돌아갈 때 부르는 함수
    public void ReturnToMenu()
    {
        targetTransform = mainMenuPos;
        isMoving = true;
    }

    // 평소엔 가만히 있다가, isMoving이 true가 되면 매 프레임 부드럽게 이동합니다.
    void Update()
    {
        if (isMoving && targetTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * transitionSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetTransform.rotation, Time.deltaTime * transitionSpeed);

            // 도착 확인
            if (Vector3.Distance(transform.position, targetTransform.position) < 0.01f &&
                Quaternion.Angle(transform.rotation, targetTransform.rotation) < 0.1f)
            {
                transform.position = targetTransform.position;
                transform.rotation = targetTransform.rotation;
                isMoving = false; // 도착했으니 스위치 OFF
            }
        }
    }
}