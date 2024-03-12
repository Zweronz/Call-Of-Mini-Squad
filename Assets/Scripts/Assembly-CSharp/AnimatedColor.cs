using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
public class AnimatedColor : MonoBehaviour
{
	public Color color = Color.white;

	private UIWidget mWidget;

	private void OnEnable()
	{
		mWidget = GetComponent<UIWidget>();
		Update();
	}

	private void Update()
	{
		mWidget.color = color;
	}
}
