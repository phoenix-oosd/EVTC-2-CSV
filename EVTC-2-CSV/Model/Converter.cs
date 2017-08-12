using System;
using System.Linq;
using System.Text;

namespace EVTC_2_CSV.Model
{
    public class Converter
    {
        #region Members
        private readonly Parser _parser;
        private readonly NPC _target;
        private readonly double _fightDuration;
        #endregion

        #region Constructor
        public Converter(Parser parser)
        {
            _parser = parser;
            _target = _parser.NPCs.Find(n => n.SpeciesId == _parser.Metadata.TargetSpeciesId);
            _fightDuration = _target.LastAware - _target.FirstAware;
        }
        #endregion

        #region Public Methods
        public String ToCSV()
        {
            StringBuilder lines = new StringBuilder();
            foreach (Player p in _parser.Players)
            {
                p.LoadEvents(_target, _parser.Events);
                ConvertMetadata(lines);
                ConvertTarget(lines);
                ConvertGroup(lines, p);
                ConvertDamage(lines, p);
                ConvertBoons(lines, p);
                ConvertStatistics(lines, p);
                lines.Remove(lines.Length - 1, 1);
                lines.Append(Environment.NewLine);
            }
            return lines.ToString();
        }
        #endregion

        #region Private Methods
        private void ConvertMetadata(StringBuilder lines)
        {
            if (Properties.Settings.Default.Arc)
            {
                lines.Append(_parser.Metadata.ARCVersion + ",");
            }
            if (Properties.Settings.Default.Date)
            {
                lines.Append(_parser.Metadata.LogStart.ToString("yyyy-MM-dd,"));
            }
            if (Properties.Settings.Default.Build)
            {
                lines.Append(_parser.Metadata.GWBuild + ",");
            }
        }
        private void ConvertTarget(StringBuilder lines)
        {
            if (Properties.Settings.Default.Species)
            {
                lines.Append(_target.SpeciesId + ",");
            }
            if (Properties.Settings.Default.Target)
            {
                lines.Append(_target.Name + ",");
            }
            if (Properties.Settings.Default.Time)
            {
                lines.Append(_fightDuration / 1000.0 + ",");
            }
        }
        private void ConvertGroup(StringBuilder lines, Player p)
        {
            if (Properties.Settings.Default.Account)
            {
                lines.Append(p.Account + ",");
            }
            if (Properties.Settings.Default.Character)
            {
                lines.Append(p.Character + ",");
            }
            if (Properties.Settings.Default.Profession)
            {
                lines.Append(p.Profession + ",");
            }
            if (Properties.Settings.Default.Gear)
            {
                lines.Append(((p.Condition > 5) ? "Condition," : "Power,"));
            }
        }
        private void ConvertDamage(StringBuilder lines, Player p)
        {
            if (Properties.Settings.Default.DPS)
            {
                lines.Append(Math.Round(p.DamageEvents.Sum(e => e.Damage) / (_fightDuration / 1000), 2) + ","); // DPS
            }
        }
        private void ConvertBoons(StringBuilder lines, Player p)
        {
            //lines.Append(Math.Round(p.BoonEvents.Where(b => b.SkillId == (int)Boon.Fury).Sum(b => b.Duration) / fightDuration, 2) + ","); // Fury
            //lines.Append(Math.Round(p.BoonEvents.Where(b => b.SkillId == (int)Boon.Quickness).Sum(b => b.Duration) / fightDuration, 2) + ","); // Quickness
            //lines.Append(Math.Round(p.BoonEvents.Where(b => b.SkillId == (int)Boon.Alacrity).Sum(b => b.Duration) / fightDuration, 2) + ","); // Alacrity
        }
        private void ConvertStatistics(StringBuilder lines, Player p)
        {
            float n = p.DamageEvents.Where(e => e.IsBuff == false).Count();
            if (n == 0) { n = 1; }
            if (Properties.Settings.Default.Critical)
            {
                lines.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.Result == Result.Critical).Count() / n, 2) + ","); //  Critical Rate
            }
            if (Properties.Settings.Default.Scholar)
            {
                lines.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsNinety).Count() / n, 2) + ","); //  Scholar Rate
            }
            if (Properties.Settings.Default.Flank)
            {
                lines.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsFlanking).Count() / n, 2) + ","); //  Flanking Rate
            }
            if (Properties.Settings.Default.Moving)
            {
                lines.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsMoving).Count() / n, 2) + ","); //  Moving Rate
            }
            if (Properties.Settings.Default.Dodge)
            {
                lines.Append(_parser.Events.Where(e => e.SrcInstid == p.Instid && e.SkillId == (int)CustomSkill.Dodge).Count() + ","); // Dodge Count
            }
            if (Properties.Settings.Default.Swap)
            {
                lines.Append(p.StateEvents.Where(s => s.State == StateChange.WeaponSwap).Count() + ","); // Weapon Swap Count
            }
            if (Properties.Settings.Default.Resurrect)
            {
                lines.Append(_parser.Events.Where(e => e.SrcInstid == p.Instid && e.SkillId == (int)CustomSkill.Resurrect).Count() + ","); // Resurrect Count
            }
            if (Properties.Settings.Default.Downed)
            {
                lines.Append(p.StateEvents.Where(s => s.State == StateChange.ChangeDown).Count() + ","); // Downed Count
            }
            if (Properties.Settings.Default.Died)
            {
                lines.Append(p.StateEvents.Where(s => s.State == StateChange.ChangeDead).Count() + ","); // Died
            }
        }
        #endregion
    }
}
