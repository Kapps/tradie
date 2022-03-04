using Amazon.JSII.Runtime.Deputy;
using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws.Rds;
using HashiCorp.Cdktf.Providers.Aws.S3;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Tradie.Infrastructure.Aspects {
	/// <summary>
	/// An aspect that operates on every node to prefix them with an environment key.
	/// </summary>
	public class EnvironmentPrefixerAspect : DeputyBase, IAspect {
		public EnvironmentPrefixerAspect(string prefix) {
			this.prefix = prefix;
		}

		public void Visit(IConstruct node) {
			bool updated = false;
			var prefixedProps = new[] {"Name", "Bucket", "Family", "Identifier", "ClusterIdentifier"};
			foreach(var prop in prefixedProps) {
				if(prop == "Bucket" && node.GetType() != typeof(S3Bucket)) {
					continue;
				}

				if(prop == "Name" && node.GetType() == typeof(DbInstance)) {
					// This is weird, because it's the name of the database to create, not the instance name.
					continue;
				}

				if(prop == "Family" && node.GetType() == typeof(DbParameterGroup)) {
					// Really need a better way of doing this...
					continue;
				}
				updated |= TryPrefix(node, prop);
			}
			
			if(!updated) {
				Console.WriteLine($"No prefix rewrites were performed on {node.GetType().Name} {node}.");
			}
		}

		private bool TryPrefix(object obj, string property) {
			string inputProp = $"{property}Input";
			if(!HasProperties(obj, property, inputProp)) {
				return false;
			}

			string currVal = (string)obj.GetType().GetProperty(inputProp).GetValue(obj);
			if(String.IsNullOrWhiteSpace(currVal)) {
				Console.WriteLine($"Skipped prefixing {obj.GetType().Name} {obj} as the {property} property was null or empty.");
				return false;
			}
				
			string newVal = $"{this.prefix}-{currVal}";
			obj.GetType().GetProperty(property).SetValue(obj, newVal, null);
			Console.WriteLine($"Prefixed {obj.GetType().Name} {obj} to {newVal}");
			return true;
		}

		private static bool HasProperties(object obj, params string[] properties) {
			if(obj == null) {
				return false;
			}

			foreach(var prop in properties) {
				if(obj.GetType().GetProperty(prop) == null) {
					return false;
				}
			}

			return true;
		}

		private string prefix;
	}
}
