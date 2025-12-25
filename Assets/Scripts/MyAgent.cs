using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class MyAgent : Agent
{
    public Transform target;
    public float speed = 2f;

    public override void OnEpisodeBegin()
    {
        // Reset agent position
        transform.localPosition = Vector3.zero;

        // Randomize target position
        target.localPosition = new Vector3(
            Random.Range(-4f, 4f),
            0,
            Random.Range(-4f, 4f)
        );
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition +=
            new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;

        float distance = Vector3.Distance(
            transform.localPosition,
            target.localPosition
        );

        if (distance < 1.5f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        if (transform.localPosition.magnitude > 6)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }
}
