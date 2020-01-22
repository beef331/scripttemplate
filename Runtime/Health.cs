using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode, ScriptTemplate.Templated, ExecuteAlways]
public class Health : MonoBehaviour
{

	[SerializeField]
	private int maxHP;

	private int currentHP;

	private bool canTakeDamage = true;

	void Awake()
	{
		currentHP = maxHP;
	}

	public void Damage(int Damage)
	{
		if (canTakeDamage)
		{
			currentHP -= Damage;
			if (currentHP <= 0)
			{
				Destroy(gameObject);
			}
		}
	}

	public void Heal(int amount)
	{
		currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
	}
}
