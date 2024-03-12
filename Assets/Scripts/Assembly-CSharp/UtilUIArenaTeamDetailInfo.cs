using System.Collections.Generic;
using CoMDS2;
using UnityEngine;

public class UtilUIArenaTeamDetailInfo : MonoBehaviour
{
	public class TargetPlayerDetailDataInfo
	{
		public string teamName = string.Empty;

		public int teamCombat;

		public string userId = string.Empty;

		public int heroIndex;

		public int weaponStar;

		public int weaponLevel;

		public int weaponMaxLevel;

		public int skillStar;

		public int siteNum;

		public int state;

		public int helmsEquipIndex;

		public int helmsEquipLevel;

		public int armorEquipIndex;

		public int armorEquipLevel;

		public int ornamentEquipIndex;

		public int ornamentEquipLevel;

		public int combat;
	}

	[SerializeField]
	protected UILabel teamName;

	[SerializeField]
	protected UILabel teamCombat;

	[SerializeField]
	protected UILabel heroName;

	[SerializeField]
	protected UILabel heroCombat;

	[SerializeField]
	protected UILabel health;

	[SerializeField]
	protected UILabel damage;

	[SerializeField]
	protected UILabel defense;

	[SerializeField]
	protected UILabel drduction;

	[SerializeField]
	protected UILabel critRate;

	[SerializeField]
	protected UILabel dodge;

	[SerializeField]
	protected UILabel resilience;

	[SerializeField]
	protected UITexture weaponIcon;

	[SerializeField]
	protected GameObject weaponStarsGO;

	[SerializeField]
	protected UITexture skillIcon;

	[SerializeField]
	protected GameObject skillStarsGO;

	[SerializeField]
	protected UITexture helmsIcon;

	[SerializeField]
	protected GameObject helmsStarsGO;

	[SerializeField]
	protected UITexture armorIcon;

	[SerializeField]
	protected GameObject armorStarsGO;

	[SerializeField]
	protected UITexture ornamentsIcon;

	[SerializeField]
	protected GameObject ornamentsStarsGO;

	public GameObject selectPlayerEffectPrefab;

	protected GameObject[] gTeamModels;

	public UtilUIArenaTeamModelControl arenaTeamModelControl;

	public void Init(TargetPlayerDetailDataInfo[] infos)
	{
		CreateTeamModels(infos);
		UtilUIArenaTeamModelControl.ITEMINFO iTEMINFO = arenaTeamModelControl.InitTargets(gTeamModels);
		arenaTeamModelControl.SetTurningFinishedDelegate(HandleTurningFinished);
		arenaTeamModelControl.TurnFinished(iTEMINFO.go.GetComponent<TweenPosition>());
		arenaTeamModelControl.SetVisable(true);
		UpdateTeamInfoUI(infos[0].teamName, infos[0].teamCombat + string.Empty);
	}

	public void CreateTeamModels(TargetPlayerDetailDataInfo[] infos)
	{
		if (gTeamModels != null)
		{
			for (int i = 0; i < gTeamModels.Length; i++)
			{
				if (gTeamModels[i] != null)
				{
					Object.DestroyImmediate(gTeamModels[i]);
				}
			}
		}
		gTeamModels = null;
		gTeamModels = new GameObject[infos.Length];
		for (int j = 0; j < infos.Length; j++)
		{
			DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(infos[j].heroIndex);
			DataConf.WeaponData weaponDataByType = DataCenter.Conf().GetWeaponDataByType(heroDataByIndex.weaponType);
			GameObject gameObject = Resources.Load("Models/NewCharacters/" + heroDataByIndex.modelFileName) as GameObject;
			Object original = gameObject.transform.Find("Model").gameObject;
			GameObject gameObject2 = Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject2.transform.Rotate(0f, 180f, 0f);
			Transform transform = null;
			transform = gameObject2.transform.Find("Bip01/Spine_00/Bip01 Spine/Spine_0/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 Prop1");
			Weapon weapon = WeaponBuilder.CreateWeaponPlayer(heroDataByIndex.weaponType, 0);
			weapon.Mount(gameObject2.transform, transform, null);
			if (heroDataByIndex.characterType == Player.CharacterType.Lili || heroDataByIndex.characterType == Player.CharacterType.Clint || heroDataByIndex.characterType == Player.CharacterType.Eva || heroDataByIndex.characterType == Player.CharacterType.Bourne)
			{
				((WeaponPistol)weapon).GetLeftGun().SetActive(false);
			}
			string text = DataCenter.Conf().GetNewCharacterAnim(heroDataByIndex.name, "Idle").name;
			gameObject2.GetComponent<Animation>()[text].wrapMode = WrapMode.Loop;
			gameObject2.GetComponent<Animation>().Play(text);
			gTeamModels[j] = gameObject2;
		}
	}

	public void SetVisable(bool bShow)
	{
		base.gameObject.SetActive(bShow);
	}

	public void SelectPlayer(TargetPlayerDetailDataInfo data)
	{
		DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(data.heroIndex);
		DataConf.WeaponData weaponDataByType = DataCenter.Conf().GetWeaponDataByType(heroDataByIndex.weaponType);
		DataConf.HeroSkillInfo heroSkillInfo = DataCenter.Conf().GetHeroSkillInfo(heroDataByIndex.characterType, 0, 0);
		DataConf.EquipData equipDataByIndex = DataCenter.Conf().GetEquipDataByIndex(data.helmsEquipIndex);
		DataConf.EquipData equipDataByIndex2 = DataCenter.Conf().GetEquipDataByIndex(data.armorEquipIndex);
		DataConf.EquipData equipDataByIndex3 = DataCenter.Conf().GetEquipDataByIndex(data.ornamentEquipIndex);
		List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
		list.Add(new KeyValuePair<int, int>(data.helmsEquipIndex, data.helmsEquipLevel));
		list.Add(new KeyValuePair<int, int>(data.armorEquipIndex, data.armorEquipLevel));
		list.Add(new KeyValuePair<int, int>(data.ornamentEquipIndex, data.ornamentEquipLevel));
		int num = PlayerData.CalacHp(data.heroIndex, list);
		float num2 = PlayerData.CalacDamage(data.heroIndex, data.weaponLevel, data.weaponStar, data.weaponMaxLevel, list);
		float num3 = PlayerData.CalacDefense(data.heroIndex, list);
		int num4 = PlayerData.CalacHit(data.heroIndex, list);
		int num5 = PlayerData.CalacCritRate(data.heroIndex, list);
		int num6 = PlayerData.CalacDodge(data.heroIndex, list);
		UpdateHeroDetailDataInfoUI(heroDataByIndex.name, data.combat + string.Empty, num + string.Empty, num2 + string.Empty, num3 + "%", string.Empty, num5 + string.Empty, num6 + string.Empty, string.Empty);
		UpdateWSInfoUI(UIUtil.GetEquipTextureMaterial(weaponDataByType.iconFileName).mainTexture, data.weaponStar, UIUtil.GetEquipTextureMaterial(heroSkillInfo.fileName).mainTexture, data.skillStar);
		UpdateEquipmentInfoUI(UIUtil.GetEquipTextureMaterial(equipDataByIndex.fileName).mainTexture, data.helmsEquipLevel, UIUtil.GetEquipTextureMaterial(equipDataByIndex2.fileName).mainTexture, data.armorEquipLevel, UIUtil.GetEquipTextureMaterial(equipDataByIndex3.fileName).mainTexture, data.ornamentEquipLevel);
	}

	public void UpdateTeamInfoUI(string _teamName, string _teamCombat)
	{
		teamName.text = _teamName;
		teamCombat.text = _teamCombat;
	}

	public void UpdateHeroDetailDataInfoUI(string _heroName, string _heroCombat, string _health, string _damage, string _defense, string _dreduction, string _critR, string _dodge, string _resilience)
	{
		heroName.text = _heroName;
		heroCombat.text = _heroCombat;
		health.text = _health;
		damage.text = _damage;
		defense.text = _defense;
		drduction.text = _dreduction + "%";
		critRate.text = _critR + "%";
		dodge.text = _dodge + "%";
		resilience.text = _resilience + "%";
	}

	public void UpdateWSInfoUI(Texture wT, int wCount, Texture sT, int sCount)
	{
		weaponIcon.mainTexture = wT;
		UIUtil.ShowStars(weaponStarsGO.transform, wCount);
		skillIcon.mainTexture = sT;
		UIUtil.ShowStars(skillStarsGO.transform, sCount);
	}

	public void UpdateEquipmentInfoUI(Texture hT, int hC, Texture aT, int aC, Texture oT, int oC)
	{
		helmsIcon.mainTexture = hT;
		UIUtil.ShowStars(helmsStarsGO.transform, hC);
		armorIcon.mainTexture = aT;
		UIUtil.ShowStars(armorStarsGO.transform, hC);
		ornamentsIcon.mainTexture = oT;
		UIUtil.ShowStars(ornamentsStarsGO.transform, hC);
	}

	public void HandleCloseBtnClicked()
	{
		SetVisable(false);
		arenaTeamModelControl.SetVisable(false);
	}

	public void HandleTurningFinished(UtilUIArenaTeamModelControl.ITEMINFO ii)
	{
		if (ii.nowSiteIndex == 0)
		{
			UtilUIStandbyPlayersInfo.SetPlayerModelHaloEffectVisable(false, ii.go, selectPlayerEffectPrefab);
			UtilUIStandbyPlayersInfo.SetPlayerModelOutLineEffectVisable(false, ii.go, 0.02f);
		}
		if (ii.nextSiteIndex == 0)
		{
			UtilUIStandbyPlayersInfo.SetPlayerModelHaloEffectVisable(true, ii.go, selectPlayerEffectPrefab);
			UtilUIStandbyPlayersInfo.SetPlayerModelOutLineEffectVisable(true, ii.go, 0.02f);
			SelectPlayer(UIConstant.gLSTargetDetailData[ii.siteIndexInTeam]);
		}
	}

	public void HandleDragIconsFinished(Vector2 delta)
	{
		if (delta.x < 0f)
		{
			arenaTeamModelControl.Turning(true);
		}
		else if (delta.x > 0f)
		{
			arenaTeamModelControl.Turning(false);
		}
	}
}
