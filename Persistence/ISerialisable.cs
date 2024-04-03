namespace SadChromaLib.Persistence;

/// <summary>
/// A persistent object that writes to its own file.
/// </summary>
public interface ISerialisable
{
	void SerialisableWrite(PersistenceWriter writer);
	void SerialisableRead(PersistenceReader reader);
	string SerialisableGetFilename();
}

/// <summary>
/// A persistent object that can be used as building blocks for standalone serialisables
/// </summary>
public interface ISerialisableComponent
{
	void Serialise(PersistenceWriter writer);
	void Deserialise(PersistenceReader reader);
}