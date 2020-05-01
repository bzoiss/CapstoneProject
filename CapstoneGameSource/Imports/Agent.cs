using System;

interface Agent
{
	void Reproduce();
	void Die();
	void ChangeSatisfaction(double s);
	void ChangeHealth(double h);
	void Behavior(float delta);
}
