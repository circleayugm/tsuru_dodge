using UnityEngine;

public class CursorController : MonoBehaviour
{
    private IMouseInteractive hoveredObject;
    private IMouseInteractive selectedObject;
    private bool isHovering;

    private void Update()
    {
        if (Camera.main == null) return;

        CalculateCursorData();
        CheckMouseDown();
        CheckLeftMouseUp();
        CheckMouseDrag();
    }

    private void CalculateCursorData()
    {
        RaycastHit hitInfo = new RaycastHit();
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        bool rayHitSomething = Physics.Raycast(ray, out hitInfo);

        bool hasRigidBodyAndIsInteractive = (hitInfo.rigidbody != null) && (hitInfo.rigidbody.GetComponent<IMouseInteractive>() != null);
        bool hasColliderAndIsInteractive = (hitInfo.collider != null) && (hitInfo.collider.GetComponent<IMouseInteractive>() != null);
        isHovering = (rayHitSomething) && (hasRigidBodyAndIsInteractive || hasColliderAndIsInteractive);

        if (isHovering)
        {
            if (hitInfo.rigidbody != null) hoveredObject = hitInfo.collider.attachedRigidbody.GetComponent<IMouseInteractive>();
            else if (hitInfo.collider != null) hoveredObject = hitInfo.collider.GetComponent<IMouseInteractive>();
            else hoveredObject = null;
        }
        else
        {
            hoveredObject = null;
        }
    }

    private void CheckMouseDown()
    {
        if (!isHovering || !Input.GetMouseButtonDown(0)) return;
        selectedObject = hoveredObject;
        hoveredObject.OnCursorDown();
    }

    private void CheckLeftMouseUp()
    {
        if (selectedObject == null || !Input.GetMouseButtonUp(0)) return;
        selectedObject.OnCursorUp();
        selectedObject = null;
    }

    private void CheckMouseDrag()
    {
        if (selectedObject != null && Input.GetMouseButton(0)) selectedObject.OnCursorDrag();
    }
}