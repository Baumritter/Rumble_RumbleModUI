# Version 2.1.2
- Updated MelonLoader Version to 0.6.4
# Version 2.1.1
- Fixed some minor bugs
# Version 2.1.0
- Fixed some major bugs
# Version 2.0.0
- Updated to MelonLoader 0.6.2
# Version 1.5.5
- Fixed Delay subclass
# Version 1.5.4
- Deactivated an internal override that always set the park size to 2.
# Version 1.5.3
- Internal Changes.
# Version 1.5.2
- Refined the Dictionary for Modnames ingame <-> Modnames on Thunderstore (again)
- Added a Dictionary for Authors 
# Version 1.5.1
- Added new state for Version checker
		- "?" -> Mod has not been found on Thunderstore or check is still running.
- Refined the Dictionary for Modnames ingame <-> Modnames on Thunderstore
# Version 1.5.0
- Added Setting to make Dropdown Selection persistent
- Window Position will only get reset if it is out of bounds
- Changed the background color of the togglebox to improve readability
- Added some public calls for mod creators (Class: Baum_API)
- Added basic networking using Photon RPCs (Credits to nickklmao for the PunRPC workaround)
- Added a call for the Thunderstore API 
- Moved some functions into the Baum_API class for public use
- Added a new theme 
- Reworked old themes (especially the High Contrast Theme)
- Added a version checker.
	- This will add an entry to the Menu for every installed mod
	- A symbol will be added to the start of the name depending on status
		- ">" -> Mod is older than the version in the Thunderstore
		- "=" -> Mod is the same as the version in the Thunderstore
		- "<" -> Mod is newer than the version in the Thunderstore
- Added a lot of documentation to public methods
# Version 1.4.5
- Fixed some broken code
- Revised the ReadMe/Changelog/Documentation
# Version 1.4.4
- Added some obnoxious log messages when the mod isnt up to date
# Version 1.4.3
- Deprecated some methods and added clearer replacements
- Updated the icon to be more reflective of the mod
# Version 1.4.2
- Changed the events CurrentValueChanged and SavedValueChanged to only trigger when the newvalue != oldvalue
# Version 1.4.1
- Deprecated some methods and added events as replacements (yay for performance)
- Added melon loader version verification
# Version 1.4.0
- Added events to a lot of stuff (See documentation)
- Refactored the UI-Creation methods to allow for easy creation of multiple windows
- Added an automatic version check that will inform the user/mod creator that an update to this package has been published
- Included a template for mod creators (See Template folder (Should include 2 .cs files that are also found in the documentation))
- Added some methods for the creation of custom themes
- Refactored a lot of code (All changes are documented)
# Version 1.3.1
- Fixed some bugs
# Version 1.3.0
- Refactored the UI Methods
- Implemented a better version for the string validation
- Fixed a bug where the switching between toggle box and input field would not work 
- Embedded all UI-Elements in the assembly
# Version 1.2.1
- Fixed a minor oversight in the saving of files
# Version 1.2.0
- Added a keybind for opening the menu while in VR (both triggers + both primary buttons)
- Boolean Settings now feature a checkbox for easier input
- Multiple Methods have been Added
- The ModSetting class has been reworked to support non-string value types now
- UI has been scaled by 1,5x for easier interaction in VR
# Version 1.0.1
- Added some methods
# Version 1.0.0
- Created