using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ContactsApp.ViewModels.Abstractions
{
    public abstract class BaseBindableObject<TData> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();
        private readonly Dictionary<string, PropertyChangedEventArgs> _propertyChangedArgs = new Dictionary<string, PropertyChangedEventArgs>();
        private readonly Dictionary<string, List<PropertyChangedEventArgs>> _propertyDependencies = new Dictionary<string, List<PropertyChangedEventArgs>>();

        protected TProperty GetProperty<TProperty>(TProperty defaultValue = default, [CallerMemberName] string propertyName = "")
        {
            if (!_properties.ContainsKey(propertyName))
                AddProperty(propertyName, defaultValue);

            return (TProperty)_properties[propertyName];
        }

        protected BaseBindableObject<TData> SetProperty<TProperty>(TProperty value, Action<TProperty> onChanged = null, [CallerMemberName] string propertyName = "")
        {
            if (!_properties.TryGetValue(propertyName, out object storedValue))
                AddProperty(propertyName, value);
            else if (storedValue is object && storedValue.Equals(value))
                return this;

            _properties[propertyName] = value;
            PropertyChanged?.Invoke(this, _propertyChangedArgs[propertyName]);
            onChanged?.Invoke(value);

            if (_propertyDependencies.TryGetValue(propertyName, out List<PropertyChangedEventArgs> alsoNotifyFor))
            {
                foreach (var dependentPropertyArgs in alsoNotifyFor)
                    PropertyChanged?.Invoke(this, dependentPropertyArgs);
            }

            return this;
        }

        internal BaseBindableObject<TData> Notify(string propertyName, params string[] otherPropertyNames)
        {
            if (!_propertyChangedArgs.ContainsKey(propertyName))
                _propertyChangedArgs.Add(propertyName, new PropertyChangedEventArgs(propertyName));

            PropertyChanged?.Invoke(this, _propertyChangedArgs[propertyName]);

            foreach (var otherPropertyName in otherPropertyNames)
            {
                if (!_propertyChangedArgs.ContainsKey(otherPropertyName))
                    _propertyChangedArgs.Add(otherPropertyName, new PropertyChangedEventArgs(otherPropertyName));

                PropertyChanged?.Invoke(this, _propertyChangedArgs[otherPropertyName]);
            }

            return this;
        }

        internal BaseBindableObject<TData> Notify<TProperty>(Expression<Func<TData, TProperty>> propertyExpression)
        {
            if (!(propertyExpression.Body is MemberExpression property))
                throw new ArgumentException($"Expression '{propertyExpression}' does not refer to a property.");

            return Notify(property.Member.Name);
        }

        private void AddProperty(string propertyName, object defaultValue)
        {
            if (!_propertyChangedArgs.ContainsKey(propertyName))
                _propertyChangedArgs.Add(propertyName, new PropertyChangedEventArgs(propertyName));

            _properties.Add(propertyName, defaultValue);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
