
using UnityEngine;

public class PlayerMove01 : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float smoothInputSpeed = 10f;

    private Animator animator;
    private Vector3 smoothInput = Vector3.zero;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(h, 0f, v).normalized;

        smoothInput = Vector3.Lerp(smoothInput, inputDir, smoothInputSpeed*Time.deltaTime);

        if(smoothInput.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(smoothInput);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed*Time.deltaTime);  
        }

        transform.Translate(smoothInput * moveSpeed * Time.deltaTime, Space.World);

        animator.SetFloat("Blend", smoothInput.magnitude);

    }
}
