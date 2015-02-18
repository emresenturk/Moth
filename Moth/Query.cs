using System;
using Moth.Annotations;

namespace Moth
{
    public class Query : IQuery
    {
        private readonly QueryParameterCollection parameters;

        public Query()
        {
            parameters = new QueryParameterCollection();
        }

        public Query(string command)
        {
            parameters = new QueryParameterCollection();
            Command = command;
        }

        public Query(string command, params Parameter[] parameters)
        {
            Command = command;
            this.parameters = new QueryParameterCollection(parameters);
        }

        public static Query Create(string command, params Parameter[] parameters)
        {
            return new Query(command, parameters);
        }

        public static Query Create()
        {
            return new Query();
        }

        public string Command { get; set; }

        public QueryParameterCollection Parameters
        {
            get { return parameters; }
        }

        public void AddParameter([NotNull]Parameter parameter)
        {
            parameters.Add(parameter);
        }

        public void AddParameter([NotNull]string name, object value)
        {
            parameters.Add(name, value);
        }

        public void AddParameter([NotNull]string name, object value, [NotNull]Type valueType)
        {
            parameters.Add(name, value, valueType);
        }

        public bool RemoveParameter(string name)
        {
            return parameters.Remove(name);
        }
    }
}