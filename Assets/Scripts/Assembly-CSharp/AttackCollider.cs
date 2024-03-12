using CoMDS2;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
	public enum AttackColliderType
	{
		Normal = 0,
		Dash = 1,
		Bat = 2,
		Grab = 3,
		Thud = 4,
		Drill = 5
	}

	public DS2ActiveObject.Clique clique;

	public GameObject belong;

	public AttackColliderType type;

	public int index;

	public void Start()
	{
		if (GameBattle.m_instance == null)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (belong == null)
		{
			return;
		}
		Character character = DS2ObjectStub.GetObject<DS2Object>(belong) as Character;
		if (type == AttackColliderType.Grab)
		{
			Character @object = DS2ObjectStub.GetObject<Character>(other.gameObject);
			character.AddToGrabList(@object);
			return;
		}
		DS2ActiveObject object2 = DS2ObjectStub.GetObject<DS2ActiveObject>(other.gameObject);
		IFighter fighter = object2.GetFighter();
		if (fighter == null)
		{
			return;
		}
		DS2ActiveObject dS2ActiveObject = object2;
		HitInfo hitInfo = character.GetHitInfo();
		hitInfo.hitPoint = character.GetTransform().position;
		hitInfo.repelDirection = object2.GetTransform().position - character.GetTransform().position;
		if (type == AttackColliderType.Dash && character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
		{
			GameBattle.m_instance.SetCameraQuake(Defined.CameraQuakeType.Quake_D);
			float num = Vector3.Angle(hitInfo.repelDirection, character.GetModelTransform().forward);
			if (num < 15f)
			{
				float num2 = 0.75f;
				hitInfo.repelDirection = new Vector3(hitInfo.repelDirection.x + num2, hitInfo.repelDirection.y, hitInfo.repelDirection.z + num2);
				num = Vector3.Angle(hitInfo.repelDirection, character.GetModelTransform().forward);
				if (num < 15f)
				{
					num2 = -1f;
					hitInfo.repelDirection = new Vector3(hitInfo.repelDirection.x + num2, hitInfo.repelDirection.y, hitInfo.repelDirection.z + num2);
				}
			}
		}
		else if (type == AttackColliderType.Bat && character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
		{
			GameBattle.m_instance.SetCameraQuake(Defined.CameraQuakeType.Quake_A);
		}
		hitInfo.source = character;
		if (fighter.OnHit(hitInfo).isHit && hitInfo.hitEffect != Defined.EFFECT_TYPE.NONE && object2.m_effectPoint != null)
		{
			BattleBufferManager.Instance.GenerateEffectFromBuffer(hitInfo.hitEffect, object2.m_effectPoint.position);
		}
	}
}
