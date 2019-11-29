using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLeaderboard : MonoBehaviour
{
    public Text place_1;
    public Text place_2;
    public Text place_3;
    public Text place_4;

    private void Start()
    {
        Leaderboard.Reset();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        List<string> places = Leaderboard.GetPlaces();

        place_1.text = "N/A";
        place_2.text = "N/A";
        place_3.text = "N/A";
        place_4.text = "N/A";

        if (places.Count == 4)
        { 
            place_1.text = places[0];
            place_2.text = places[1];
            place_3.text = places[2];
            place_4.text = places[3];        
        }
    }
}
