namespace Tradie.Scanner;

using System;
using System.IO;
using System.Text;

/// <summary>
/// An interface to allow for reading stash tab changesets from a stream.
/// </summary>
public interface IChangeSetParser {
	/// <summary>
	/// Efficiently reads characters from the stream until the next changeset ID can be parsed, without parsing the entirety of the changeset.
	/// </summary>
	ChangeSetDetails ReadHeader(Stream stream);
}

public class ChangeSetParser : IChangeSetParser {
	public ChangeSetDetails ReadHeader(Stream stream) {
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

		return new ChangeSetDetails(nextChangeSet);
	}
}

enum ChangeSetReadState {
	Initial = 0,
	KeyStart = 1,
	KeyEnd = 2,
	ValueStart = 3,
	ValueEnd = 4,
}