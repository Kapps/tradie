using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Acm;
using HashiCorp.Cdktf.Providers.Aws.Elb;
using HashiCorp.Cdktf.Providers.Aws.Route53;
using HashiCorp.Cdktf.Providers.Aws.Vpc;
using System;
using System.Linq;

namespace Tradie.Infrastructure.Foundation;

public class Alb {

	/// <summary>
	/// The ID of the hosted route 53 zone.
	/// </summary>
	public readonly string HostedZoneId = "Z04495081AOUFV3T91DNR"; // External hosted zone iD.
	
	/// <summary>
	/// Security group to allow traffic from 80 and 443.
	/// </summary>
	public readonly SecurityGroup HttpTrafficSecurityGroup;
	
	/// <summary>
	/// The load balancer for incoming HTTP(s) requests.
	/// </summary>
	public readonly HashiCorp.Cdktf.Providers.Aws.Elb.Alb HttpAlb;

	public readonly AlbListener HttpsListener;
	
	/// <summary>
	/// HTTPS certificate for the webserver.
	/// </summary>
	public readonly AcmCertificate HttpsCertificate;

	/// <summary>
	/// An HTTPS certificate located in the US East region.
	/// </summary>
	public readonly AcmCertificate HttpsUsCertificate;


	public Alb(TerraformStack stack, Network network, ResourceConfig config) {
		string appHost = $"{config.Environment}-api.tradie.io";
		var awsUseProvider = ((FoundationStack)stack).AwsUsProvider;
		
		var httpTrafficIngresses = new[] {
			new SecurityGroupIngress() {
				CidrBlocks = new[] {"0.0.0.0/0"},
				FromPort = 80, ToPort = 80,
				Protocol = "tcp",
				SecurityGroups = Array.Empty<string>(),
				SelfAttribute = false,
				PrefixListIds = Array.Empty<string>(),
				Description = "Allow all HTTP traffic.",
				Ipv6CidrBlocks = Array.Empty<string>()
			},
			new SecurityGroupIngress() {
				CidrBlocks = new[] {"0.0.0.0/0"},
				FromPort = 443, ToPort = 443,
				Protocol = "tcp",
				SecurityGroups = Array.Empty<string>(),
				SelfAttribute = false,
				PrefixListIds = Array.Empty<string>(),
				Description = "Allow all HTTPs traffic.",
				Ipv6CidrBlocks = Array.Empty<string>()
			}
		};

		this.HttpTrafficSecurityGroup = new(stack, "http-sg", new SecurityGroupConfig() {
			Name = "http-sg",
			Egress = new[] { network.AllOutgoingTrafficEgress },
			Ingress = httpTrafficIngresses,
			VpcId = network.Vpc.Id
		});

		this.HttpAlb = new(stack, "http-alb", new AlbConfig() {
			Internal = false,
			Subnets = network.PublicSubnets.Select(c=>c.Id).ToArray(),
			Name = "http-input",
			DropInvalidHeaderFields = false,
			LoadBalancerType = "application",
			SecurityGroups = new[] {
				this.HttpTrafficSecurityGroup.Id
			},
			IpAddressType = "ipv4",
			EnableHttp2 = false
		});

		this.HttpsCertificate = this.CreateCert(stack, "https-cert", null);
		this.HttpsUsCertificate = this.CreateCert(stack, "https-global-cert", awsUseProvider);

		var validationOpt = this.HttpsCertificate.DomainValidationOptions.Get(0);
		
		var record = new Route53Record(stack, "https-validation-record", new Route53RecordConfig() {
			Name = validationOpt.ResourceRecordName,
			Type = validationOpt.ResourceRecordType,
			ZoneId = HostedZoneId,
			Ttl = 60,
			Records = new[] {
				validationOpt.ResourceRecordValue
			}
		});

		var certificateValidation = new AcmCertificateValidation(stack, "https-validation", new AcmCertificateValidationConfig() {
			CertificateArn = this.HttpsCertificate.Arn,
			ValidationRecordFqdns = new[] {
				record.Fqdn
			}
			// Provider = awsUseProvider
		});

		var useCertificationValidation = new AcmCertificateValidation(stack, "https-global-validation", new AcmCertificateValidationConfig() {
			CertificateArn = this.HttpsUsCertificate.Arn,
			Provider = awsUseProvider,
			ValidationRecordFqdns = new[] {
				record.Fqdn
			}
		});

		var httpListener = new AlbListener(stack, "http-listener", new AlbListenerConfig() {
			LoadBalancerArn = this.HttpAlb.Arn,
			Port = 80,
			Protocol = "HTTP",
			DefaultAction = new[] {
				new AlbListenerDefaultAction() {
					Type = "redirect",
					Redirect = new AlbListenerDefaultActionRedirect() {
						Port = "443",
						Protocol = "HTTPS",
						StatusCode = "HTTP_301"
					}
				}
			},
			DependsOn = new[] { certificateValidation }
		});

		this.HttpsListener = new AlbListener(stack, "https-listener", new AlbListenerConfig() {
			LoadBalancerArn = this.HttpAlb.Arn,
			Port = 443,
			Protocol = "HTTPS",
			CertificateArn = this.HttpsCertificate.Arn,
			DefaultAction = new[] {
				new AlbListenerDefaultAction() {
					Type = "fixed-response",
					FixedResponse = new AlbListenerDefaultActionFixedResponse() {
						ContentType = "text/plain",
						MessageBody = "Default 404.",
						StatusCode = "404"
					}
				}
			},
			DependsOn = new[] { certificateValidation }
		});
		
		var httpsListenerCertConfig = new AlbListenerCertificate(stack, "alb-https-cert", new AlbListenerCertificateConfig() {
			CertificateArn = this.HttpsCertificate.Arn,
			ListenerArn = this.HttpsListener.Arn
		});

		var rootRecord = new Route53Record(stack, "root-record", new Route53RecordConfig() {
			ZoneId = HostedZoneId,
			Type = "CNAME",
			Name = appHost,
			Ttl = 60,
			Records = new[] {
				this.HttpAlb.DnsName
			},
		});
	}

	private AcmCertificate CreateCert(TerraformStack stack, string id, TerraformProvider provider) {
		return new AcmCertificate(stack, id, new AcmCertificateConfig() {
			DomainName = "tradie.io",
			ValidationMethod = "DNS",
			SubjectAlternativeNames = new[] {
				"*.tradie.io"
			},
			Lifecycle = new TerraformResourceLifecycle() {
				CreateBeforeDestroy = true
			},
			ValidationOption = new[] {
				new AcmCertificateValidationOption() {
					DomainName = "tradie.io",
					ValidationDomain = "tradie.io"
				},
			},
			Provider = provider
		});
	}
}