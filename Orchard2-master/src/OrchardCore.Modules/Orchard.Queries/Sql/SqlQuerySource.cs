using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Fluid;
using Newtonsoft.Json.Linq;
using Orchard.Liquid;
using YesSql;

namespace Orchard.Queries.Sql
{
    public class SqlQuerySource : IQuerySource
    {
        private readonly IStore _store;
        private readonly ILiquidTemplateManager _liquidTemplateManager;
        private readonly ISession _session;

        public SqlQuerySource(
            IStore store,
            ILiquidTemplateManager liquidTemplateManager,
            ISession session)
        {
            _store = store;
            _liquidTemplateManager = liquidTemplateManager;
            _session = session;
        }

        public string Name => "Sql";

        public Query Create()
        {
            return new SqlQuery();
        }

        public async Task<object> ExecuteQueryAsync(Query query, IDictionary<string, object> parameters)
        {
            var sqlQuery = query as SqlQuery;

            var templateContext = new TemplateContext();
            foreach (var parameter in parameters)
            {
                templateContext.SetValue(parameter.Key, parameter.Value);
            }

            var tokenizedQuery = await _liquidTemplateManager.RenderAsync(sqlQuery.Template, templateContext);

            var connection = _store.Configuration.ConnectionFactory.CreateConnection();
            var dialect = SqlDialectFactory.For(connection);

            var results = new List<JObject>();

            if (!SqlParser.TryParse(tokenizedQuery, dialect, _store.Configuration.TablePrefix, out var rawQuery, out var rawParameters, out var messages))
            {
                return results;
            }
                        
            if (sqlQuery.ReturnDocuments)
            {
                IEnumerable<int> documentIds;

                using (connection)
                {
                    connection.Open();
                    documentIds = await connection.QueryAsync<int>(rawQuery, rawParameters);
                }

                var documents = await _session.GetAsync<object>(documentIds.ToArray());

                foreach (var document in documents)
                {
                    results.Add(JObject.FromObject(document));
                }
            }
            else
            {
                IEnumerable<dynamic> queryResults;

                using (connection)
                {
                    connection.Open();
                    queryResults = await connection.QueryAsync(rawQuery, rawParameters);
                }

                foreach (var document in queryResults)
                {
                    results.Add(JObject.FromObject(document));
                }
            }

            return results.ToArray();
        }
    }
}
