using UnityEngine;

public class ObjCtrl : MonoBehaviour
{
    private Vector3 mPositionOffset;
    private Vector3 mOriginPosition;

    private void OnMouseDown()
    {
        mOriginPosition = Camera.main.WorldToScreenPoint(transform.position);
        mPositionOffset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + mPositionOffset;
    }

    private Vector3 GetMouseWorldPosition()
    {
        var mousePoint = Input.mousePosition;
        mousePoint.z = mOriginPosition.z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
