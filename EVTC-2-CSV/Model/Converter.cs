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
                lines.Append(_parser.Metadata.GWBuild + ",");
                lines.Append(target.Name + ",");
                lines.Append(fightDuration + ",");
                lines.Append(p.Account + ",");
                lines.Append(p.Character + ",");
                lines.Append(p.Profession + ",");
                lines.Append(((p.Condition > 5) ? "Condition" : "Power") + ",");

                // DPS
                lines.Append(Math.Round(p.DamageEvents.Sum(e => e.Damage) / (fightDuration / 1000), 2) + ","); // DPS

                // Boons
                //lines.Append(Math.Round(p.BoonEvents.Where(b => b.SkillId == (int)Boon.Fury).Sum(b => b.Duration) / fightDuration, 2) + ","); // Fury
                //lines.Append(Math.Round(p.BoonEvents.Where(b => b.SkillId == (int)Boon.Quickness).Sum(b => b.Duration) / fightDuration, 2) + ","); // Quickness
                //lines.Append(Math.Round(p.BoonEvents.Where(b => b.SkillId == (int)Boon.Alacrity).Sum(b => b.Duration) / fightDuration, 2) + ","); // Alacrity

                // Statistics
                float n = p.DamageEvents.Where(e => e.IsBuff == false).Count();
                if (n == 0) { n = 1; }
                lines.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.Result == Result.Critical).Count() / n, 2) + ","); //  Critical Rate
                lines.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsNinety).Count() / n, 2) + ","); //  Scholar Rate
                lines.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsFlanking).Count() / n, 2) + ","); //  Flanking Rate
                lines.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsMoving).Count() / n, 2) + ","); //  Moving Rate
                lines.Append(_parser.Events.Where(e => e.SrcInstid == p.Instid && e.SkillId == (int)CustomSkill.Dodge).Count() + ","); // Dodge Count
                lines.Append(p.StateEvents.Where(s => s.State == StateChange.WeaponSwap).Count() + ","); // Weapon Swap Count
                lines.Append(_parser.Events.Where(e => e.SrcInstid == p.Instid && e.SkillId == (int)CustomSkill.Resurrect).Count() + ","); // Resurrect Count
                lines.Append(p.StateEvents.Where(s => s.State == StateChange.ChangeDown).Count() + ","); // Downed Count
                lines.Append(p.StateEvents.Where(s => s.State == StateChange.ChangeDead).Count()); // Died - 1 = true, 0 = false
                lines.Append(Environment.NewLine);
            }
            return lines.ToString();
        }
        #endregion
    }
}
