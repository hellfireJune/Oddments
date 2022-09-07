/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using InControl;
using System.Reflection;

namespace BindingAPI
{
	class ExampleBinding : ETGModule
	{
		public override void Start()
		{
			//init and add my binding behaviours, need to be in a monobehaviour attached to a gameobject.
			BindingBuilder.Init("examplebinding");
			ETGModMainBehaviour.Instance.gameObject.AddComponent<MyBindingBehaviours>();
		}
		public override void Exit() { }

		public override void Init() { }
		
	}

	class MyBindingBehaviours : MonoBehaviour
	{
		//my bindings
		PlayerAction die;
		PlayerAction speedUp;
		PlayerAction speedDown;

		FieldInfo m_currentGunAngle;

		//my oneaxisinputcontrol, aka can go from a positive to negative value
		OneAxisInputControl speed;
		public void Start()
		{
			m_currentGunAngle = typeof(PlayerController).GetField("m_currentGunAngle", BindingFlags.NonPublic | BindingFlags.Instance);

			//create bindings and default key binding
			die = BindingBuilder.CreateBinding("Die", InControl.Key.P);
			speedUp = BindingBuilder.CreateBinding("Speed Up", defaultmouse:InControl.Mouse.PositiveScrollWheel);
			speedDown = BindingBuilder.CreateBinding("Speed Down", defaultmouse:InControl.Mouse.NegativeScrollWheel);
			
			//creates the one axis input control
			speed = BindingBuilder.CreateOneAxisBinding(speedDown, speedUp);
		}

		void Update()
		{
			//runs code after die was pressed, there is also actions like, WasReleased, IsPressed, etc
			if(die.WasPressed)
			{
				if(GameManager.Instance.PrimaryPlayer != null)
					GameManager.Instance.PrimaryPlayer.healthHaver.ApplyDamage(9999999, Vector2.zero, "Fucking died", ignoreInvulnerabilityFrames: true);
				if (GameManager.Instance.SecondaryPlayer != null)
					GameManager.Instance.SecondaryPlayer.healthHaver.ApplyDamage(9999999, Vector2.zero, "Fucking died", ignoreInvulnerabilityFrames: true);
			}

			//gets the value of speed and applies that knockback
			if (GameManager.Instance.PrimaryPlayer != null)
				GameManager.Instance.PrimaryPlayer.knockbackDoer.ApplyKnockback(BraveMathCollege.DegreesToVector((float)m_currentGunAngle.GetValue(GameManager.Instance.PrimaryPlayer)), speed.Value * 10);
			if (GameManager.Instance.SecondaryPlayer != null)
				GameManager.Instance.SecondaryPlayer.knockbackDoer.ApplyKnockback(BraveMathCollege.DegreesToVector((float)m_currentGunAngle.GetValue(GameManager.Instance.SecondaryPlayer)), speed.Value * 10);
		}
	}

}*/
