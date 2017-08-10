using System;
using System.Collections.Generic;

namespace EVTC_2_CSV.Model
{
    public class Player : IAgent
    {
        #region Properties
        public string Address { get; set; }
        public int FirstAware { get; set; }
        public int LastAware { get; set; }
        public int Instid { get; set; }
        public Profession Profession { get; set; }
        public string Character { get; set; }
        public string Account { get; set; }
        public string Group { get; set; }
        public int Toughness { get; set; }
        public int Healing { get; set; }
        public int Condition { get; set; }
        public List<CombatEvent> DamageEvents { get; set; } = new List<CombatEvent>();
        public List<StateEvent> StateEvents { get; set; } = new List<StateEvent>();
        public List<BoonEvent> BoonEvents { get; set; } = new List<BoonEvent>();
        #endregion

        #region Constructor
        public Player(string agentName)
        {
            string[] splitName = agentName.Split('\0');
            Character = splitName[0];
            Account = splitName[1] ?? "N/A";
            Group = splitName[2] ?? "N/A";
        }
        #endregion

        #region Public Methods
        public void LoadEvents(NPC target, List<Event> events)
        {
            SetDamageEvents(target, events);
            SetStateEvents(events);
            SetBoonEvents(events);
        }
        #endregion

        #region Private Methods
        private void SetDamageEvents(NPC target, List<Event> events)
        {
            foreach (Event e in events)
            {
                if (e.SrcInstid == Instid || e.SrcMasterInstid == Instid)
                {
                    if (e.DstInstid == target.Instid && e.IFF == IFF.Foe)
                    {
                        if (e.StateChange == StateChange.None)
                        {
                            if (e.IsBuff && e.BuffDmg != 0)
                            {
                                DamageEvents.Add(new CombatEvent()
                                {
                                    Time = e.Time,
                                    Damage = e.BuffDmg,
                                    SkillId = e.SkillId,
                                    IsBuff = e.IsBuff,
                                    Result = e.Result,
                                    IsNinety = e.IsNinety,
                                    IsMoving = e.IsMoving,
                                    IsFlanking = e.IsFlanking
                                });
                            }
                            else if (e.IsBuff == false && e.Value != 0)
                            {
                                DamageEvents.Add(new CombatEvent()
                                {
                                    Time = e.Time,
                                    Damage = e.Value,
                                    SkillId = e.SkillId,
                                    IsBuff = e.IsBuff,
                                    Result = e.Result,
                                    IsNinety = e.IsNinety,
                                    IsMoving = e.IsMoving,
                                    IsFlanking = e.IsFlanking
                                });
                            }
                        }
                    }
                }
            }
        }

        private void SetStateEvents(List<Event> events)
        {
            foreach (Event e in events)
            {
                if (e.SrcInstid == Instid)
                {
                    if (e.StateChange != StateChange.None)
                    {
                        StateEvents.Add(new StateEvent()
                        {
                            Time = e.Time,
                            State = e.StateChange
                        });
                    }
                }
            }
        }

        private void SetBoonEvents(List<Event> events)
        {
            foreach (Event e in events)
            {
                if (e.DstInstid == Instid)
                {
                    if (Enum.IsDefined(typeof(Boon), e.SkillId))
                    {
                        BoonEvents.Add(new BoonEvent()
                        {
                            Time = e.Time,
                            Duration = e.Value - e.OverstackValue,
                            SkillId = e.SkillId
                        });
                    }
                }
            }
        }
        #endregion
    }
}
