using UnityEngine;

public class SM_prefabGeneratorCS : MonoBehaviour
{
	public GameObject[] createThis;

	private float rndNr;

	public int thisManyTimes = 3;

	public float overThisTime = 1f;

	public float xWidth;

	public float yWidth;

	public float zWidth;

	public float xRotMax;

	public float yRotMax = 180f;

	public float zRotMax;

	public bool allUseSameRotation;

	private bool allRotationDecided;

	public bool detachToWorld = true;

	private float x_cur;

	private float y_cur;

	private float z_cur;

	private float xRotCur;

	private float yRotCur;

	private float zRotCur;

	private float timeCounter;

	private int effectCounter;

	private float trigger;

	public float prefabOverTime;

	public float delayGenerateTime;

	private float delayCountTime;

	private void Start()
	{
		if (thisManyTimes < 1)
		{
			thisManyTimes = 1;
		}
		trigger = overThisTime / (float)thisManyTimes;
	}

	private void Update()
	{
		delayCountTime += Time.deltaTime;
		if (delayCountTime < delayGenerateTime)
		{
			return;
		}
		timeCounter += Time.deltaTime;
		if (timeCounter > trigger && effectCounter <= thisManyTimes)
		{
			rndNr = Mathf.Floor(Random.value * (float)createThis.Length);
			x_cur = base.transform.position.x + Random.value * xWidth - xWidth * 0.5f;
			y_cur = base.transform.position.y + Random.value * yWidth - yWidth * 0.5f;
			z_cur = base.transform.position.z + Random.value * zWidth - zWidth * 0.5f;
			if (!allUseSameRotation || !allRotationDecided)
			{
				xRotCur = base.transform.rotation.x + Random.value * xRotMax * 2f - xRotMax;
				yRotCur = base.transform.rotation.y + Random.value * yRotMax * 2f - yRotMax;
				zRotCur = base.transform.rotation.z + Random.value * zRotMax * 2f - zRotMax;
				allRotationDecided = true;
			}
			GameObject gameObject = Object.Instantiate(createThis[(int)rndNr], new Vector3(x_cur, y_cur, z_cur), base.transform.rotation) as GameObject;
			gameObject.transform.Rotate(xRotCur, yRotCur, zRotCur);
			if (!detachToWorld)
			{
				gameObject.transform.parent = base.transform;
			}
			gameObject.AddComponent<prefabDestroyerCS>().OverTime = prefabOverTime;
			timeCounter -= trigger;
			effectCounter++;
		}
	}

	private void OnEnable()
	{
		timeCounter = 0f;
		effectCounter = 0;
	}
}
