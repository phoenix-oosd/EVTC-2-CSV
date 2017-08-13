# EVTC-2-CSV

### ABOUT
* Exports parsed `.evtc` data into a `.csv` file format
* Customizable header fields
* Each row consists of character data

### USAGE
* Move `EVTC-2-CSV.exe` into a new folder
* Move `EVTC-2-CSV.exe.config` into the same folder (edit the .config as desired)
* Move `.evtc` files you wish to export to the new folder
* Run `EVTC-2-CSV.exe`

### HEADER INFO
|Name      |Type    |Description|
|----------|--------|-----------|
|Arc       |:STRING |Build of [arcdps](https://www.deltaconnected.com/arcdps/) used to generate the log
|Date      |:DATE   |Date of log creation in "yyyy-mm-dd" format
|Build     |:INTEGER|Guild Wars 2 [build](https://api.guildwars2.com/v2/build) to identify game patch
|Species   |:INTEGER|Species ID of the target NPC e.g. 15429 = Gorseval
|Target    |:STRING |Name of the target NPC
|Time      |:INTEGER|Duration of the encounter
|Account   |:STRING |Display name and account number
|Character |:STRING |Character name
|[Boons](#boons)   |:FLOAT  | Rates and stacks of various boons
|Profession|:STRING |Profession / elite specialization
|Gear      |:STRING |Main attribute of gear ("P" = Power, "C" = Condition)
|DPS       |:FLOAT  |Total target DPS
|PDPS      |:FLOAT  |Power target DPS
|CDPS      |:FLOAT  |Condition target DPS
|Critical  |:FLOAT  |Critical hit rate
|Scholar   |:FLOAT  |Health above 90% hit rate
|Flank     |:FLOAT  |Flanking hit rate
|Moving    |:FLOAT  |Moving hit rate (e.g. Seaweed Salad)
|Dodge     |:INTEGER|Dodge count
|Swap      |:INTEGER|Weapon swap count
|Resurrect |:INTEGER|Resurrection pulse count
|Downed    |:INTEGER|Downed count
|Died      |:BOOLEAN|If died during encounter

### BOONS
|Name                 |
|---------------------|
|Might                |
|Fury                 |
|Quickness            |
|Alacrity             |
|EmpowerAllies        |
|BannerOfStrength     |
|BannerOfDiscipline   |
|Spotter              |
|SunSpirit            |
|FrostSpirit          |
|GlyphOfEmpowerment   |
|GraceOfTheLand       |
|PinpointDistribution |
|AssassinsPresence    |
|NaturalisticResonance|

### CONFIG INFO
* Header fields are written in the order of the table above
* Change the Boolean between `<value></value>` to `True` to include the field
* Change the Boolean between `<value></value>` to `False` to exclude the field
