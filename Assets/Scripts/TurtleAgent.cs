using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.Rendering;
using System.Collections;
public class TurtleAgent : Agent
{
    [SerializeField] private Transform _goal;
    [SerializeField] private Renderer _groundRenderer;
    [SerializeField] private float _moveSpeed = 1.5f;
    [SerializeField] private float _rotationSpeed = 100f;
    private Renderer _renderer;
    [HideInInspector]public int _currentEpisode = 0;
    [HideInInspector]public float _cumulativeReward = 0f;

    Color _defaultGroundColor;
    private Coroutine _groundFlashCroutine;
    public override void Initialize()
    {
        Debug.Log("Initialize()");
        _renderer = GetComponent<Renderer>();
        _currentEpisode = 0;
        _cumulativeReward = 0f;
        if(_groundRenderer != null)
        {
            _defaultGroundColor=_groundRenderer.material.color;
        }
    }
    public override void OnEpisodeBegin()
    {
        if (_groundRenderer != null && _cumulativeReward != 0) 
        {
            Color flashcolor=(_cumulativeReward<0 ? Color.red : Color.green);
            if (_groundFlashCroutine != null) 
            {
                StopCoroutine(_groundFlashCroutine);
            }
            _groundFlashCroutine = StartCoroutine(FlashGround(flashcolor, 0.3f));
        }
        Debug.Log("OnEpispodeBegin()");
        _currentEpisode++;
        _cumulativeReward = 0f;
        _renderer.material.color = Color.blue;
        SpawnObjects();
    }
    private IEnumerator FlashGround(Color targetColor,float time)
    {
        float elapsedTime = 0;
        _groundRenderer.material.color = targetColor;
        while (elapsedTime < time) 
        {
            elapsedTime += Time.deltaTime;
            _groundRenderer.material.color=Color.Lerp(targetColor,_defaultGroundColor,elapsedTime/time);
            yield return null;
        }
    }
    private void SpawnObjects()
    {
        transform.localRotation = Quaternion.identity;

        transform.localPosition=new Vector3(0,0.15f,0);

        float randomAngle = Random.Range(0f, 360f);
        Vector3 randomDirection = Quaternion.Euler(0f, randomAngle, 0f)*Vector3.forward;

        float randomDistance = Random.Range(1f, 2.5f);
        Vector3 goalPosition = transform.localPosition + randomDirection * randomDistance;

        _goal.localPosition = new Vector3(goalPosition.x, 0.3f, goalPosition.z);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        float goalPosX_normalized = _goal.localPosition.x / 5f;
        float goalPosY_normalized = _goal.localPosition.y / 5f;

        float turtlePosX_normalized = transform.localPosition.x / 5f;
        float turtlePosY_normalized = transform.localPosition.y / 5f;

        float turtleRotation_normalized = (transform.localRotation.eulerAngles.y / 360f) * 2f - 1f;
        sensor.AddObservation(goalPosX_normalized);
        sensor.AddObservation(goalPosY_normalized);
        sensor.AddObservation(turtlePosX_normalized);
        sensor.AddObservation(turtlePosY_normalized);
        sensor.AddObservation(turtleRotation_normalized);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;//yapma hicbisey
        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActionsOut[0] = 3;
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveAgent(actions.DiscreteActions);
        AddReward(-2f / MaxStep);
        _cumulativeReward = GetCumulativeReward();
    }
    private void MoveAgent(ActionSegment<int> act)
    {
        var action = act[0];
        switch (action)
        {
            case 1:
                transform.position += transform.forward * _moveSpeed * Time.deltaTime;
                break;
            case 2:
                transform.Rotate(0f, -_rotationSpeed * Time.deltaTime, 0f);
                break;
            case 3:
                transform.Rotate(0f, _rotationSpeed * Time.deltaTime, 0f);
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            _renderer.material.color = Color.green;
            GoalReached();
        }
    }
    private void GoalReached()
    {
        AddReward(1.0f);
        _cumulativeReward = GetCumulativeReward();
        EndEpisode();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.05f);
            if (_renderer != null)
            {
                _renderer.material.color = Color.red;
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {

        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.01f*Time.fixedDeltaTime);
            
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {  
            if (_renderer != null)
            {
                _renderer.material.color = Color.blue;
            }
        }
    }
}
