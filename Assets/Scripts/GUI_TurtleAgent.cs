using System;
using UnityEngine;

public class GUI_TurtleAgent : MonoBehaviour
{
    [SerializeField] TurtleAgent turtleAgent;
    private GUIStyle _defaultStyle=new GUIStyle();
    private GUIStyle _positiveStyle=new GUIStyle();
    private GUIStyle _negativeStyle=new GUIStyle();
    void Start()
    {
        _defaultStyle.fontSize = 20;
        _defaultStyle.normal.textColor = Color.yellow;

        _positiveStyle.fontSize = 20;
        _positiveStyle.normal.textColor = Color.green;

        _negativeStyle.fontSize = 20;
        _negativeStyle.normal.textColor = Color.red;
    }
    private void OnGUI()
    {
        string debugEpisode = "Episode: " + turtleAgent._currentEpisode + " - Step: " + turtleAgent.StepCount;
        string debugReward="Reward: "+turtleAgent._cumulativeReward.ToString();

        GUIStyle rewardStyle = turtleAgent._cumulativeReward < 0 ? _negativeStyle : _positiveStyle;
        GUI.Label(new Rect(20, 20, 500, 30), debugEpisode, _defaultStyle);
        GUI.Label(new Rect(20, 60, 500, 30), debugReward, rewardStyle);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
