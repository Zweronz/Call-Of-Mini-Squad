using CoMDS2;
using UnityEngine;

public class UtilUITeamExhibitionInfo : MonoBehaviour
{
	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private new UILabel name;

	[SerializeField]
	private UILabel intro;

	[SerializeField]
	private UIImageButton weaponBtn;

	[SerializeField]
	private UITexture weaponIcon;

	[SerializeField]
	private UIImageButton skillBtn;

	[SerializeField]
	private UITexture skillIcon;

	[SerializeField]
	private UILabel heroType;

	private Player gPlayer;

	public Camera exhibitionCamera;

	private float keyReleaseTime = -1f;

	public void Enter(int heroIndex)
	{
		DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(heroIndex);
		TeamData teamData = DataCenter.Save().GetTeamData();
		PlayerData playerData = DataCenter.Save().GetPlayerData(heroIndex);
		Vector3 position = new Vector3(0f, 10000f);
		Player player2 = CharacterBuilder.CreatePlayerByCharacterType(teamData, playerData, position, new Quaternion(0f, 0f, 0f, 0f), 9);
		Weapon weapon = WeaponBuilder.CreateWeaponPlayer(player2.getWeaponType(), playerData.weaponLevel);
		weapon.DoReload();
		player2.AddWeapon(0, weapon);
		player2.UseWeapon(0);
		player2.UpdateAnimationSpeed();
		player2.CurrentController = true;
		player2.SetHalo();
		exhibitionCamera.enabled = true;
		exhibitionCamera.transform.position = new Vector3(player2.GetTransform().position.x, player2.GetTransform().position.y + 8f, player2.GetTransform().position.z - 5f);
		exhibitionCamera.transform.LookAt(player2.GetTransform());
		gPlayer = player2;
		gPlayer.GetTransform().Rotate(Vector3.up, -135f);
		Player obj = gPlayer;
		Vector3 forward = gPlayer.GetTransform().forward;
		gPlayer.MoveDirection = forward;
		obj.FaceDirection = forward;
	}

	public void Update()
	{
		if (gPlayer == null)
		{
			return;
		}
		gPlayer.Update(Time.deltaTime);
		exhibitionCamera.transform.position = new Vector3(gPlayer.GetTransform().position.x, gPlayer.GetTransform().position.y + 8f, gPlayer.GetTransform().position.z - 5f);
		exhibitionCamera.transform.LookAt(gPlayer.GetTransform());
		BattleBufferManager.Instance.UpdateInteractObjListForUIExhibition(Time.deltaTime);
		if (keyReleaseTime != -1f)
		{
			keyReleaseTime += Time.deltaTime;
			if (keyReleaseTime >= 0.8f)
			{
				keyReleaseTime = -1f;
				gPlayer.SetFire(false, gPlayer.FaceDirection);
			}
		}
	}

	public void Exit()
	{
		if (gPlayer != null)
		{
			Object.Destroy(gPlayer.GetGameObject());
			gPlayer = null;
			BattleBufferManager.Instance.RemoveAllObjFromActiveObjectListForUIExhibition();
		}
		exhibitionCamera.enabled = false;
		Time.timeScale = 1f;
	}

	public void UpdateHeroInfo(string heroIcon, string heroName, string heroIntro, string heroType, Texture heroWeapon, Texture heroSkill)
	{
		icon.spriteName = heroIcon;
		name.text = heroName;
		intro.text = heroIntro;
		this.heroType.text = heroType;
		weaponIcon.mainTexture = heroWeapon;
		skillIcon.mainTexture = heroSkill;
	}

	public void HandlePressFiringBtn()
	{
		keyReleaseTime = -1f;
		gPlayer.SetFire(true, gPlayer.FaceDirection);
	}

	public void HandeleReleaseFiringBtn()
	{
		keyReleaseTime = 0f;
	}

	public void HandleUseSkillBtn()
	{
		gPlayer.UseSkill(0);
	}
}
