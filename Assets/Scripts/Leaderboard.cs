using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct PlayerStats
{
    public string name;
    public int position;
    public float time;

    public PlayerStats(string newName, int newPosition, float newTime)
    {
        name = newName;
        position = newPosition;
        time = newTime;
    }
}

public class Leaderboard
{
    static Dictionary<int, PlayerStats> lb = new Dictionary<int, PlayerStats>();
    static int carsRegistered = -1;

    public static void Reset()
    {
        lb.Clear();
        carsRegistered = -1;
    }

    public static int RegisterCar(string name)
    {
        carsRegistered++;
        lb.Add(carsRegistered, new PlayerStats(name, 0, 0));
        return carsRegistered;
    }

    public static void SetPosition(int rego, int lap, int checkpoint, float time)
    {
        int position = lap * 1000 + checkpoint;
        lb[rego] = new PlayerStats(lb[rego].name, position, time);
    }

    public static bool GetEntry(int rego, out PlayerStats playerStats)
    {
        foreach (KeyValuePair<int, PlayerStats> pos in lb.OrderByDescending(key => key.Value.position).ThenBy(key => key.Value.time))
        {
            if (pos.Key == rego)
            {
                playerStats = pos.Value;
                return true;
            }
        }
        playerStats = new PlayerStats();
        return false;
    }

    public static string GetPosition(int rego)
    {
        int index = 0;
        foreach (KeyValuePair<int, PlayerStats> pos in lb.OrderByDescending(key => key.Value.position).ThenBy(key => key.Value.time))
        {
            index++;
            if (pos.Key == rego)
            {
                switch (index)
                {
                    case 1: return "1st";
                    case 2: return "2nd";
                    case 3: return "3rd";
                    case 4: return "4th";
                }
            }
        }
        return "N/A";
    }

    public static List<string> GetPlaces()
    {
        List<string> places = new List<string>();

        foreach (KeyValuePair<int, PlayerStats> pos in lb.OrderByDescending(key => key.Value.position).ThenBy(key => key.Value.time))
        {
            places.Add(GetPosition(pos.Key) + " - " + pos.Value.name);
        }

        return places;
    }
}
