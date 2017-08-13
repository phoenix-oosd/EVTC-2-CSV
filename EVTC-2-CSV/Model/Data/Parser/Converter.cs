using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EVTC_2_CSV.Model
{
    public class Converter
    {
        #region Members
        private readonly Parser _parser;
        private readonly NPC _target;
        private readonly int _time;
        #endregion

        #region Constructor
        public Converter(Parser parser)
        {
            _parser = parser;
            _target = _parser.NPCs.Find(n => n.SpeciesId == _parser.Metadata.TargetSpeciesId);
            _time = _target.LastAware - _target.FirstAware;
        }
        #endregion

        #region Public Methods
        public String ToCSV()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Player p in _parser.Players)
            {
                p.LoadEvents(_target, _parser.Events);
                ConvertMetadata(sb);
                ConvertTarget(sb);
                ConvertGroup(sb, p);
                ConvertDamage(sb, p);
                ConvertBoons(sb, p);
                ConvertStatistics(sb, p);
                sb.Remove(sb.Length - 1, 1);
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
        #endregion

        #region Private Methods
        private void ConvertMetadata(StringBuilder sb)
        {
            if (Properties.Settings.Default.Arc)
            {
                sb.Append(_parser.Metadata.ARCVersion + ",");
            }
            if (Properties.Settings.Default.Date)
            {
                sb.Append(_parser.Metadata.LogStart.ToString("yyyy-MM-dd,"));
            }
            if (Properties.Settings.Default.Build)
            {
                sb.Append(_parser.Metadata.GWBuild + ",");
            }
        }

        private void ConvertTarget(StringBuilder sb)
        {
            if (Properties.Settings.Default.Species)
            {
                sb.Append(_target.SpeciesId + ",");
            }
            if (Properties.Settings.Default.Target)
            {
                sb.Append(_target.Name + ",");
            }
            if (Properties.Settings.Default.Time)
            {
                sb.Append(_time / 1000.0 + ",");
            }
        }

        private void ConvertGroup(StringBuilder sb, Player p)
        {
            if (Properties.Settings.Default.Account)
            {
                sb.Append(p.Account + ",");
            }
            if (Properties.Settings.Default.Character)
            {
                sb.Append(p.Character + ",");
            }
            if (Properties.Settings.Default.Profession)
            {
                sb.Append(p.Profession + ",");
            }
            if (Properties.Settings.Default.Gear)
            {
                sb.Append(((p.Condition > 5) ? "C," : "P,"));
            }
        }

        private void ConvertDamage(StringBuilder sb, Player p)
        {
            if (Properties.Settings.Default.DPS)
            {
                sb.Append(Math.Round(p.DamageEvents.Sum(e => e.Damage) / (_time / 1000.0), 2) + ","); // DPS
            }
            if (Properties.Settings.Default.PDPS)
            {
                sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff).Sum(e => e.Damage) / (_time / 1000.0), 2) + ","); // DPS
            }
            if (Properties.Settings.Default.CDPS)
            {
                sb.Append(Math.Round(p.DamageEvents.Where(e => e.IsBuff).Sum(e => e.Damage) / (_time / 1000.0), 2) + ","); // DPS
            }
            // Damage Taken
        }

        private void ConvertBoons(StringBuilder sb, Player p)
        {
            Dictionary<int, List<BoonEvent>> be = p.BoonEvents;
            foreach (Boon b in Boon.Values)
            {
                if (b.IsDuration)
                {
                    sb.Append(Math.Round(BoonDurationRates(b, be[b.SkillId]), 2) + ","); // Duration Boon Rates
                }
                else
                {
                    sb.Append(Math.Round(BoonIntensityStacks(b, be[b.SkillId]), 2) + ","); // Duration Boon Stacks
                }
            }
        }

        private void ConvertStatistics(StringBuilder sb, Player p)
        {
            double n = p.DamageEvents.Where(e => !e.IsBuff).Count(); // Power Damage Events
            if (n == 0) { n = 1; } // Avoid 0 Division
            if (Properties.Settings.Default.Critical)
            {
                sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.Result == Result.Critical).Count() / n, 2) + ","); // Critical Rate
            }
            if (Properties.Settings.Default.Scholar)
            {
                sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsNinety).Count() / n, 2) + ","); // Scholar Rate
            }
            if (Properties.Settings.Default.Flank)
            {
                sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsFlanking).Count() / n, 2) + ","); // Flanking Rate
            }
            if (Properties.Settings.Default.Moving)
            {
                sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsMoving).Count() / n, 2) + ","); // Moving Rate
            }
            if (Properties.Settings.Default.Dodge)
            {
                sb.Append(_parser.Events.Where(e => e.SrcInstid == p.Instid && e.SkillId == (int)CustomSkill.Dodge).Count() + ","); // Dodge Count
            }
            if (Properties.Settings.Default.Swap)
            {
                sb.Append(p.StateEvents.Where(s => s.StateChange == StateChange.WeaponSwap).Count() + ","); // Weapon Swap Count
            }
            if (Properties.Settings.Default.Resurrect)
            {
                sb.Append(_parser.Events.Where(e => e.SrcInstid == p.Instid && e.SkillId == (int)CustomSkill.Resurrect).Count() + ","); // Resurrect Count
            }
            if (Properties.Settings.Default.Downed)
            {
                sb.Append(p.StateEvents.Where(s => s.StateChange == StateChange.ChangeDown).Count() + ","); // Downed Count
            }
            if (Properties.Settings.Default.Died)
            {
                sb.Append(p.StateEvents.Where(s => s.StateChange == StateChange.ChangeDead).Count() + ","); // Died
            }
        }

        private double BoonDurationRates(Boon b, List<BoonEvent> boonEvents)
        {
            if (boonEvents.Count == 0) { return 0.0; }
            int prev = 0;
            int curr = 0;
            List<Interval> bi = new List<Interval>();
            BoonStack bs = new BoonStackDuration(b.Capacity);
            foreach (BoonEvent be in boonEvents)
            {
                curr = be.Time;
                bs.Update(curr - prev);
                bs.Add(be.Duration);
                bi.Add(new Interval
                {
                    Start = curr,
                    End = curr + bs.CalculateValue()
                });
                prev = curr;
            }
            List<Interval> mbi = (bi.Count == 1) ? bi : new List<Interval>();
            if (mbi.Count == 0)
            {
                Interval a = bi[0];
                int s = a.Start;
                int e = a.End;
                for (int i = 1; i < bi.Count; i++)
                {
                    Interval c = bi[i];
                    if (c.Start <= e)
                    {
                        e = Math.Max(c.End, e);
                    }
                    else
                    {
                        mbi.Add(new Interval() { Start = s, End = e });
                        s = c.Start;
                        e = c.End;
                    }
                }
                mbi.Add(new Interval() { Start = s, End = e });
            }
            Interval z = mbi[mbi.Count - 1];
            if (z.End > _time) { z.End = _time; }
            double duration = 0.0;
            foreach (Interval i in mbi)
            {
                duration += (i.End - i.Start);
            }
            return duration / _time;
        }

        private double BoonIntensityStacks(Boon b, List<BoonEvent> boonEvents)
        {
            if (boonEvents.Count == 0) { return 0.0; }
            int prev = 0;
            int curr = 0;
            List<int> s = new List<int> { 0 };

            BoonStackIntensity bs = new BoonStackIntensity(b.Capacity);
            foreach (BoonEvent be in boonEvents)
            {
                curr = be.Time;
                bs.SimulateTimePassed(curr - prev, s);
                bs.Update(curr - prev);
                bs.Add(be.Duration);
                if (prev != curr)
                {
                    s.Add(bs.CalculateValue());
                }
                else
                {
                    s[s.Count - 1] = bs.CalculateValue();
                }
                prev = curr;
            }
            bs.SimulateTimePassed(_time - prev, s);
            bs.Update(1);
            s.Add(bs.CalculateValue());
            //Console.WriteLine();
            //Console.WriteLine(s.Sum() + "|" +_time);
            //using (StreamWriter f = new StreamWriter("stacks.txt"))
            //{
            //    foreach (int k in s)
            //    {
            //        f.WriteLine(k.ToString());
            //    }
            //}
            //Console.Read();

            return s.Average();
        }

        #endregion
    }
}
