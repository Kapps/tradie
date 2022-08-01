using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;

namespace Tradie.Infrastructure.Packaging;

public class DockerBuild {
	public void ThrowIfError() {
		if(this.errors.Any()) {
			throw new AggregateException(this.errors.Select(err =>
				new ApplicationException($"[{err.Code}]: {err.Message}")));
		}		
	}

	public string ImageId {
		get {
			if(this._imageId == null) {
				throw new DataException("Expected an image id to be found, but none was.");
			}

			return this._imageId;
		}
	}

	/*public string ImageManifest {
		get {
			if(this._manifest == null) {
				throw new DataException()
			}
		}
	}*/

	public Progress<JSONMessage> CreateBuildProgressAction() {
		return new(message => {
			if(message.Error != null) {
				Console.Error.WriteLine(message.Error.Message);
				this.errors.Add(message.Error);
				return;
			}
			
			Console.Write(message.Stream);
			if(message.Aux?.ExtensionData?.TryGetValue("ID", out object unformattedId) == true) {
				string id = (string)unformattedId;
				id = id.Substring(id.IndexOf(":", StringComparison.InvariantCultureIgnoreCase) + 1);
				this._imageId = id;
			}
		});
	}

	public Progress<JSONMessage> CreateProgressAction() {
		return new(message => {
			if(message.Error != null) {
				Console.Error.WriteLine(message.Error.Message);
				this.errors.Add(message.Error);
				return;
			}
			
			Console.Write(message.Stream);
		});
	}

	private string _imageId;
	private List<JSONError> errors = new();
}