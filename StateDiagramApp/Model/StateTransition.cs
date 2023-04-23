namespace StateDiagramApp.Model
{
    public class StateTransition
    {
        
        public uint ToStateID { get; set; }

        public string Comment;

        public StateTransition(State toState)
        {
            ToStateID = toState.IDNo;
        }

        public StateTransition() { }
    }
}