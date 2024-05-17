using System.Collections.Generic;
using SensenToolkit.Lerp;

namespace SensenToolkit
{
    public class PacingValueSet
    {
        private readonly HashSet<PacingAttributeValue> _values = new();
        private readonly Dictionary<PacingAttributeDefinition, PacingAttributeValue> _valuesMap = new();
        private EaseLerper _lerper;

        public IReadOnlyCollection<PacingAttributeValue> All => _values;

        public PacingValueSet(IReadOnlyCollection<PacingAttributeValue> values = null)
        {
            _lerper = new EaseLerper();
            if (values != null)
            {
                OverwriteValues(values);
            }
        }

        public void OverwriteValues(IReadOnlyCollection<PacingAttributeValue> values)
        {
            Clear();
            foreach (PacingAttributeValue value in values)
            {
                _values.Add(value);
                _valuesMap[value.Definition] = value;
            }
        }

        public void Clear()
        {
            _values.Clear();
            _valuesMap.Clear();
        }

        public PacingAttributeValue Get(PacingAttributeDefinition attribute)
        {
            if (!_valuesMap.TryGetValue(attribute, out PacingAttributeValue value))
            {
                throw new KeyNotFoundException($"Attribute {attribute.Name} not found in set");
            }
            return value;
        }

        public void Add(PacingAttributeValue value)
        {
            _values.Add(value);
            _valuesMap[value.Definition] = value;
        }
    }
}
