using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Input2 : MonoBehaviour
{
	public static Touch GetTouch(int i)
	{
		return (Application.isMobilePlatform) ? Input.GetTouch(i) : InputHelper.GetTouches()[0];
	}

	public static int touchCount
	{
		get
		{
			return (Application.isMobilePlatform) ? Input.touchCount : InputHelper.GetTouches().Count;
		}
	}

	public static Touch[] touches
	{
		get
		{
			return (Application.isMobilePlatform) ? Input.touches : InputHelper.GetTouches().ToArray();
		}
	}

	public static Vector2 acceleration
	{
		get
		{
			if (Application.isMobilePlatform)
			{
				return Input.acceleration;
			}
			return new Vector2(Input.GetAxis("Horizontal"), 0);
		}
	}

	public class InputHelper : MonoBehaviour
	{
	    private static TouchCreator lastFakeTouch;

	    public static List<Touch> GetTouches()
	    {
	        List<Touch> touches = new List<Touch>();
	        touches.AddRange(Input.touches);
	        if (lastFakeTouch == null) lastFakeTouch = new TouchCreator();
	        if (Input.GetMouseButtonDown(0))
	        {
	            lastFakeTouch.phase = TouchPhase.Began;
	            lastFakeTouch.deltaPosition = new Vector2(0, 0);
	            lastFakeTouch.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
	            lastFakeTouch.fingerId = 0;
	        }
	        else if (Input.GetMouseButtonUp(0))
	        {
	            lastFakeTouch.phase = TouchPhase.Ended;
	            Vector2 newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
	            lastFakeTouch.deltaPosition = newPosition - lastFakeTouch.position;
	            lastFakeTouch.position = newPosition;
	            lastFakeTouch.fingerId = 0;
	        }
	        else if (Input.GetMouseButton(0))
	        {
	            lastFakeTouch.phase = TouchPhase.Moved;
	            Vector2 newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
	            lastFakeTouch.deltaPosition = newPosition - lastFakeTouch.position;
	            lastFakeTouch.position = newPosition;
	            lastFakeTouch.fingerId = 0;
	        }
	        else
	        {
	            lastFakeTouch = null;
	        }
	        if (lastFakeTouch != null) touches.Add(lastFakeTouch.Create());
	        return touches;
	    }

	}

	public class TouchCreator
	{
	    static BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;

	    static Dictionary<string, FieldInfo> fields;

	    object touch;

	    public float deltaTime { get { return ((Touch)touch).deltaTime; } set { fields["m_TimeDelta"].SetValue(touch, value); } }

	    public int tapCount { get { return ((Touch)touch).tapCount; } set { fields["m_TapCount"].SetValue(touch, value); } }

	    public TouchPhase phase { get { return ((Touch)touch).phase; } set { fields["m_Phase"].SetValue(touch, value); } }

	    public Vector2 deltaPosition { get { return ((Touch)touch).deltaPosition; } set { fields["m_PositionDelta"].SetValue(touch, value); } }

	    public int fingerId { get { return ((Touch)touch).fingerId; } set { fields["m_FingerId"].SetValue(touch, value); } }

	    public Vector2 position { get { return ((Touch)touch).position; } set { fields["m_Position"].SetValue(touch, value); } }

	    public Vector2 rawPosition { get { return ((Touch)touch).rawPosition; } set { fields["m_RawPosition"].SetValue(touch, value); } }

	    public Touch Create()
	    {
	        return (Touch)touch;
	    }

	    public TouchCreator()
	    {
	        touch = new Touch();
	    }

	    static TouchCreator()
	    {
	        fields = new Dictionary<string, FieldInfo>();
	        foreach (var f in typeof(Touch).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
	        {
	            fields.Add(f.Name, f);
	            Debug.Log("name: " + f.Name);
	        }
	    }
	}
}
