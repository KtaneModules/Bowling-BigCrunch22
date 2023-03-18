using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class BowlingScript : MonoBehaviour
{
	public KMAudio Audio;
    public KMBombInfo Bomb;
	public KMBombModule Module;
	
	public SpriteRenderer Pins, Strike;
	public Sprite[] PinsImage, StrikeImage;
	public KMSelectable[] Buttons;
	public TextMesh[] AngleAndPostion;
	public KMSelectable Submit;
	public AudioClip StrikeSound;
	
	List<int> ValidPins = new List<int>();
	int Focus, CorrectAngle, CorrectPosition, CurrentAngle = 0, CurrentPosition = 1;
	
	//Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool ModuleSolved;
	
	void Awake()
	{
		moduleId = moduleIdCounter++;
		for (int i = 0; i < Buttons.Length; i++)
		{
			int Press = i;
			Buttons[i].OnInteract += delegate ()
			{
				ButtonPress(Press);
				return false;
			};
		}
		Submit.OnInteract += delegate () {SubmitAnswer(); return false; };
	}
	
	void Start()
	{
		bool NoneApplied = true;
		Focus = UnityEngine.Random.Range(0,3);
		Pins.sprite = PinsImage[Focus];
		if ((new[] {'A', 'E', 'I', 'O', 'U'}.Any(c => Bomb.GetSerialNumber().Contains(c))))
		{
			NoneApplied = false;
			ValidPins.Add(1);
		}
		
		if (Bomb.GetIndicators().Count() <= 3)
		{
			NoneApplied = false;
			ValidPins.Add(2);
		}
		
		if (Bomb.GetSerialNumberNumbers().Last() % 2 == 0)
		{
			NoneApplied = false;
			ValidPins.Add(3);
		}
		
		if (Bomb.IsPortPresent("Serial"))
		{
			NoneApplied = false;
			ValidPins.Add(4);
		}
		
		if (Bomb.IsIndicatorPresent("FRQ"))
		{
			NoneApplied = false;
			ValidPins.Add(5);
		}
		
		if (Bomb.GetBatteryHolderCount() >= 3)
		{
			NoneApplied = false;
			ValidPins.Add(6);
		}
		
		if (Bomb.IsPortPresent("DVI"))
		{
			NoneApplied = false;
			ValidPins.Add(7);
		}
		
		if (Bomb.GetPortPlates().Where(x => x.Length != 0).Count() >= 2)
		{
			NoneApplied = false;
			ValidPins.Add(8);
		}
		
		if (Bomb.GetBatteryHolderCount(1) > 2)
		{
			NoneApplied = false;
			ValidPins.Add(9);
		}
		
		if (NoneApplied)
		{
			ValidPins.Add(10);
		}
		
		string Valid = "Standing pins: ";
		for (int x = 0; x < ValidPins.Count(); x++)
		{
			Valid += x != ValidPins.Count() - 1 ? ValidPins[x].ToString() + ", " : ValidPins[x].ToString();
		}
		Debug.LogFormat("[Bowling #{0}] {1}", moduleId, Valid);
		string Color = Focus == 0 ? "Red" : Focus == 1 ? "Blue" : "Green";
		Debug.LogFormat("[Bowling #{0}] Color of the bowling pins: {1}", moduleId, Color);
		
		switch (Focus)
		{
			case 0:
			
				if (new[] {10}.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = 0;
					CorrectPosition = 2;
				}
				
				else if (new[] { 3, 7, 1}.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = -5;
					CorrectPosition = 9;
				}
				
				else if (new[] { 6, 7, 4}.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = -20;
					CorrectPosition = 1;
				}
				
				else if (new[] { 5, 3, 6}.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = 45;
					CorrectPosition = 1;
				}
				
				else if (new[] { 2, 5, 9 }.All(c => !ValidPins.Contains(c)))
				{
					CorrectAngle = -40;
					CorrectPosition = 9;
				}

				else if (new[] { 1, 4}.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = 10;
					CorrectPosition = 8;
				}
				
				else if (new[] { 3, 6 }.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = -15;
					CorrectPosition = 1;
				}
				
				else if (new[] { 4, 7 }.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = 30;
					CorrectPosition = 2;
				}
				
				else if (new[] { 5, 8 }.All(c => !ValidPins.Contains(c)))
				{
					CorrectAngle = -35;
					CorrectPosition = 2;
				}
				
				else
				{
					CorrectAngle = 15;
					CorrectPosition = 4;
				}
				
				break;
			case 1:
				if (new[] {10}.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = -5;
					CorrectPosition = 10;
				}
				
				else if (new[] { 3, 7, 9 }.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = -30;
					CorrectPosition = 9;
				}
				
				else if (new[] { 2, 5, 8}.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = 20;
					CorrectPosition = 8;
				}
				
				else if (new[] { 7, 2, 5 }.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = 15;
					CorrectPosition = 5;
				}
				
				else if (new[] { 3, 1, 2 }.All(c => !ValidPins.Contains(c)))
				{
					CorrectAngle = 30;
					CorrectPosition = 6;
				}

				else if (new[] { 2, 7}.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = -20;
					CorrectPosition = 1;
				}
				
				else if (new[] { 6, 3}.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = -30;
					CorrectPosition = 9;
				}
				
				else if (new[] { 8, 9}.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = -45;
					CorrectPosition = 4;
				}
				
				else if (new[] { 9, 8 }.All(c => !ValidPins.Contains(c)))
				{
					CorrectAngle = -10;
					CorrectPosition = 4;
				}
				
				else
				{
					CorrectAngle = 35;
					CorrectPosition = 3;
				}
				break;
			default:
				if (new[] {10}.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = 25;
					CorrectPosition = 10;
				}
				
				else if (new[] { 4, 9, 8}.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = -45;
					CorrectPosition = 8;
				}
				
				else if (new[] { 7, 3, 8 }.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = 0;
					CorrectPosition = 6;
				}
				
				else if (new[] { 1, 6, 5 }.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = -15;
					CorrectPosition = 1;
				}
				
				else if (new[] { 9, 2, 6 }.All(c => !ValidPins.Contains(c)))
				{
					CorrectAngle = 45;
					CorrectPosition = 5;
				}

				else if (new[] { 2, 8 }.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = -35;
					CorrectPosition = 2;
				}
				
				else if (new[] { 1, 4 }.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = 10;
					CorrectPosition = 4;
				}
				
				else if (new[] { 9, 5 }.All(c => ValidPins.Contains(c)))
				{
					CorrectAngle = -20;
					CorrectPosition = 5;
				}
				
				else if (new[] { 3, 7 }.All(c => !ValidPins.Contains(c)))
				{
					CorrectAngle = 35;
					CorrectPosition = 9;
				}
				
				else
				{
					CorrectAngle = -45;
					CorrectPosition = 3;
				}
				break;
		}
		Debug.LogFormat("[Bowling #{0}] The correct angle: {1}° / The correct position: {2}", moduleId, CorrectAngle.ToString(), CorrectPosition.ToString());
	}
	
	void ButtonPress(int Press)
	{
		Buttons[Press].AddInteractionPunch(0.2f);
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, Buttons[Press].transform);
		if (!ModuleSolved)
		{
			switch (Press)
			{
				case 0:
					if (CurrentAngle > -45)
					{
						CurrentAngle = CurrentAngle - 5;
						AngleAndPostion[0].text = "ANGLE: " + CurrentAngle.ToString() + "°";
					}
					break;
				case 1:
					if (CurrentAngle < 45)
					{
						CurrentAngle = CurrentAngle + 5;
						AngleAndPostion[0].text = "ANGLE: " + CurrentAngle.ToString() + "°";
					}
					break;
				case 2:
					if (CurrentPosition > 1)
					{
						CurrentPosition--;
						AngleAndPostion[1].text = "POSITION: " + CurrentPosition.ToString();
					}
					break;
				case 3:
					if (CurrentPosition < 10)
					{
						CurrentPosition++;
						AngleAndPostion[1].text = "POSITION: " + CurrentPosition.ToString();
					}
					break;
				default:
					break;
			}
		}
	}
	
	void SubmitAnswer()
	{
		Submit.AddInteractionPunch(0.2f);
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, Submit.transform);
		if (!ModuleSolved)
		{
			if (CurrentPosition == CorrectPosition && CurrentAngle == CorrectAngle)
			{
				Debug.LogFormat("[Bowling #{0}] You submitted on Angle: {1}° / Position: {2}. That was correct. Module solved.", moduleId, CurrentAngle.ToString(), CurrentPosition.ToString());
				Audio.PlaySoundAtTransform(StrikeSound.name, transform);
				ModuleSolved = true;
				Module.HandlePass();
				Pins.sprite = null;
				Strike.sprite = StrikeImage[Focus];
			}
			
			else
			{
				Debug.LogFormat("[Bowling #{0}] You submitted on Angle: {1}° / Position: {2}. That was incorrect. Module striked.", moduleId, CurrentAngle.ToString(), CurrentPosition.ToString());
				Module.HandleStrike();
			}
		}
	}
	
	//twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"To set your angle/position to a certain number, use the command !{0} angle [-45 to 45] / !{0} position [1 to 10] | To submit, use the command !{0} bowl.";
    #pragma warning restore 414
	
	IEnumerator ProcessTwitchCommand(string command)
    {
		string[] parameters = command.Split(' ');
		
		if (Regex.IsMatch(command, @"^\s*bowl\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
			yield return null;
			Submit.OnInteract();
		}
		
		if (Regex.IsMatch(parameters[0], @"^\s*angle\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
			yield return null;
			if (parameters.Length != 2)
			{
				yield return "sendtochaterror Invalid parameter length. The command was not processed.";
				yield break;
			}
			
			int Out;
			if (!Int32.TryParse(parameters[1], out Out))
			{
				yield return "sendtochaterror Invalid number detected. The command was not processed.";
				yield break;
			}
			
			if (Out < -45 || Out > 45)
			{
				yield return "sendtochaterror Number is not in range. The command was not processed.";
				yield break;
			}
			
			if (Out % 5 != 0)
			{
				yield return "sendtochaterror The number is not a multiple of 0. The command was not processed.";
				yield break;
			}
			
			if (Out == CurrentAngle)
			{
				yield return "sendtochaterror The angle is already set to this number. The command was not processed.";
				yield break;
			}
			
			if (Out > CurrentAngle)
			{
				while (CurrentAngle != Out)
				{
					Buttons[1].OnInteract();
					yield return new WaitForSecondsRealtime(0.1f);
				}
			}
			
			else if (Out < CurrentAngle)
			{
				while (CurrentAngle != Out)
				{
					Buttons[0].OnInteract();
					yield return new WaitForSecondsRealtime(0.1f);
				}
			}
		}
		
		if (Regex.IsMatch(parameters[0], @"^\s*position\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
			yield return null;
			if (parameters.Length != 2)
			{
				yield return "sendtochaterror Invalid parameter length. The command was not processed.";
				yield break;
			}
			
			int Out;
			if (!Int32.TryParse(parameters[1], out Out))
			{
				yield return "sendtochaterror Invalid number detected. The command was not processed.";
				yield break;
			}
			
			if (Out < 1 || Out > 10)
			{
				yield return "sendtochaterror Number is not in range. The command was not processed.";
				yield break;
			}
			
			if (Out == CurrentPosition)
			{
				yield return "sendtochaterror The position is already set to this number. The command was not processed.";
				yield break;
			}
			
			if (Out > CurrentPosition)
			{
				while (CurrentPosition != Out)
				{
					Buttons[3].OnInteract();
					yield return new WaitForSecondsRealtime(0.1f);
				}
			}
			
			else if (Out < CurrentPosition)
			{
				while (CurrentPosition != Out)
				{
					Buttons[2].OnInteract();
					yield return new WaitForSecondsRealtime(0.1f);
				}
			}
		}
	}

	IEnumerator TwitchHandleForcedSolve()
	{
		yield return ProcessTwitchCommand("angle " + CorrectAngle);
		yield return ProcessTwitchCommand("position " + CorrectPosition);
		Submit.OnInteract();
	}
}
