using Amazon.JSII.Runtime.Deputy;
using Constructs;
using HashiCorp.Cdktf;
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
			updated |= TryPrefix(node, "Name");
			updated |= TryPrefix(node, "Bucket");

			if(!updated) {
				Console.WriteLine($"No prefix rewrites were performed on {node}.");
			}
		}

		private bool TryPrefix(object obj, string property) {
			string inputProp = $"{property}Input";
			if(!HasProperties(obj, property, inputProp)) {
				return false;
			}

			string currVal = (string)obj.GetType().GetProperty(inputProp).GetValue(obj);
			if(String.IsNullOrWhiteSpace(currVal)) {
				Console.WriteLine($"Skipped prefixing {obj} as the {property} property was null or empty.");
				return false;
			}
				
			string newVal = $"{this.prefix}-{currVal}";
			obj.GetType().GetProperty(property).SetValue(obj, newVal, null);
			Console.WriteLine($"Prefixed {obj} to {newVal}");
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
