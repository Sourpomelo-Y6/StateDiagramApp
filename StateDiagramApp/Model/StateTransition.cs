namespace StateDiagramApp.Model
{
    public class StateTransition
    {
        public State ToState { get; set; }

        public StateTransition(State toState)
        {
            ToState = toState;
        }

    }
}