# EVTC-2-CSV

### ABOUT
* Exports an event chain log's (.evtc) relevant information into a .csv file format
* Used to populate databases with a light load

### USAGE
* Move the .exe in new folder
* Move the .evtc files to that new folder
* Run the .exe
* Press the any key
* Wait
* A single .csv file will be exported to the same directory

### HEADER INFO
|Name|Type|Description|
|----|----|-----------|
|Build|:INTEGER|The game's [build](https://api.guildwars2.com/v2/build) at the time of logging
|Target|:STRING|The name of the target NPC
|Time|:INTEGER|The fight duration in milliseconds
|Account|:STRING|The display name and account number
|Character|:STRING|The character name
|Profession|:STRING|The character profession (elite)
|Gear|:STRING|The gear in terms of "Power" or "Condition"
|DPS|:FLOAT|The DPS from start to end of the fight
|Critical|:FLOAT|The percentage of hits which were critical
|Scholar|:FLOAT|The percentage of hits which were above 90% health
|Flank|:FLOAT|The percentage of hits flanking
|Moving|:FLOAT|The percentage of hits while moving
|Dodge|:INTEGER|The number of dodges
|Swap|:INTEGER|The number of weapon swaps
|Resurrect|:INTEGER|The number of times resurrecting an ally
|Downed|:INTEGER|The number of times downed in a fight
|Died|:BOOLEAN|1 = Died, 0 = Survived
