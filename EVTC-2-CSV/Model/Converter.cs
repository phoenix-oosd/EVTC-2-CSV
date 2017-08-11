using System;
using System.Linq;
using System.Text;

namespace EVTC_2_CSV.Model
{
    public class Converter
    {
        #region Members
        private readonly Parser _parser;
        #endregion

        #region Constructor
        public Converter(Parser parser)
        {
            _parser = parser;
        }
        #endregion

        #region Public Methods
        public String CSV()
        {
            // CSV Lines
            StringBuilder lines = new StringBuilder();

            // Target
            NPC target = _parser.NPCs.Find(n => n.SpeciesId == _parser.Metadata.TargetSpeciesId);
            double fightDuration = target.LastAware - target.FirstAware;

            // Player
            foreach (Player p in _parser.Players)
            {
                // Load
                p.LoadEvents(target, _parser.Events); // Load Events

                // Info
                if (Properties.Settings.Default.WriteBuild)
                {
                    lines.Append(_parser.Metadata.GWBuild + ",");
                }
                if (Properties.Settings.Default.WriteTarget)
                {
                    lines.Append(target.Name + ",");
                }
                if (Properties.Settings.Default.WriteTime)
                {
                    lines.Append(fightDuration + ",");
                }
                if (Properties.Settings.Default.WriteAccount)
                {
                    lines.Append(p.Account + ",");
                }
                if (Properties.Settings.Default.WriteCharacter)
                {
                    lines.Append(p.Character + ",");
                }
                if (Properties.Settings.Default.WriteProfession)
                {
                    lines.Append(p.Profession + ",");
                }
                if (Properties.Settings.Default.WriteGear)
                {
                    lines.Append(((p.Condition > 5) ? "Condition" : "Power") + ",");
                }

                // DPS
                if (Properties.Settings.Default.WriteDPS)
                {
                    lines.Append(Math.Round(p.DamageEvents.Sum(e => e.Damage) / (fightDuration / 1000), 2) + ","); // DPS
                }

                // Boons
                //lines.Append(Math.Round(p.BoonEvents.Where(b => b.SkillId == (int)Boon.Fury).Sum(b => b.Duration) / fightDuration, 2) + ","); // Fury
                //lines.Append(Math.Round(p.BoonEvents.Where(b => b.SkillId == (int)Boon.Quickness).Sum(b => b.Duration) / fightDuration, 2) + ","); // Quickness
                //lines.Append(Math.Round(p.BoonEvents.Where(b => b.SkillId == (int)Boon.Alacrity).Sum(b => b.Duration) / fightDuration, 2) + ","); // Alacrity

                // Statistics
                float n = p.DamageEvents.Where(e => e.IsBuff == false).Count();
                if (n == 0) { n = 1; }
                if (Properties.Settings.Default.WriteCritical)
                {
                    lines.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.Result == Result.Critical).Count() / n, 2) + ","); //  Critical Rate
                }
                if (Properties.Settings.Default.WriteScholar)
                {
                    lines.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsNinety).Count() / n, 2) + ","); //  Scholar Rate
                }
                if (Properties.Settings.Default.WriteFlank)
                {
                    lines.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsFlanking).Count() / n, 2) + ","); //  Flanking Rate
                }
                if (Properties.Settings.Default.WriteMoving)
                {
                    lines.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsMoving).Count() / n, 2) + ","); //  Moving Rate
                }
                if (Properties.Settings.Default.WriteDodge)
                {
                    lines.Append(_parser.Events.Where(e => e.SrcInstid == p.Instid && e.SkillId == (int)CustomSkill.Dodge).Count() + ","); // Dodge Count
                }
                if (Properties.Settings.Default.WriteSwap)
                {
                    lines.Append(p.StateEvents.Where(s => s.State == StateChange.WeaponSwap).Count() + ","); // Weapon Swap Count
                }
                if (Properties.Settings.Default.WriteResurrect)
                {
                    lines.Append(_parser.Events.Where(e => e.SrcInstid == p.Instid && e.SkillId == (int)CustomSkill.Resurrect).Count() + ","); // Resurrect Count
                }
                if (Properties.Settings.Default.WriteDowned)
                {
                    lines.Append(p.StateEvents.Where(s => s.State == StateChange.ChangeDown).Count() + ","); // Downed Count
                }
                if (Properties.Settings.Default.WriteDied)
                {
                    lines.Append(p.StateEvents.Where(s => s.State == StateChange.ChangeDead).Count() + ","); // Died - 1 = true, 0 = false
                }
                lines.Remove(lines.Length - 1, 1);
                lines.Append(Environment.NewLine);
            }
            return lines.ToString();
        }
        #endregion
    }
}
