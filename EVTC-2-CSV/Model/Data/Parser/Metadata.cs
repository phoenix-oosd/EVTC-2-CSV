using System;

namespace EVTC_2_CSV.Model
{
    public class Metadata
    {
        #region Properties
        public string BuildVersion { get; set; }
        public int TargetSpeciesId { get; set; }
        public DateTime LogStart { get; set; }
        public DateTime LogEnd { get; set; }
        public string PointOfView { get; set; }
        public Language Language { get; set; }
        public int GWBuild { get; set; }
        public int ShardID { get; set; }
        public bool IsSuccess { get; set; }
        #endregion
    }
}
