using System;
using System.Linq;

namespace EVTC_2_CSV.Model
{
    public class BoonStackDuration : BoonStack
    {
        #region Constructor
        public BoonStackDuration(int capacity) : base(capacity) { }
        #endregion

        #region Abstract Methods
        public override int CalculateValue()
        {
            return _boonStack.Sum();
        }

        public override void Update(int timePassed)
        {
            if (_boonStack.Count > 0)
            {
                if (timePassed >= CalculateValue())
                {
                    _boonStack.Clear();
                }
                else
                {
                    _boonStack[0] -= timePassed;
                    if (_boonStack[0] < 0)
                    {
                        timePassed = _boonStack[0];
                        _boonStack.RemoveAt(0);
                        Update(Math.Abs(timePassed));
                    }
                }
            }
        }
        #endregion
    }
}
