using UnityEngine;

public class TimeCalculagraph : MonoBehaviour
{
	private int id = -1;

	private bool running;

	private Calculagraph_CallBack CallBack_TimeOut;

	private Calculagraph_CallBackUp CallBack_Update;

	private float m_fMaxCountDownTimer = -1f;

	private float m_fMaxCountDownTime = 3f;

	public int ID
	{
		get
		{
			return id;
		}
	}

	public void Init(int _id, float MaxTime, Calculagraph_CallBack CallBack_End, Calculagraph_CallBackUp CallBack_Up)
	{
		id = _id;
		CallBack_TimeOut = CallBack_End;
		CallBack_Update = CallBack_Up;
		running = true;
		m_fMaxCountDownTimer = 0f;
		m_fMaxCountDownTime = MaxTime;
	}

	private void Update()
	{
		if (running && m_fMaxCountDownTimer >= 0f)
		{
			m_fMaxCountDownTimer += Time.deltaTime;
			if (CallBack_Update != null)
			{
				CallBack_Update(m_fMaxCountDownTime - m_fMaxCountDownTimer);
			}
			if (m_fMaxCountDownTimer >= m_fMaxCountDownTime && CallBack_TimeOut != null)
			{
				CallBack_TimeOut();
				running = false;
				m_fMaxCountDownTimer = -1f;
				TimeManager.Instance.DestroyCalculagraph(this);
			}
		}
	}
}
