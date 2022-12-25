#if false
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Ssm;

namespace Tradie.Infrastructure.Foundation;

public class DbParams {
	public DbParams(TerraformStack stack) {
		var ssmHost = new SsmParameter(stack, "ssm-dbhost", new SsmParameterConfig() {
			Name = "Config.DbHost",
			Type = "String",
			Value = "51.161.119.211"
		});

		var ssmUser = new SsmParameter(stack, "ssm-dbuser", new SsmParameterConfig() {
			Name = "Config.DbUser",
			Type = "String",
			Value = "tradie"
		});

		var ssmPass = new SsmParameter(stack, "ssm-dbpass", new SsmParameterConfig() {
			Name = "Config.DbPass",
			Type = "String",
			Value = "gfsnVb2OvSh91CyhikhZUEBjM58VAedQ"
		});

		var ssmName = new SsmParameter(stack, "ssm-dbname", new SsmParameterConfig() {
			Name = "Config.DbName",
			Type = "String",
			Value = "tradie"
		});
	}
}
#endif
