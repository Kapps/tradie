namespace Tradie.Scanner;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

/// <summary>
/// An interface to allow for reading stash tab changesets from a stream.
/// </summary>
public interface IChangeSetParser {
	/// <summary>
	/// Efficiently reads characters from the stream until the next changeset ID can be parsed, without parsing the entirety of the changeset.
	/// </summary>
	Task<ChangeSetDetails> ReadHeader(Stream stream);
	/// <summary>
	/// Copies the stash tabs from a changeset stream that has had the header read via ReadHeader.
	/// The changeset stream is read from fully at the end of this method.
	/// </summary>
	Task ReadChanges(Stream changeSetStream, Stream outputStream);
}

public class ChangeSetParser : IChangeSetParser {
	public Task<ChangeSetDetails> ReadHeader(Stream stream) {
		int curr;
		string? nextChangeSet = null;
		ChangeSetReadState state = ChangeSetReadState.Initial;
		StringBuilder valueBuilder = new StringBuilder();
		char[] bom = new char[] { (char)239, (char)187, (char)191 };
		int bomIndex = 0;

		do {
			curr = stream.ReadByte();
			if(curr == -1) {
				throw new InvalidDataException("Reached EOF prior to reading changeset header.");
			}

			char c = (char)curr;
			if(c == '"') {
				state++;
				if(state == ChangeSetReadState.KeyStart || state == ChangeSetReadState.ValueStart) {
					valueBuilder.Clear();
				}
			}

			switch(state) {
				case ChangeSetReadState.Initial:
					if(bomIndex < bom.Length && bom[bomIndex] == c) {
						// Currently nothing special about the BOM, but skip through it in case.
						bomIndex++;
						continue;
					}
					continue;
				case ChangeSetReadState.KeyStart:
				case ChangeSetReadState.ValueStart:
					if(c != '"')
						valueBuilder.Append(c);
					break;
				case ChangeSetReadState.KeyEnd:
					string key = valueBuilder.ToString();
					if(key != "next_change_id") {
						throw new InvalidDataException($"Got an unexpected key of {key} instead of next_change_id.");
					}
					break;
				case ChangeSetReadState.ValueEnd:
					nextChangeSet = valueBuilder.ToString();
					if(String.IsNullOrWhiteSpace(nextChangeSet)) {
						throw new InvalidDataException("Got an empty next changeset somehow?");
					}
					break;
			}
		} while(nextChangeSet == null);

		return Task.FromResult(new ChangeSetDetails(nextChangeSet));
	}

	public async Task ReadChanges(Stream changeSetStream, Stream outputStream) {
		byte[] buff = new byte[81920];

		int bytesRead = await changeSetStream.ReadAsync(buff, 0, buff.Length);

		int arrayStart = Array.IndexOf(buff, (byte)'[');
		if(arrayStart == -1) {
			throw new InvalidDataException($"Unable to find stash tab opener in first {bytesRead} bytes.");
		}

		int copyStart = arrayStart;
		bool firstCopy = true;

		// At this point we're inside a {, and starting to copy at a [.
		// So we want to read until the second last character (]) in the stream, to prevent an extra trailing } in the tab array.

		while(bytesRead > 0) {
			// Leave one character left for the next read, as we don't want to write the last character.
			// So when we get to a 0 byte read, discard the remaining character.
			await outputStream.WriteAsync(buff, copyStart, bytesRead - copyStart);

			if(firstCopy) {
				copyStart = 1;
				firstCopy = false;
			} else {
				buff[0] = buff[bytesRead];
				copyStart = 0;
			}
			bytesRead = await changeSetStream.ReadAsync(buff, 1, buff.Length - 1);
			
			/*
			buff[0] = buff[bytesRead - (1 - leftover)];
			copyStart = 0;
			leftover = 1;
			bytesRead = await changeSetStream.ReadAsync(buff, 1, buff.Length - 1);*/
		}		
	}
}

enum ChangeSetReadState {
	Initial = 0,
	KeyStart = 1,
	KeyEnd = 2,
	ValueStart = 3,
	ValueEnd = 4,
}