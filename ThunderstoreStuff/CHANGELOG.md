# Version 1.5.0
- Added Setting to make Dropdown Selection persistent
- Window Position will only get reset if it is out of bounds
- Changed the background color of the togglebox to improve readability
- Removed the see-through aspect of light/dark theme (Looked horrendous with the above change)
- Added some public calls for mod creators (Class: Baum_API)
- Added basic networking using Photon RPCs (Credits to nickklmao for the PunRPC workaround)
- Added a call for the Thunderstore API 
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