using UnityEngine;

namespace CoMDS2
{
	public class Mine : DS2ActiveObject
	{
		private float m_explodeRadius = 2.5f;

		private EffectParticleContinuous m_mineSmoke;

		public Mine()
		{
			base.objectType = Defined.OBJECT_TYPE.OBJECT_TYPE_OTHERS;
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_BOMB_1, 1);
		}

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			base.clique = ((layer == 15) ? Clique.Computer : Clique.Player);
			m_mineSmoke = GetTransform().GetComponentInChildren<EffectParticleContinuous>();
			m_mineSmoke.gameObject.SetActive(false);
		}

		public void SetInfo(HitInfo info, float explode_radius)
		{
			base.hitInfo = info;
			base.hitInfo.source = this;
			m_explodeRadius = explode_radius;
		}

		public void SetMine()
		{
			m_mineSmoke.gameObject.SetActive(true);
			m_mineSmoke.StartEmit();
			if (GameBattle.m_instance != null)
			{
				GameBattle.m_instance.AddObjToInteractObjectList(this);
			}
			else
			{
				BattleBufferManager.Instance.AddObjToInteractObjectListForUIExhibition(this);
			}
		}

		public void Detonate()
		{
			GameBattle.m_instance.SetCameraQuake(Defined.CameraQuakeType.Quake_D);
			BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_BOMB_1, new Vector3(GetTransform().position.x, 1f, GetTransform().position.z), 2f);
			int layerMask = ((base.Layer != 14) ? 1536 : 526336);
			Collider[] array = Physics.OverlapSphere(GetTransform().position, m_explodeRadius, layerMask);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(collider.gameObject);
				if (@object == null)
				{
				}
				@object.OnHit(base.hitInfo);
			}
			m_mineSmoke.gameObject.SetActive(false);
			GetGameObject().SetActive(false);
			if (GameBattle.m_instance != null)
			{
				GameBattle.m_instance.AddObjToInteractObjectList(this);
			}
			else
			{
				BattleBufferManager.Instance.AddToInteractObjectNeedDeleteListForUIExhibition(this);
			}
		}
	}
}
