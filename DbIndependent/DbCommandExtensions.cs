using System.Data.Common;

namespace DbIndependent
{
    public static class DbCommandExtensions
    {
        public static void AddWithValue(
            this DbParameterCollection parameters,
            string parameterName,
            object value,
            DbParameter parameter)
        {
            parameter.ParameterName = parameterName;
            parameter.Value = value;

            if (!parameters.Contains(parameter))
                parameters.Add(parameter);
        }

        public static void AddWithValue(
            this DbParameterCollection parameters,
            string parameterName,
            object value,
            DbProviderFactory factory)
        {
            AddWithValue(parameters, parameterName, value, factory.CreateParameter());
        }

        public static void AddWithValue(
           this DbParameterCollection parameters,
           string parameterName,
           object value,
           DbCommand command)
        {
            AddWithValue(parameters, parameterName, value, command.CreateParameter());
        }

        public static void AddParameterWithValue(
            this DbCommand command,
            string parameterName,
            object value)
        {
            AddWithValue(command.Parameters, parameterName, value, command);
        }
    }
}