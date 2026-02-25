using Server.Application.Port.Output.StaticData;

namespace Server.Domain.Entity;

public class GameData
{
    public int UserId { get; }
    public string Name { get; private set; }
    public int Exp { get; private set; }
    public int Level { get; private set; }

    public GameData(int userId, string name, int exp, int level)
    {
        UserId = userId;
        Name = name;
        Exp = exp;
        Level = level;
    }
    
    public static GameData Create(int userId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Invalid name");

        return new GameData(userId, name, exp: 0, level: 1);
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be empty");
        
        Name = newName;
    }

    public void GainExp(int amount, ILevelDefinitionStore store, Action<int> onLevelUp)
    {
        Exp += amount;

        while (true)
        {
            var levelDef = store.GetLevelDefinition(Level);
            if (levelDef == null || Exp < levelDef.MaxExp)
                break;

            Exp -= levelDef.MaxExp;
            Level++;
            
            onLevelUp?.Invoke(levelDef.MaxActionPoint);
        }
    }
}