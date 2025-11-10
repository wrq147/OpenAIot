using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateAction.Core
{
    public class ExtentionDataCollection
    {
        private IDictionary<Type, object> _features = new Dictionary<Type, object>(10);
        public ExtentionDataCollection()
        {
        }
        public object this[Type key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                object result;
                if(_features.TryGetValue(key, out result))
                {
                    return result;
                }
                return null;
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                _features[key] = value;
            }
        }
        public TFeature Get<TFeature>()
        {
            return (TFeature)this[typeof(TFeature)];
        }

        public void Set<TFeature>(TFeature instance)
        {
            this[typeof(TFeature)] = instance;
        }
    }
}
