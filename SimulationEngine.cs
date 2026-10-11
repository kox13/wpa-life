namespace PLA_1
{
    internal class SimulationEngine
    {
        public Board Board { get; private set; }

        public SimulationEngine()
        {
            this.Board = new Board(50, 50); // TODO: dynamic size
        }
    }
}
