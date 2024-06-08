using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class User
{
    public string username;
    public int ELO;
    public int gamesPlayed;
    public int gamesWon;
    public int XP;
    public int level;
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Username: ").Append(username).Append("\n");
        sb.Append("ELO: ").Append(ELO).Append("\n");
        sb.Append("Games Played: ").Append(gamesPlayed).Append("\n");
        sb.Append("Games Won: ").Append(gamesWon).Append("\n");
        sb.Append("XP: ").Append(XP).Append("\n");
        sb.Append("Level: ").Append(level).Append("\n");
        return sb.ToString();
    }
}
