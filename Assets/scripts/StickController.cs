using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StickController : MonoBehaviour
{
    public float rotateSpeed = 100f;
    public Transform targetStick; // Thanh xanh kiểm tra win
    public GameObject linePrefab; // Prefab để vẽ đoạn nối

    private Transform currentPivot;
    private bool isRotating = false;
    private List<LineRenderer> lines = new List<LineRenderer>();

    void Update()
    {
        if (isRotating && currentPivot != null)
        {
            transform.RotateAround(currentPivot.position, Vector3.forward, -rotateSpeed * Time.deltaTime);
            CheckWinCondition();
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleClickOnPivot();
        }
    }

    void HandleClickOnPivot()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        if (hit != null && hit.CompareTag("Pivot"))
        {
            SetPivot(hit.transform);
        }
    }

    public void SetPivot(Transform pivot)
    {
        if (currentPivot != null && pivot != currentPivot)
        {
            CreateConnection(currentPivot.position, pivot.position);
        }

        currentPivot = pivot;
        isRotating = true;
        Handheld.Vibrate();
    }

    public void StopRotation()
    {
        isRotating = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pivot") && other.transform != currentPivot)
        {
            StopRotation();
            transform.position = GetSnappedPosition(other.transform.position);
        }
    }

    Vector3 GetSnappedPosition(Vector3 pivotPos)
    {
        return pivotPos; // Có thể cộng offset nếu muốn lệch
    }

    void CreateConnection(Vector3 from, Vector3 to)
    {
        GameObject line = Instantiate(linePrefab);
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
        lines.Add(lr);
    }

    void CheckWinCondition()
    {
        if (targetStick == null) return;

        float angleDiff = Quaternion.Angle(transform.rotation, targetStick.rotation);
        if (angleDiff < 1f)
        {
            Debug.Log("🎉 Win!");
            isRotating = false;
            Invoke("LoadNextScene", 1f); // delay 1s trước khi chuyển
        }
    }

    void LoadNextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        if (index + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(index + 1);
        }
        else
        {
            Debug.Log("🎮 Hết màn!");
        }
    }
}
