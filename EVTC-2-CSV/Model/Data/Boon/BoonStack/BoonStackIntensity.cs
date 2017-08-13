namespace EVTC_2_CSV.Model
{
    public class BoonStackIntensity : BoonStack
    {
        #region Constructor
        public BoonStackIntensity(int capacity) : base(capacity) { }
        #endregion

        #region Abstract Methods
        public override int CalculateValue()
        {
            return -1;
        }

        public override void Update(int timePassed)
        {
        }
        #endregion
    }
}
