using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Ssm;

namespace Tradie.Infrastructure.Foundation;

public class Cache {
	public Cache(TerraformStack stack) {
		var ssmHost = new SsmParameter(stack, "ssm-redis-host", new SsmParameterConfig() {
			Name = "Config.RedisHost",
			Type = "String",
			Value = "srv.tradie.io"
		});

		var ssmUser = new SsmParameter(stack, "ssm-redis-user", new SsmParameterConfig() {
			Name = "Config.RedisUser",
			Type = "String",
			Value = "tradie"
		});

		var ssmPass = new SsmParameter(stack, "ssm-redis-pass", new SsmParameterConfig() {
			Name = "Config.RedisPass",
			Type = "String",
			Value = "$4%Jdke9URcD!C^Y%2q2y^Dwf&TEE6JNYfKBK8vv@dH7G86E"
		});
	}
}