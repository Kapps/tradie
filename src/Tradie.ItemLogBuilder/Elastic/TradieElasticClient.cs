using Nest;
using Tradie.Common;
using Tradie.ItemLogBuilder.Elastic.Models;
using LoggedItem = Tradie.Analyzer.Entities.LoggedItem;

namespace Tradie.ItemLogBuilder.Elastic;

public class TradieElasticClient : Nest.ElasticClient {
	public TradieElasticClient(IConnectionSettingsValues settings) : base(settings) { }

	public TradieElasticClient() : this(CreateSettings()) {
		if(TradieConfig.IsTestEnvironment) {
			this.DeleteByQuery<LoggedItem>(c => c.MatchAll());
			this.DeleteByQuery<LoggedTab>(c => c.MatchAll());
		}

		//this.Indices.Delete(TradieConfig.IsTestEnvironment ? "tabs_test" : "tabs", c => c);
		this.Indices.PutMapping<LoggedTab>(m => m.AutoMap());
	}

	private static IConnectionSettingsValues CreateSettings() {
		var settings = new ConnectionSettings(new Uri(TradieConfig.ElasticServiceUrl));
		settings.DefaultMappingFor<LoggedItem>(m =>
			m.IdProperty(c => c.IdHash)
				.IndexName(TradieConfig.IsTestEnvironment ? "items_test" : "items")
		);
		settings.DefaultMappingFor<LoggedTab>(m =>
			m.IdProperty(c => c.StashTabId)
				.IndexName(TradieConfig.IsTestEnvironment ? "tabs_test" : "tabs")
		);
		#if DEBUG
		settings.EnableDebugMode();
		#endif
		settings.EnableApiVersioningHeader();
		settings.ServerCertificateValidationCallback((_, _, _, _) => true);
		settings.BasicAuthentication(TradieConfig.ElasticUser, TradieConfig.ElasticPassword);
		settings.ThrowExceptions();
		return settings;
	}
}