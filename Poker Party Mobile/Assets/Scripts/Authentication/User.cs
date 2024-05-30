using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int ELO { get; set; }
    public int gamesPlayed { get; set; }
    public int gamesWon { get; set; }
    public int XP { get; set; }
    public int Level { get; set; }
    public DateTime created_at { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Username: ").Append(Username).Append("\n");
        sb.Append("ELO: ").Append(ELO).Append("\n");
        sb.Append("Games Played: ").Append(gamesPlayed).Append("\n");
        sb.Append("Games Won: ").Append(gamesWon).Append("\n");
        sb.Append("XP: ").Append(XP).Append("\n");
        sb.Append("Level: ").Append(Level).Append("\n");
        sb.Append("Created At: ").Append(created_at).Append("\n");
        return sb.ToString();
    }
}
