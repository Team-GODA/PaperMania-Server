namespace Server.Infrastructure.Persistence.Model;

public record PlayerGameData(
    int UserId, 
    string Name, 
    int Exp, 
    int Level = 1
);