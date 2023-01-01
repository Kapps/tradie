using Constructs;
using HashiCorp.Cdktf.Providers.Aws.CloudfrontDistribution;
using HashiCorp.Cdktf.Providers.Aws.DataAwsIamPolicyDocument;
using HashiCorp.Cdktf.Providers.Aws.Route53Record;
using HashiCorp.Cdktf.Providers.Aws.S3Bucket;
using HashiCorp.Cdktf.Providers.Aws.S3BucketAcl;
using HashiCorp.Cdktf.Providers.Aws.S3BucketPolicy;
using HashiCorp.Cdktf.Providers.Aws.S3BucketWebsiteConfiguration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Tradie.Infrastructure.Foundation;

namespace Tradie.Infrastructure.Client;

public class ClientStack : StackBase {
	public ClientStack(Construct scope, string id, ResourceConfig config, FoundationStack foundation) : base(scope, id,
		config) {
		string bucketName = $"{config.Environment}.tradie.io";
		string bucketPath = $"s3://{bucketName}/";
		string apiBase = $"https://{config.Environment}-api.tradie.io";
		string staticBaseNoScheme = $"{bucketName}.s3-website.{config.Region}.amazonaws.com";

		var bucket = new S3Bucket(this, $"tradie-{config.Region}-client-bucket", new S3BucketConfig() {
			Bucket = bucketName
		});
		this.ExcludeFromPrefixing(bucket);


		var bucketWebConfig = new S3BucketWebsiteConfiguration(this, "client-bucket-web",
			new S3BucketWebsiteConfigurationConfig() {
				Bucket = bucket.Bucket,
				IndexDocument = new S3BucketWebsiteConfigurationIndexDocument() {
					Suffix = "index.html"
				},
			});

		var policy = new DataAwsIamPolicyDocument(this, "client-policy-doc", new DataAwsIamPolicyDocumentConfig() {
			Statement = new[] {
				new DataAwsIamPolicyDocumentStatement() {
					Actions = new[] {
						"s3:GetObject"
					},
					Principals = new[] {
						new DataAwsIamPolicyDocumentStatementPrincipals() {
							Identifiers = new[] {"*"},
							Type = "AWS"
						}
					},
					Resources = new[] {
						$"{bucket.Arn}/*"
					},
				}
			}
		});

		_ = new S3BucketAcl(this, "client-acl", new S3BucketAclConfig() {
			Bucket = bucket.Bucket,
			Acl = "public-read"
		});

		_ = new S3BucketPolicy(this, "client-policy", new S3BucketPolicyConfig() {
			Bucket = bucketWebConfig.Bucket,
			Policy = policy.Json
		});

		var cf = new CloudfrontDistribution(this, "client-distro", new CloudfrontDistributionConfig() {
			Origin = new[] {
				new CloudfrontDistributionOrigin() {
					//DomainName = staticBaseNoScheme,
					//DomainName = bucketName,
					DomainName = $"{bucketName}.s3.{config.Region}.amazonaws.com",
					OriginId = $"S3-{bucketName}",
					CustomOriginConfig = new CloudfrontDistributionOriginCustomOriginConfig() {
						HttpPort = 80,
						HttpsPort = 443,
						OriginProtocolPolicy = "http-only",
						OriginSslProtocols = new[] { "TLSv1.2"},
					},
				},
			},
			Enabled = true,
			Aliases = new[] { bucketName },
			IsIpv6Enabled = false,
			DefaultRootObject = "index.html",
			PriceClass = "PriceClass_100",
			DefaultCacheBehavior = new CloudfrontDistributionDefaultCacheBehavior() {
				Compress = true,
				AllowedMethods = new[] { "GET", "HEAD" },
				CachedMethods = new[] { "GET", "HEAD" },
				MaxTtl = 7200,
				ViewerProtocolPolicy = "allow-all",
				TargetOriginId = $"S3-{bucketName}",
				ForwardedValues = new CloudfrontDistributionDefaultCacheBehaviorForwardedValues() {
					QueryString = false,
					Cookies = new CloudfrontDistributionDefaultCacheBehaviorForwardedValuesCookies() {
						Forward = "none"
					}
				}
			},
			ViewerCertificate = new CloudfrontDistributionViewerCertificate() {
				AcmCertificateArn = foundation.Alb.HttpsUsCertificate.Arn,
				SslSupportMethod = "sni-only"
			},
			Restrictions = new CloudfrontDistributionRestrictions() {
				GeoRestriction = new CloudfrontDistributionRestrictionsGeoRestriction() {
					RestrictionType = "none"
				}
			}
		});

		var cnameRecord = new Route53Record(this, "client-route53", new Route53RecordConfig() {
			ZoneId = foundation.Alb.HostedZoneId,
			Type = "CNAME",
			Name = bucketName,
			Ttl = 60,
			Records = new[] {
				cf.DomainName
			}
		});

		if(config.StacksToDeploy.Contains("client", StringComparer.InvariantCultureIgnoreCase)) {
			string clientDir = Path.GetFullPath(Path.Combine(config.BaseDirectory, "Tradie.Client/"));
			this.RunShell(new ProcessStartInfo("yarn", "build") {
				Environment = {
					{"REACT_APP_API_URL", apiBase},
					{"REACT_APP_STATIC_URL", $"https://{staticBaseNoScheme}" }
				},
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				WorkingDirectory = clientDir
			});

			string buildDir = Path.Combine(clientDir, "build/");
			RunShell(new ProcessStartInfo("aws", $"s3 sync \"{buildDir}\" \"{bucketPath}\"") {
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				WorkingDirectory = clientDir
			});
		}
	}

	private void RunShell(ProcessStartInfo psi) {
		var buildProc = Process.Start(psi)!;
		buildProc.WaitForExit();
		Console.WriteLine(buildProc.StandardOutput.ReadToEnd());
		Console.WriteLine(buildProc.StandardError.ReadToEnd());
	}
}