using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    Animator animator;
    CharacterController characterController;
    public float speed = 6.0f;
    public float roatationSpeed = 25f; // แก้คำสะกดจาก roatation เป็น rotation (ถ้าต้องการ)
    public float jumpSpeed = 7.5f;
    public float gravity = 20.0f;
    Vector3 inputVec;
    Vector3 targetDirection;
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        Time.timeScale = 1;
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        float x = -(Input.GetAxisRaw("Vertical"));
        float z = Input.GetAxisRaw("Horizontal");
        inputVec = new Vector3(x, 0, z);

        // ตรวจสอบชื่อใน Animator ของคุณด้วยว่าสะกด "Input X" (ตัว n) ตรงกันหรือไม่
        animator.SetFloat("Input X", z);
        animator.SetFloat("Input Z", -(x));

        if (x != 0 || z != 0) {
            animator.SetBool("Moving", true);
            animator.SetBool("Running", true);
        } else {
            animator.SetBool("Moving", false);
            animator.SetBool("Running", false);
        }

        if (characterController.isGrounded) {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= speed;
        }
        
        // 1. คำนวณทิศทางจากกล้องก่อน
        getCameraRelative(); 
        
        // 2. สั่งหมุนตัวละคร (เพิ่มเช็กป้องกัน Vector Zero)
        RotateTowardMovementDirection();

        // 3. สั่งเคลื่อนที่
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void RotateTowardMovementDirection() {
        // แก้ไข: เช็กที่ targetDirection ว่าไม่ใช่ค่าว่าง ก่อนจะสั่ง LookRotation
        if (targetDirection != Vector3.zero) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * roatationSpeed);
        }
    }

    void getCameraRelative() {
        if (Camera.main == null) return;

        Transform cameraTransform = Camera.main.transform;
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        
        // คำนวณทิศทางเป้าหมาย
        targetDirection = (h * right) + (v * forward);
    }
}