using CoMDS2;
using UnityEngine;

internal class BulletTriggerScript : MonoBehaviour
{
	private float m_lashTriggerTime;

	public void OnTriggerEnter(Collider other)
	{
		Bullet @object = DS2ObjectStub.GetObject<Bullet>(base.gameObject);
		if (other.gameObject.layer == 26)
		{
			@object.Destroy();
		}
		else if (other.gameObject.layer == 18 || other.gameObject.layer == 16 || other.gameObject.layer == 25)
		{
			if (other.gameObject.layer == 25 && @object.GetCreator().clique == DS2ActiveObject.Clique.Computer)
			{
				m_lashTriggerTime = Time.time;
				return;
			}
			if (base.gameObject.layer == 12)
			{
				@object.Destroy();
			}
			else
			{
				if (@object.attribute.damageType == Bullet.BULLET_DAMAGE_TYPE.MULTI)
				{
					int layerMask = ((@object.GetCreator().clique != 0) ? 1536 : 526336);
					Collider[] array = Physics.OverlapSphere(base.transform.position, @object.attribute.damageRange, layerMask);
					Collider[] array2 = array;
					foreach (Collider collider in array2)
					{
						DS2ActiveObject object2 = DS2ObjectStub.GetObject<DS2ActiveObject>(collider.gameObject);
						if (object2 == null)
						{
						}
						HitInfo hitInfo = @object.hitInfo;
						if (collider.gameObject == other.gameObject)
						{
							hitInfo.repelDirection = @object.GetModelTransform().forward;
						}
						else
						{
							hitInfo.repelDirection = object2.GetTransform().position - base.transform.position;
						}
						if (@object.attribute.damageDecayRange > 0f && Vector3.Distance(new Vector3(collider.transform.position.x, 0f, collider.transform.position.z), new Vector3(base.transform.position.x, 0f, base.transform.position.z)) > @object.attribute.damageDecayRange)
						{
							hitInfo = new HitInfo(@object.hitInfo);
							hitInfo.damage = new NumberSection<float>(hitInfo.damage.left * @object.attribute.damageDecayPercent, hitInfo.damage.right * @object.attribute.damageDecayPercent);
						}
						if (object2.OnHit(hitInfo).isHit)
						{
						}
					}
				}
				@object.hitInfo.hitPoint = @object.GetTransform().position;
				@object.HitEffect();
			}
		}
		else if (@object.attribute.damageType == Bullet.BULLET_DAMAGE_TYPE.SINGLE)
		{
			DS2ActiveObject object3 = DS2ObjectStub.GetObject<DS2ActiveObject>(other.gameObject);
			if (object3 != null && object3.Alive())
			{
				@object.TriggerEnter(object3);
			}
		}
		else if (@object.attribute.damageType == Bullet.BULLET_DAMAGE_TYPE.MULTI)
		{
			if ((double)(Time.time - m_lashTriggerTime) < 1.5 / (double)@object.attribute.speed)
			{
				return;
			}
			int layerMask2 = ((@object.GetCreator().clique != 0) ? 1536 : 526336);
			Collider[] array3 = Physics.OverlapSphere(base.transform.position, @object.attribute.damageRange, layerMask2);
			Collider[] array4 = array3;
			foreach (Collider collider2 in array4)
			{
				DS2ActiveObject object4 = DS2ObjectStub.GetObject<DS2ActiveObject>(collider2.gameObject);
				if (!object4.Alive())
				{
					return;
				}
				if (object4 == null)
				{
				}
				HitInfo hitInfo2 = @object.hitInfo;
				if (collider2.gameObject == other.gameObject)
				{
					hitInfo2.repelDirection = @object.GetModelTransform().forward;
				}
				else
				{
					hitInfo2.repelDirection = object4.GetTransform().position - base.transform.position;
				}
				if (@object.attribute.damageDecayRange > 0f && Vector3.Distance(new Vector3(collider2.transform.position.x, 0f, collider2.transform.position.z), new Vector3(base.transform.position.x, 0f, base.transform.position.z)) > @object.attribute.damageDecayRange)
				{
					hitInfo2 = new HitInfo(@object.hitInfo);
					hitInfo2.damage = new NumberSection<float>(hitInfo2.damage.left * @object.attribute.damageDecayPercent, hitInfo2.damage.right * @object.attribute.damageDecayPercent);
				}
				hitInfo2.hitPoint = @object.GetTransform().position;
				if (object4.OnHit(hitInfo2).isHit)
				{
					if (@object.attribute.isPenetrate)
					{
						BattleBufferManager.Instance.GenerateEffectFromBuffer(@object.attribute.effectHit, base.transform.position);
					}
					else
					{
						@object.HitEffect();
					}
				}
			}
		}
		m_lashTriggerTime = Time.time;
	}
}
