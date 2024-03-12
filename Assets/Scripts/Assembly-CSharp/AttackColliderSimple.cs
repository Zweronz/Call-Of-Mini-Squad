using CoMDS2;
using UnityEngine;

public class AttackColliderSimple : MonoBehaviour
{
	public DS2ActiveObject.Clique clique;

	public GameObject belong;

	public HitInfo hitInfo;

	public void Start()
	{
		if (GameBattle.m_instance == null)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(other.gameObject);
		IFighter fighter = @object.GetFighter();
		if (fighter != null)
		{
			DS2ActiveObject dS2ActiveObject = @object;
			hitInfo.hitPoint = belong.transform.position;
			hitInfo.repelDirection = @object.GetTransform().position - belong.transform.position;
			hitInfo.source = DS2ObjectStub.GetObject<DS2Object>(belong) as DS2ActiveObject;
			HitResultInfo hitResultInfo = fighter.OnHit(hitInfo);
			if (hitResultInfo.isHit && hitInfo.hitEffect != Defined.EFFECT_TYPE.NONE && @object.m_effectPoint != null)
			{
				BattleBufferManager.Instance.GenerateEffectFromBuffer(hitInfo.hitEffect, @object.m_effectPoint.position);
			}
		}
	}
}
