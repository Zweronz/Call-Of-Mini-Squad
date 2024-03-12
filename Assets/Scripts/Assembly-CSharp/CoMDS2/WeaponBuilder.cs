namespace CoMDS2
{
	public class WeaponBuilder
	{
		public static Weapon CreateWeaponPlayer(Weapon.WeaponType type, int level)
		{
			Weapon weapon = null;
			switch (type)
			{
			case Weapon.WeaponType.Magnum:
			case Weapon.WeaponType.Pistol_04:
				weapon = new WeaponPistol(type, WeaponPistol.PistolType.Scatter);
				break;
			case Weapon.WeaponType.Virus_Eva:
				weapon = new WeaponPistol(type, WeaponPistol.PistolType.Buff);
				break;
			case Weapon.WeaponType.Sniper_03:
				weapon = new WeaponSniper(type);
				break;
			case Weapon.WeaponType.Grenade_Claire:
			case Weapon.WeaponType.Grenade_01:
				weapon = new WeaponGrenade(type);
				break;
			case Weapon.WeaponType.Ninjia_Fire_03:
				weapon = new WeaponNinjia(type, PlayerNinjia.NinjiaType.Fire);
				break;
			case Weapon.WeaponType.Ninjia_Ice_02:
				weapon = new WeaponNinjia(type, PlayerNinjia.NinjiaType.Ice);
				break;
			case Weapon.WeaponType.ShotGun_01:
			case Weapon.WeaponType.ShotGun_02:
			case Weapon.WeaponType.ShotGun_04:
				weapon = new WeaponShotgun(type);
				break;
			case Weapon.WeaponType.Machinegun2:
			case Weapon.WeaponType.Machinegun_Tanya:
			case Weapon.WeaponType.Machinegun4:
				weapon = new WeaponMachinegun(type);
				break;
			case Weapon.WeaponType.Laser_02:
				weapon = new WeaponLaser(type);
				break;
			case Weapon.WeaponType.Sniper_Wesker:
				weapon = new WeaponSniperWesker(type);
				break;
			case Weapon.WeaponType.Pistol_Lili:
				weapon = new WeaponPistolDoctor(type, WeaponPistol.PistolType.Buff);
				break;
			case Weapon.WeaponType.Grenade_Shepard:
				weapon = new WeaponGrenadeShepard(type);
				break;
			}
			weapon.Initialize(level);
			return weapon;
		}

		public static Weapon CreateWeaponNPC(string name)
		{
			return null;
		}
	}
}
