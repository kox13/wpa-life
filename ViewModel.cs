namespace PLA_1
{
    internal class ViewModel
    {
        public SimulationEngine Engine { get; private set; }

        public ViewModel()
        {
            Engine = new SimulationEngine();
        }
    }
}
