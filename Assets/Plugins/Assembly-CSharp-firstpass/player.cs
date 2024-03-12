using UnityEngine;

public class player : MonoBehaviour
{
	public GameObject[] enemyObj;

	public KeyCode[] keys_Attack = new KeyCode[3]
	{
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3
	};

	private int index;

	public AnimationClip[] animations_Attack;

	public AnimationClip[] animations_Hurt;

	public bool thirdPersonView;

	public KeyCode[] keys_Move = new KeyCode[4]
	{
		KeyCode.W,
		KeyCode.S,
		KeyCode.A,
		KeyCode.D
	};

	public AnimationClip animations_Idle;

	public AnimationClip[] animations_Run;

	public float runSpeed = 0.2f;

	private Vector2 moveDirectionN = Vector2.up;

	public Transform cameraAimTrs;

	public Camera cmr;

	public float mouseMoveSensitivity = 6f;

	private float cmrToPlayerDis;

	private void Start()
	{
		base.GetComponent<Animation>()[animations_Idle.name].layer = -1;
		base.GetComponent<Animation>().Play(animations_Idle.name);
		base.GetComponent<Animation>()[animations_Run[0].name].layer = -1;
		base.GetComponent<Animation>()[animations_Run[1].name].layer = -1;
		base.GetComponent<Animation>()[animations_Run[2].name].layer = -1;
		base.GetComponent<Animation>()[animations_Run[3].name].layer = -1;
		moveDirectionN = new Vector2(base.transform.forward.x, base.transform.forward.z);
		if (cmr != null)
		{
			cmrToPlayerDis = (cmr.transform.position - cameraAimTrs.position).magnitude;
		}
		Screen.lockCursor = true;
	}

	private void OnHit()
	{
		if (enemyObj == null)
		{
			return;
		}
		for (int i = 0; i < enemyObj.Length; i++)
		{
			if (enemyObj[i].GetComponent<Animation>()[animations_Hurt[index].name] != null)
			{
				enemyObj[i].GetComponent<Animation>().CrossFade(animations_Hurt[index].name);
			}
		}
	}

	private void Update()
	{
		for (int i = 0; i < keys_Attack.Length; i++)
		{
			if (Input.GetKeyDown(keys_Attack[i]))
			{
				index = i;
				base.GetComponent<Animation>().Stop(animations_Attack[i].name);
				base.GetComponent<Animation>().CrossFade(animations_Attack[i].name);
			}
		}
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			index = 0;
			base.GetComponent<Animation>().Stop(animations_Attack[0].name);
			base.GetComponent<Animation>().CrossFade(animations_Attack[0].name);
		}
		Vector2 to = new Vector2(0f, 0f);
		if (Input.GetKey(keys_Move[0]))
		{
			to += new Vector2(base.transform.forward.x, base.transform.forward.z) * runSpeed;
			base.GetComponent<Animation>().CrossFade(animations_Run[0].name);
		}
		if (Input.GetKey(keys_Move[1]))
		{
			to -= new Vector2(base.transform.forward.x, base.transform.forward.z) * runSpeed;
			base.GetComponent<Animation>().CrossFade(animations_Run[1].name);
		}
		if (Input.GetKey(keys_Move[2]))
		{
			to -= new Vector2(base.transform.forward.z, 0f - base.transform.forward.x) * runSpeed;
			base.GetComponent<Animation>().CrossFade(animations_Run[2].name);
		}
		if (Input.GetKey(keys_Move[3]))
		{
			to += new Vector2(base.transform.forward.z, 0f - base.transform.forward.x) * runSpeed;
			base.GetComponent<Animation>().CrossFade(animations_Run[3].name);
		}
		if (Mathf.Abs(to.x) > 0.01f || Mathf.Abs(to.y) > 0.01f)
		{
			if (thirdPersonView)
			{
				moveDirectionN = to.normalized;
				Vector2 from = new Vector2(base.transform.forward.x, base.transform.forward.z);
				from = Vector2.Lerp(from, to, 0.6f);
				base.transform.forward = new Vector3(from.x, base.transform.forward.y, from.y);
				base.transform.position += new Vector3(to.x, 0f, to.y) * runSpeed;
			}
			else
			{
				moveDirectionN = to.normalized;
				base.transform.position += new Vector3(to.x, 0f, to.y) * runSpeed;
			}
		}
		else
		{
			base.GetComponent<Animation>().CrossFade(animations_Idle.name);
		}
		if (cmr != null)
		{
			Vector3 eulerAngles = cmr.transform.eulerAngles;
			eulerAngles += new Vector3((0f - Input.GetAxis("Mouse Y")) * mouseMoveSensitivity, Input.GetAxis("Mouse X") * mouseMoveSensitivity, 0f);
			if (eulerAngles.x < 0f)
			{
				eulerAngles.x = 0f;
			}
			if (eulerAngles.x > 80f)
			{
				eulerAngles.x = 80f;
			}
			cmr.transform.eulerAngles = eulerAngles;
			cmr.transform.position = cameraAimTrs.position - cmr.transform.forward * cmrToPlayerDis;
			if (!thirdPersonView)
			{
				Vector3 forward = cameraAimTrs.position - cmr.transform.position;
				forward.y = 0f;
				base.transform.forward = forward;
			}
		}
	}
}
