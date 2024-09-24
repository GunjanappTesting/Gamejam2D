using System;
using System.Collections.Generic;

[Serializable]
public class LeaderboardResponse
{
    public string message;
    public int code;
    public List<LeaderboardEntry> leaderboard;
}

[Serializable]
public class LeaderboardEntry
{
    public string unique_id;
    public string name;
    public string country;
    public int level;
    public int points;
    public int score;
    public int xp;
    public string createdAt;
    public int rank;
}
