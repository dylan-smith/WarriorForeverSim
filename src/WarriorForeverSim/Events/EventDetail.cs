namespace WarriorForeverSim
{
    // A labelled, pre-formatted value describing how an event was resolved. Shown in the
    // combat log tooltip, grouped by Section.
    public class EventDetail
    {
        public string Section { get; }
        public string Label { get; }
        public string Value { get; }

        public EventDetail(string section, string label, string value)
        {
            Section = section;
            Label = label;
            Value = value;
        }
    }
}
