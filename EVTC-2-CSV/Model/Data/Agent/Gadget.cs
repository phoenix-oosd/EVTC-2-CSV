namespace EVTC_2_CSV.Model
{
    public class Gadget : IAgent
    {
        #region Properties
        public string Address { get; set; }
        public int FirstAware { get; set; }
        public int LastAware { get; set; }
        public int Instid { get; set; }
        public int PseudoId { get; set; }
        public string Name { get; set; }
        #endregion
    }
}
