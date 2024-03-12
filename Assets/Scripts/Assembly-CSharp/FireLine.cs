using UnityEngine;

public class FireLine : MonoBehaviour
{
	public Transform startTrs;

	public Transform endTrs;

	public Transform ptlTrs;

	public float speed = 3f;

	public float accel = 1f;

	public float holdTime = 1f;

	private float _lerp;

	private float _speed;

	private float _hold;

	private void Start()
	{
		_hold = holdTime;
		_speed = speed;
	}

	private void Update()
	{
		if (_lerp <= 1f)
		{
			ptlTrs.localPosition = Vector3.Lerp(startTrs.localPosition, endTrs.localPosition, _lerp);
			_speed += accel * Time.deltaTime;
			_lerp += _speed * Time.deltaTime / Vector3.Distance(startTrs.localPosition, endTrs.localPosition);
			return;
		}
		if (_hold > 0f)
		{
			_hold -= Time.deltaTime;
			return;
		}
		_hold = holdTime;
		_speed = speed;
		_lerp = 0f;
		Transform transform = Object.Instantiate(ptlTrs, startTrs.localPosition, startTrs.rotation) as Transform;
		transform.name = ptlTrs.name;
		transform.parent = startTrs.parent;
		Object.DestroyObject(ptlTrs.gameObject);
		ptlTrs = transform;
	}
}
