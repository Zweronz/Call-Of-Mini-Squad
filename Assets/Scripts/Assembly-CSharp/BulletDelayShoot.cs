using CoMDS2;
using UnityEngine;

public class BulletDelayShoot : MonoBehaviour
{
	private Bullet m_bullet;

	private float m_bulletLife;

	public void SetBullet(Bullet bullet)
	{
		m_bullet = bullet;
	}

	public void ShootBulletDelay(float delay_time, float bulletLife)
	{
		m_bulletLife = bulletLife;
		Invoke("EmitBullet", delay_time);
	}

	private void EmitBullet()
	{
		m_bullet.Emit(m_bulletLife);
	}
}
