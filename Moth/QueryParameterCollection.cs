using System;
using System.Collections;
using System.Collections.Generic;
using Moth.Annotations;

namespace Moth
{
    public class QueryParameterCollection : ICollection<Parameter>
    {
        private readonly IDictionary<string, Parameter> parameters;

        public QueryParameterCollection()
        {
            parameters = new Dictionary<string, Parameter>();
        }

        internal QueryParameterCollection(params Parameter[] parameters) : this()
        {
            if (parameters == null) return;
            foreach (var queryParameter in parameters)
            {
                AddParameter(queryParameter.Name, queryParameter);
            }
        }

        public object this[[NotNull] string name]
        {
            get { return parameters[name].Value; }
            set { parameters[name] = new Parameter(name, value); }
        }

        public void Add(Parameter parameter)
        {
            AddParameter(parameter.Name, parameter);
        }

        public void Clear()
        {
            parameters.Clear();
        }

        public bool Contains(Parameter item)
        {
            return parameters.ContainsKey(item.Name) && parameters[item.Name] == item;
        }

        public void CopyTo(Parameter[] array, int arrayIndex)
        {
            var index = arrayIndex;
            foreach (var queryParameter in parameters)
            {
                array[index] = new Parameter(queryParameter.Value.Name, queryParameter.Value.Value, queryParameter.Value.ValueType);
                index++;
            }
        }
        
        public int Count
        {
            get { return parameters.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add([NotNull]string name, object value)
        {
            AddParameter(name, new Parameter(name, value));
        }

        public void Add([NotNull]string name, object value, Type valueType)
        {
            AddParameter(name, new Parameter(name, value, valueType));
        }

        public bool Remove(Parameter item)
        {
            if (!parameters.ContainsKey(item.Name))
            {
                return false;
            }

            if (parameters[item.Name] != item)
            {
                return false;
            }

            parameters.Remove(item.Name);
            return true;
        }


        public bool Remove(string name)
        {
            if (!parameters.ContainsKey(name))
            {
                return false;
            }

            parameters.Remove(name);
            return true;
        }

        public IEnumerator<Parameter> GetEnumerator()
        {
            return parameters.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void AddParameter(string name, Parameter parameter)
        {
            if (parameters.ContainsKey(name))
            {
                throw new ArgumentException(
                        string.Format(@"A parameter with the name ""{0}"" already added.", name), "name");
            }

            parameters.Add(name, parameter);
        }
    }
}