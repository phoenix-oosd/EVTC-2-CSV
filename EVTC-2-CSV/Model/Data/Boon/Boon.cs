using System.Collections.Generic;

namespace EVTC_2_CSV.Model
{
    public class Boon
    {
        #region Static
        public static readonly Boon Might = new Boon(740, 25, false);
        public static readonly Boon Quickness = new Boon(1187, 5, true);
        public static readonly Boon Fury = new Boon(725, 9, true);
        #endregion

        #region Members
        private readonly int _skillId;
        private readonly int _capacity;
        private readonly bool _isDuration;
        #endregion

        #region Properties
        public int SkillId { get { return _skillId; } }
        public int Capacity { get { return _capacity; } }
        public bool IsDuration { get { return _isDuration; } }
        #endregion

        #region Constructor
        private Boon(int skillId, int capacity, bool isDuration)
        {
            _skillId = skillId;
            _capacity = capacity;
            _isDuration = isDuration;
        }
        #endregion

        #region Public Methods
        public static IEnumerable<Boon> Values
        {
            get
            {
                yield return Might;
                yield return Quickness;
                yield return Fury;
            }
        }
        #endregion


        //Alacrity = 30328,
        //EmpowerAllies = 14222,
        //BannerOfStrength = 14417,
        //BannerOfDiscipline = 14449,
    }
}
