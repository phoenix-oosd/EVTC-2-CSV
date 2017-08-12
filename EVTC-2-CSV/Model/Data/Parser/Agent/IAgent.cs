using System;

namespace EVTC_2_CSV.Model
{
    public interface IAgent
    {
        #region Properties
        string Address { get; set; }
        int FirstAware { get; set; }
        int LastAware { get; set; }
        int Instid { get; set; }
        #endregion
    }
}
