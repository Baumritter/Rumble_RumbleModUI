# Rumble ModUI by Baumritter

---

## Usage
### Example
	using RumbleModUI
	
	RumbleModUI.UI UI;
	Mod Mod = new Mod();
	
	public override void OnLateInitializeMelon()
	{
		base.OnLateInitializeMelon();
	
		UI = RumbleModUIClass.UI_Obj;
	
		Mod.SetName("Test");
		Mod.SetVersion("0.0.0");
		Mod.SetFolder("Test");
		Mod.AddToList("Description", ModSetting.AvailableTypes.Other, "", 0, "Does Nothing.");
		Mod.AddToList("Some Setting", ModSetting.AvailableTypes.Integer, "0", 0, "Does Nothing.");
		Mod.AddToList("Another Setting", ModSetting.AvailableTypes.Boolean, "true", 1, "Does Nothing.");
		Mod.AddToList("That Setting", ModSetting.AvailableTypes.Boolean, "false", 1, "Does Nothing.");
		Mod.GetFromFile();
	}
	
	public override void OnUpdate()
	{
		base.OnUpdate();
	
		if (UI.GetInit() && !Mod.GetUIStatus())
		{
			UI.AddMod(Mod);
		}
	
	}
	
### Explanation


	using RumbleModUI
	
	RumbleModUI.UI UI;
	Mod Mod = new Mod();
	
Includes the Mod and creates the minimum amount of objects that are necessary for the UI to function.
The Objects can be renamed as you please.


	UI = RumbleModUIClass.UI_Obj;

This attaches the existing instance of the UI to the object. This is mandatory and should be done in the initialization


 	Mod.SetName("Test");
	Mod.SetVersion("0.0.0");
	
Sets the Name and Version of the mod that will be used in the UI and the Settings File. Both of these have to be set.


	Mod.SetFolder("Test");
	
Sets the name of the Folder in UserData. The existance of the folder will be validated, and the folder will be created if necessary, upon usage by the UI. 


	Mod.AddToList("Description", ModSetting.AvailableTypes.Other, "", 0, "Does Nothing.");
	
The Setting by the name of Description (case sensitive) has some custom logic attached which will do a few things:
- Instead display the current value in the input window it will display the amount of settings that exist, not including the Description Setting.
- It will not be included in the created Settings File.
The Description Setting serves purely as a way to describe the mod in the UI. Therefore the second, third and fourth argument are without function and can be disregarded.


	Mod.AddToList("Some Setting", ModSetting.AvailableTypes.Integer, "0", 0, "Does Nothing.");
	
Adds a Setting with the first argument as its title. 
The second Argument describes if the Setting is either Boolean, Integer or Other. This will purely determine whether input validation will be applied or not.
The input validation is as follows:
- Boolean: Only accepts "true" or "false" as input
- Integer: Only accepts natural numbers
- Other: Does not validate the input. Can be anything you please.
The third argument describes the default value of the setting which will be applied if no settings file exists yet.
The fourth argument describes if the setting is part of a group. Will be ellaborated on further down.
The fifth argument is a description of the setting that will be described in the description box.


	Mod.GetFromFile();
	
This tries to get the last values for each of the defined settings from the saved file. If the file doesn't exist, the default values remain.


	if (UI.GetInit() && !Mod.GetUIStatus())
	{
		UI.AddMod(Mod);
	}
	
Checks whether the UI is active and accepting input and if the Mod is already added to the UI Instance.
If both return true then the mod will be added to the UI.

---

## Public Methods

All methods that are not mentioned here should not be used. 
Using them anyway will lead to unintended consequences.

### RumbleModUI.UI

GetInit
- Returns true/false depending on the status of the UI
- Can be used to delay the AddMod method until it can be used properly
ShowUI
- Enables the GameObject therefore toggling the visibility to be visible
- Is linked by default to the "F10" key
- Will recenter the UI based on window size 
HideUI
- Disables the GameObject therefore toggling the visibility to be invisible
- Is linked by default to the "F10" key
IsUIVisible
- Returns the whether the UI is currently visible or not.

AddMod
- Will add the Input to the List of the UI.
- If the same Mod is already in the list it will be skipped.
RemoveMod 
- Will remove a mod from the list if the input matches any entry in the list.

### RumbleModUI.Mod

SetName
- Sets the ModName variable.
SetVersion
- Sets the ModVersion variable.
SetFolder
- Sets the Folder variable.
- This is where the Settings file will be saved if the SubFolder variable is not set.
SetSubFolder
- Sets the SubFolder variable.
- This is where the Settings file will be saved if the SubFolder variable is set. The SubFolder will be put inside the (Main-)Folder.

GetName
- Returns the ModName variable.
GetVersion
- Returns the ModVersion variable.
GetFolder
- Returns the Folder variable.
GetSubFolder
- Returns the SubFolder variable.
GetReadStatus
- Returns the IsFileLoaded variable.
- Will be true if the GetFromFile method has been run.
GetSaveStatus
- Returns the IsSaved variable.
- Will be true of the File has been saved by the user.
- Will be false if the ConfirmSave method has been run.
GetUIStatus
- Returns the IsAdded variable.
- See example for usage.

ConfirmSave
- Sets the IsSaved variable to false.
- Is intended to toggle IsSaved back off after executing code upon the user saving.
AddToList
- Will add the Input to the List of the Mod.
GetFromFile
- Will get the values from the file.
- If no file exists then nothing will be done.
- The method will check if the ModName and ModVersion are included in the first line of the file. If not then nothing will be read.

### RumbleModUI.ModSetting

GetName
- Returns the Name variable.
GetDescription
- Returns the Description variable.
GetValueType
- Returns the ValueType variable.
GetValue
- Returns the Value variable.

---

## Variables

All variables that are not mentioned here should not be used. 
Using / Changing them anyway will lead to unintended consequences.

### RumbleModUI.Mod

SettingsFile
- Contains the name and file extension of the settings file.

TempSettings
- Contains the ModSettings that are currently being displayed/edited by the UI.
SavedSettings 
- Contains the a copy of the ModSettings in the SavedSettings that are only written once the user saves.
- These will be written over the TempSettings if the user discards.

### RumbleModUI.ModSettings

Nothing public here.

Quick Explanation:
- Name: Name of the Setting
- ValueType: 
	- Boolean: Only accepts "true" or "false" as input
	- Integer: Only accepts natural numbers
	- Other: Does not validate the input. Can be anything you please.
- Value: Is a string. All values are saved as strings and have to be parsed for usage in your mod.
- Description: Describes the setting for the user.
- LinkGroup:
	- Only has a use if the ValueType is Boolean.
	- For every Setting that has the same value of LinkGroup:
	  Changing the value of a setting to true will change all the other ones to false.
	  
For any further ellaboration contact Baumritter in the modding discord and I will get back to you in 1-2 Business years (jk).