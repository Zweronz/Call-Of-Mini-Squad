using System.Reflection;
using CoMDS2;
using LitJson;
using UnityEngine;

public class ExcuteAnimationEvent : MonoBehaviour
{
	public enum EffectPosition
	{
		Default = 0,
		Center = 1,
		Front = 2
	}

	public GameObject belong;

	public void AttackCollideEnable(AttackCollider.AttackColliderType type = AttackCollider.AttackColliderType.Normal)
	{
		Character character = DS2ObjectStub.GetObject<DS2Object>(belong) as Character;
		character.SetAttackCollider(true, type);
	}

	public void AttackCollideDisable(AttackCollider.AttackColliderType type = AttackCollider.AttackColliderType.Normal)
	{
		Character character = DS2ObjectStub.GetObject<DS2Object>(belong) as Character;
		character.SetAttackCollider(false, type);
	}

	public void Thud(string jsonParam)
	{
		Character character = DS2ObjectStub.GetObject<DS2Object>(belong) as Character;
		JsonData jsonData = JsonMapper.ToObject(jsonParam);
		int num = (int)jsonData["radius"];
		int layerMask = (int)jsonData["layerMask"];
		Collider[] array = Physics.OverlapSphere(character.GetTransform().position, num, layerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (!(collider.gameObject == belong))
			{
				DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(collider.gameObject);
				HitInfo hitInfo = character.GetHitInfo();
				Vector3 vector = new Vector3(character.GetTransform().position.x, @object.GetTransform().position.y, character.GetTransform().position.z);
				hitInfo.repelDirection = @object.GetTransform().position - vector;
				@object.OnHit(hitInfo);
			}
		}
		GameBattle.m_instance.SetCameraQuake(Defined.CameraQuakeType.Quake_D);
	}

	public void PlayerEffect(string name)
	{
		string[] array = name.Split(',');
		if (array.Length > 0)
		{
			string text = array[0];
			bool bind = true;
			if (array.Length > 1)
			{
				bind = bool.Parse(array[1]);
			}
			DS2ActiveObject dS2ActiveObject = DS2ObjectStub.GetObject<DS2ActiveObject>(belong) as Character;
			dS2ActiveObject.effectPlayManager.PlayEffect(text, bind);
		}
	}

	public void PlayerEffectEvent(string name)
	{
		string[] array = name.Split(',');
		if (array.Length > 0)
		{
			string text = array[0];
			bool bind = true;
			if (array.Length > 1)
			{
				bind = bool.Parse(array[1]);
			}
			DS2ActiveObject dS2ActiveObject = DS2ObjectStub.GetObject<DS2ActiveObject>(belong) as Character;
			dS2ActiveObject.effectPlayManager.PlayEffect(text, bind);
		}
	}

	public void InvokeMethod(string methodName)
	{
		Character @object = DS2ObjectStub.GetObject<Character>(belong);
		MethodInfo method = @object.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		method.Invoke(@object, null);
	}

	public void PlayerAudioEvent(string name)
	{
		Character @object = DS2ObjectStub.GetObject<Character>(belong);
		@object.audioManager.PlayAudio(name);
	}

	public void CameraQuake(Defined.CameraQuakeType quakeId)
	{
		if (GameBattle.m_instance != null)
		{
			GameBattle.m_instance.SetCameraQuake(quakeId);
		}
	}
}
