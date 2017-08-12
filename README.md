# EVTC-2-CSV

### ABOUT
* Exports an event chain log's (.evtc) information into a .csv file format
* Used to populate databases with a light load
* Customizable header fields

### USAGE
* Move EVTC-2-CSV.exe into a new folder
* Move EVTC-2-CSV.exe.config into the same folder (edit the .config as desired)
* Move .evtc files you wish to export to the new folder
* Run EVTC-2-CSV.exe
* Press the "any" key
* Play the waiting game
* A single .csv file will be exported to the folder

### HEADER INFO
|Name|Type|Description|
|----|----|-----------|
|Arc|:STRING|Build of [arcdps](https://www.deltaconnected.com/arcdps/) used to generate the log
|Date|:DATE|Date of log creation in "yyyy-mm-dd" format
|Build|:INTEGER|Guild Wars 2 [build](https://api.guildwars2.com/v2/build) to identify game patch
|Species|:INTEGER|Species ID of the target NPC e.g. 15429 = Gorseval
|Target|:STRING|Name of the target NPC
|Time|:INTEGER|Duration of the encounter in fractional seconds
|Account|:STRING|Display name and account number e.g :Account:0000
|Character|:STRING|Character name
|Profession|:STRING|Character profession (elite specialization overrides base profession)
|Gear|:STRING|Main attribute of character's gear e.g. "Power", "Condition"
|DPS|:FLOAT|Damage per second to the target
|Critical|:FLOAT|Percentage of physical hits where the character lands a critical hit
|Scholar|:FLOAT|Percentage of physical hits where the character's health is above 90%
|Flank|:FLOAT|Percentage of physical hits where the character is flanking
|Moving|:FLOAT|The percentage of physical hits where the character is moving (e.g. Seaweed Salad)
|Dodge|:INTEGER|Number of times the character dodges
|Swap|:INTEGER|Number of times the character swaps weapons (includes kits,bundles,attuning etc.)
|Resurrect|:INTEGER|Number of resurrection pulses when picking up a downed ally
|Downed|:INTEGER|Number of times the character downs
|Died|:BOOLEAN|Whether the character died during the encounter

### CONFIG INFO
* Headers are written in the order of the default configuration file
* Change value between <value></value> to "True" to include the header
* Change value between <value></value> to "True" to exclude the header

|Name|Description|
|----|-----------|
WriteHeaders|Writes the names of the fields into the first row of the .csv file (default is "False")
Write[x]|Writes [x] field into each row


### TODO
* Add relevant boons
* Anything else on request
