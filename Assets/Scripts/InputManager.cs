using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Image Area;
    public Image Joystick;

    public Vector2 Direction { get; private set; }

    private Vector2 InitialAreaPosition;

    private void Start()
    {
        InitialAreaPosition = Area.transform.position;
    }

    void Update()
    {
        Camera _camera = null;
    
        bool isButtonDown = Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began;
        bool isButtonHold = Input.GetMouseButton(0) || Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Moved;
        bool isButtonUp = Input.GetMouseButtonUp(0) || Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended;
        Vector2 inputPos = new Vector2();
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            inputPos = Input.mousePosition;
        }
        else if (Input.touchCount > 0)
        {
            inputPos = Input.touches[0].position;
        }

        if (isButtonDown)
        {
            Area.gameObject.SetActive(true);
            var rect = (RectTransform) transform;
            Vector2 point;
            bool res = RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, inputPos, _camera, out point);
            if (res)
            {
                Area.transform.position = point + InitialAreaPosition;
            }
        }
        else if (isButtonHold)
        {
            var rect = Area.rectTransform;
            Vector2 point;
            bool res = RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, inputPos, _camera, out point);
            if (res)
            {
                Joystick.transform.position = new Vector3(point.x, point.y, 0) + Area.transform.position;
            }
        }
        else if (isButtonUp)
        {
            Area.gameObject.SetActive(false);
            Joystick.transform.position = Area.transform.position;
        }

        Vector2 diff = Joystick.transform.position - Area.transform.position;
        var radius = Area.rectTransform.sizeDelta.x / 2;
        this.Direction = diff / radius;
        if (diff.magnitude > radius)
        {
            this.Direction = diff.normalized;
            Joystick.transform.position = this.Direction * radius;
            Joystick.transform.position += Area.transform.position;
        }
    }

}
