using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FreeFallExperiment : MonoBehaviour
{
   [Header("높이 조절 설정")]
    public float heightSpeed = 5f;
    public float minY = 1f;
    public float maxY = 30f;

    [Header("UI 설정")]
    public TextMeshProUGUI dataText; // ⭐️ 변수 타입을 TextMeshProUGUI로 변경했습니다.

    private Rigidbody rb;
    private Vector3 startPosition;
    private bool isSimulating = false;

    private float currentVelocity = 0f;
    private float currentAcceleration = 0f;
    private float lastVelocity = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            Debug.LogError("오브젝트에 Rigidbody 컴포넌트가 없습니다! 추가해 주세요.");
            return;
        }

        startPosition = transform.position;
        ResetToStartPosition();
    }

    void Update()
    {
        if (!isSimulating)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                startPosition.y += heightSpeed * Time.deltaTime;
                startPosition.y = Mathf.Clamp(startPosition.y, minY, maxY);
                transform.position = startPosition;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                startPosition.y -= heightSpeed * Time.deltaTime;
                startPosition.y = Mathf.Clamp(startPosition.y, minY, maxY);
                transform.position = startPosition;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isSimulating)
            {
                ResetToStartPosition();
            }
            else
            {
                StartFalling();
            }
        }
    }

    void FixedUpdate()
    {
        if (isSimulating && !rb.isKinematic)
        {
            currentVelocity = -1 * rb.linearVelocity.y;
            currentAcceleration = (currentVelocity - lastVelocity) / Time.fixedDeltaTime;
            lastVelocity = currentVelocity;
        }
        else
        {
            currentVelocity = 0f;
            currentAcceleration = 0f;
            lastVelocity = 0f;
        }

        // 텍스트 업데이트 로직은 기존과 동일하게 잘 작동합니다.
        if (dataText != null)
        {
            dataText.text = $"h : {transform.position.y:F2} m\n" +
                            $"v : {currentVelocity:F2} m/s\n" +
                            $"a : {currentAcceleration:F2} m/s²";
        }
    }

    private void ResetToStartPosition()
    {
        isSimulating = false;
        
        rb.isKinematic = true; 
        rb.useGravity = false;
        
        transform.position = startPosition; 
        rb.linearVelocity = Vector3.zero;        
        rb.angularVelocity = Vector3.zero; 
    }

    private void StartFalling()
    {
        isSimulating = true;
        
        rb.isKinematic = false;
        rb.useGravity = true;   
        
        lastVelocity = 0f; 
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isSimulating && collision.gameObject.CompareTag("Ground")) 
        {
            rb.useGravity = false;
            rb.isKinematic = true; 
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void ResetExperiment1()
    {
        heightSpeed = 5f;
        ResetToStartPosition(); // 원래 위치로 복구 및 물리 초기화
    }
}
