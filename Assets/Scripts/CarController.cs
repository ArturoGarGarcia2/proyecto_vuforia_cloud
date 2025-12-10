using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = .5f;
    public float rotationSpeed = 50f;

    private bool moveForward = false;
    private bool rotateRight = false;
    private bool rotateLeft = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (moveForward)
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if(rotateRight)
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        if(rotateLeft)
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
    }

    public void SetMoveForward(bool mf) => moveForward = mf;
    public void SetRotateLeft(bool rl) => rotateLeft = rl;
    public void SetRotateRight(bool rr) => rotateRight = rr;
}
