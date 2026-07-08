using UnityEngine;
using TMPro;

public class MagnetFallExperiment : MonoBehaviour
{
   [Header("자석 설정")]
    public float magnetStrength = 1.0f; 
    public float minStrength = 0.5f;    
    public float maxStrength = 2.0f;    
    public float strengthChangeSpeed = 0.5f; 

    [Header("UI 연결")]
    public TextMeshProUGUI dataText;    

    private Rigidbody rb;
    private Vector3 startPosition;
    
    private bool isSimulating = false; 
    private bool isAtStart = true;     

    private float currentVelocity = 0f;
    private float currentGravity = 9.81f; 
    
    private const float AccelDecreasePerSecond = 20f; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.useGravity = false;
        }

        startPosition = transform.position;
        ResetToStartPosition();
    }

    void Update()
    {
        // 1. ⭐️ 자석 세기 조절 (시작 위치에 있을 때만 가능하도록 조건 추가)
        if (isAtStart)
        {
            if (Input.GetKey(KeyCode.UpArrow)) 
            {
                magnetStrength += strengthChangeSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.DownArrow)) 
            {
                magnetStrength -= strengthChangeSpeed * Time.deltaTime;
            }
            
            // 자석 세기 제한 (0.5 ~ 2.0)
            magnetStrength = Mathf.Clamp(magnetStrength, minStrength, maxStrength);
        }

        // 2. 토글 로직 (F키)
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isSimulating)
            {
                ResetToStartPosition();
            }
            else
            {
                if (isAtStart)
                {
                    StartFalling();
                }
                else
                {
                    ResetToStartPosition();
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isSimulating && !rb.isKinematic)
        {
            float decreaseAmount = AccelDecreasePerSecond * magnetStrength * Time.fixedDeltaTime;
            currentGravity -= decreaseAmount;

            if (currentGravity < 0f)
            {
                currentGravity = 0f;
            }

            currentVelocity -= currentGravity * Time.fixedDeltaTime;
            rb.linearVelocity = new Vector3(0, currentVelocity, 0);
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (dataText != null)
        {
            float displayAccel = currentGravity > 0f ? -currentGravity : 0f;

            dataText.text = $"Φ : {magnetStrength:F2}\n" +
                            $"a : {displayAccel * -1:F2} m/s²\n" +
                            $"v : {currentVelocity * -1:F2} m/s";
        }
    }

    private void ResetToStartPosition()
    {
        isSimulating = false;
        isAtStart = true; 
        
        rb.isKinematic = true; 
        
        transform.position = startPosition; 
        rb.linearVelocity = Vector3.zero;        
        
        currentVelocity = 0f;
        currentGravity = 9.81f; 
    }

    private void StartFalling()
    {
        isSimulating = true;
        isAtStart = false; 
        
        rb.isKinematic = false;
        
        currentVelocity = 0f;
        currentGravity = 9.81f; 
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isSimulating && collision.gameObject.CompareTag("Ground")) 
        {
            isSimulating = false;
            rb.isKinematic = true; 
            rb.linearVelocity = Vector3.zero;
            
            currentGravity = 0f;
            currentVelocity = 0f;
        }
    }

    public void ResetExperiment4()
    {
        magnetStrength = 1.0f; // 자석 세기 초기화
        ResetToStartPosition(); // 원래 위치로 복구 및 물리 초기화
        UpdateUI();
    }

}