using System.Collections.Generic;

namespace EVTC_2_CSV.Model
{
    public abstract class BoonStack
    {
        #region Members
        protected readonly int _capacity;
        protected List<int> _boonStack = new List<int>();
        #endregion

        #region Constructor
        public BoonStack(int capacity)
        {
            _capacity = capacity;
        }
        #endregion

        #region Abstract Methods
        public abstract int CalculateValue();
        public abstract void Update(int timePassed);
        #endregion

        #region Public Methods
        public void Add(int duration)
        {
            if (IsFull())
            {
                int i = _boonStack.Count - 1;
                if (_boonStack[i] < duration)
                {
                    _boonStack[i] = duration;
                    ReverseSort();
                }
            }
            else
            {
                _boonStack.Add(duration);
                ReverseSort();
            }
        }
        #endregion

        #region Protected Methods
        protected bool IsFull()
        {
            return _boonStack.Count >= _capacity;
        }

        protected void ReverseSort()
        {
            _boonStack.Sort();
            _boonStack.Reverse();
        }
        #endregion
    }
}
