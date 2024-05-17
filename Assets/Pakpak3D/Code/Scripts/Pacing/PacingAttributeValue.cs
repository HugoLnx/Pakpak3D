namespace SensenToolkit
{
    public struct PacingAttributeValue
    {
        public PacingAttributeDefinition Definition { get; private set; }
        public float Value { get; private set; }

        public PacingAttributeValue(PacingAttributeDefinition attribute, float value)
        {
            Definition = attribute;
            Value = value;
        }
    }
}
