using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace EVTC_2_CSV.Model
{
    public class Parser
    {
        #region Members
        private BinaryReader _reader;
        #endregion

        #region Properties
        public Metadata Metadata { get; set; }
        public List<Player> Players { get; set; }
        public List<NPC> NPCs { get; set; }
        public List<Gadget> Gadgets { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Event> Events { get; set; }
        #endregion

        #region Public Methods
        public bool Parse(string filePath)
        {
            try
            {
                if (Path.GetExtension(filePath) == ".evtc")
                {
                    _reader = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read));
                }
                else
                {
                    Stream stream = ZipFile.OpenRead(filePath).Entries.First().Open();
                    MemoryStream memStream = new MemoryStream();
                    stream.CopyTo(memStream);
                    memStream.Position = 0;
                    _reader = new BinaryReader(memStream);
                }
                using (_reader)
                {
                    Metadata = new Metadata();
                    Players = new List<Player>();
                    NPCs = new List<NPC>();
                    Gadgets = new List<Gadget>();
                    Skills = new List<Skill>();
                    Events = new List<Event>();
                    ParseMetadata();
                    if (int.Parse(Metadata.ARCVersion.Substring(4)) < 20170218) { return false; }
                    ParseAgents();
                    ParseSkills();
                    ParseEvents();
                    FillMissingData();
                    return true;
                }
            }
            catch { return false; }
        }
        #endregion

        #region Private Methods
        private void ParseMetadata()
        {
            Metadata.ARCVersion = _reader.ReadUTF8(12); // 12 bytes: build version 
            Metadata.TargetSpeciesId = _reader.Skip(1).ReadUInt16(); // 2 bytes: instid
        }

        private void ParseAgents()
        {
            // 4 bytes: agent count
            int agentCount = _reader.Skip(1).ReadInt32();

            // 96 bytes: each agent
            for (int i = 0; i < agentCount; i++)
            {
                // add agent
                AddAgent();
            }
        }

        private void ParseSkills()
        {
            // 4 bytes: skill count
            int skillCount = _reader.ReadInt32();

            // 68 bytes: each skill
            for (int i = 0; i < skillCount; i++)
            {
                Skills.Add(new Skill()
                {
                    Id = _reader.ReadInt32(), // 4 bytes: id
                    Name = _reader.ReadUTF8(64) // 64 bytes: name
                });
            }
        }

        private void ParseEvents()
        {
            // Read until EOF
            while (_reader.BaseStream.Position != _reader.BaseStream.Length)
            {
                Event combat = new Event()
                {
                    Time = (int)_reader.ReadUInt64(), // 8 bytes: time
                    SrcAgent = _reader.ReadUInt64Hex(), // 8 bytes: src agent
                    DstAgent = _reader.ReadUInt64Hex(),  // 8 bytes: dst agent
                    Value = _reader.ReadInt32(), // 4 bytes: value
                    BuffDmg = _reader.ReadInt32(), // 4 bytes: buff damage
                    OverstackValue = _reader.ReadUInt16(), // 2 bytes: overstack value
                    SkillId = _reader.ReadUInt16(), // 2 bytes: skill id
                    SrcInstid = _reader.ReadUInt16(), // 2 bytes: src instid
                    DstInstid = _reader.ReadUInt16(), // 2 bytes: dst instid
                    SrcMasterInstid = _reader.ReadUInt16(), // 2 bytes: src master instid
                    IFF = (IFF)_reader.Skip(9).Read(), // 1 byte: IFF
                    IsBuff = _reader.ReadBoolean(), // 1 byte: IsBuff
                    Result = (Result)_reader.Read(), // 1 byte: Result
                    Activation = (Activation)_reader.Read(), // 1 byte: Activation
                    BuffRemove = (BuffRemove)_reader.Read(), // 1 byte: BuffRemove
                    IsNinety = _reader.ReadBoolean(), // 1 byte: IsNinety
                    IsFifty = _reader.ReadBoolean(), // 1 byte: IsFifty
                    IsMoving = _reader.ReadBoolean(), // 1 byte: IsMoving
                    StateChange = (StateChange)_reader.Read(), // 1 byte: StateChange
                    IsFlanking = _reader.ReadBoolean() // 1 byte: IsFlanking
                };

                // Add Combat
                _reader.Skip(3);
                Events.Add(combat);
            }
        }

        private Profession ParseProfession(int profLower, int profUpper, int isElite)
        {
            if (isElite == -1)
            {
                if (profUpper == 65535)
                {
                    return Profession.Gadget;
                }
                else
                {
                    return Profession.NPC;
                }
            }
            else
            {
                return (Profession)profLower + (9 * isElite);
            }
        }

        private void AddAgent()
        {
            string address = _reader.ReadUInt64Hex(); // 8 bytes: address
            int profLower = BitConverter.ToUInt16(_reader.ReadBytes(2), 0); // 2 bytes: prof_lower
            int profUpper = BitConverter.ToUInt16(_reader.ReadBytes(2), 0); // 2 bytes: prof_upper
            int isElite = _reader.ReadInt32(); // 4 bytes: is_elite
            int toughness = _reader.ReadInt32();  // 4 bytes: toughness
            int healing = _reader.ReadInt32();  // 4 bytes: healing
            int condition = _reader.ReadInt32();  // 4 bytes: condition
            string name = _reader.ReadUTF8(68); // 68 bytes: name

            // Add Agent by Type
            Profession profession = ParseProfession(profLower, profUpper, isElite);
            switch (profession)
            {
                case Profession.Gadget:
                    Gadgets.Add(new Gadget()
                    {
                        Address = address,
                        PseudoId = profLower,
                        Name = name
                    });
                    return;
                case Profession.NPC:
                    NPCs.Add(new NPC()
                    {
                        Address = address,
                        SpeciesId = profLower,
                        Toughness = toughness,
                        Healing = healing,
                        Condition = condition,
                        Name = name
                    });
                    return;
                default:
                    Players.Add(new Player(name)
                    {
                        Address = address,
                        Profession = profession,
                        Toughness = toughness,
                        Healing = healing,
                        Condition = condition,
                    });
                    return;
            }
        }

        private void FillMissingData()
        {
            // Update Times
            int timeStart = Events.First().Time;
            Events.ForEach(e => e.Time -= timeStart);

            // Update Instid
            foreach (Player p in Players)
            {
                foreach (Event e in Events)
                {
                    if (p.Address == e.SrcAgent && e.SrcInstid != 0)
                    {
                        p.Instid = e.SrcInstid;
                    }
                    else if (p.Address == e.DstAgent && e.DstInstid != 0)
                    {
                        p.Instid = e.DstInstid;
                    }
                }
            }
            foreach (NPC n in NPCs)
            {
                foreach (Event e in Events)
                {
                    if (n.Address == e.SrcAgent && e.SrcInstid != 0)
                    {
                        n.Instid = e.SrcInstid;
                    }
                    else if (n.Address == e.DstAgent && e.DstInstid != 0)
                    {

                        n.Instid = e.DstInstid;
                    }
                }
            }

            // Update Metadata
            IEnumerable<Event> stateChanges = Events.Where(e => e.StateChange != StateChange.None);
            foreach (Event e in stateChanges)
            {
                StateChange stateChange = e.StateChange;
                if (stateChange == StateChange.LogStart)
                {
                    Metadata.LogStart = DateTimeOffset.FromUnixTimeSeconds(e.Value).DateTime;
                }
                else if (stateChange == StateChange.LogEnd)
                {
                    Metadata.LogEnd = DateTimeOffset.FromUnixTimeSeconds(e.Value).DateTime;
                }
                else if (stateChange == StateChange.PointOfView)
                {
                    Metadata.PointOfView = (Players.Find(p => p.Address == e.SrcAgent) != null) ? Players.Find(p => p.Address == e.SrcAgent).Account : ":?.????";
                }
                else if (stateChange == StateChange.Language)
                {
                    Metadata.Language = (Language)e.Value;
                }
                else if (stateChange == StateChange.GWBuild)
                {
                    Metadata.GWBuild = int.Parse(e.SrcAgent, NumberStyles.HexNumber);
                }
                else if (stateChange == StateChange.ShardID)
                {
                    Metadata.ShardID = int.Parse(e.SrcAgent, NumberStyles.HexNumber);
                }
            }

            // Target
            NPC target = NPCs.Find(n => n.SpeciesId == Metadata.TargetSpeciesId);
            if (target.SpeciesId == 16246)
            {
                NPC xeraSecond = NPCs.Find(n => n.SpeciesId == 16286);
                if (xeraSecond != null)
                {
                    foreach (Event e in Events)
                    {
                        if (e.SrcInstid == xeraSecond.Instid)
                        {
                            e.SrcInstid = target.Instid;
                        }
                        else if (e.DstInstid == xeraSecond.Instid)
                        {
                            e.DstInstid = target.Instid;
                        }
                        else if (e.SrcMasterInstid == xeraSecond.Instid)
                        {
                            e.SrcMasterInstid = target.Instid;
                        }
                    }
                }
            }
            Event targetDeath = Events.Find(e => e.StateChange == StateChange.ChangeDead && e.SrcInstid == target.Instid);
            if (targetDeath != null) {
                target.LastAware = targetDeath.Time;
                Events = Events.TakeWhile(e => !(e.StateChange == StateChange.ChangeDead && e.SrcInstid == target.Instid)).ToList();
            }
            else
            {
                target.LastAware = Events.Where(e => e.SrcInstid == target.Instid).Last().Time;
            }
        }
        #endregion
    }
}
