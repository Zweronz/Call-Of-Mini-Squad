using UnityEngine;

[ExecuteInEditMode]
public class AnimatedAlpha : MonoBehaviour
{
	[Range(0f, 1f)]
	public float alpha = 1f;

	private UIWidget mWidget;

	private UIPanel mPanel;

	private void OnEnable()
	{
		mWidget = GetComponent<UIWidget>();
		mPanel = GetComponent<UIPanel>();
		Update();
	}

	private void Update()
	{
		if (mWidget != null)
		{
			mWidget.alpha = alpha;
		}
		if (mPanel != null)
		{
			mPanel.alpha = alpha;
		}
	}
}
