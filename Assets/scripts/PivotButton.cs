using UnityEngine;

public class PivotButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameObject stick = GameObject.FindGameObjectWithTag("Stick");
        if (stick != null)
        {
            StickController controller = stick.GetComponent<StickController>();
            controller.SetPivot(transform);
        }
    }
}
