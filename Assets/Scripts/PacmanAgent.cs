using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PacmanAgent : Agent
{
    private Movement movement;
    public GameObject[] ghosts;
    public Transform walls;
    private GameManager manager;

    private struct PelletStruct
    {
        public int id;
        public Transform trans;
        public bool state;
    }

    PelletStruct[] pellets = new PelletStruct[240];
    PelletStruct[] powerPellets = new PelletStruct[4];
    int pid = 0;
    int ppid = 0;

    private bool bonusState = false;
    private float bonusDuration = 0.0f;
    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        movement = this.GetComponent<Movement>();
    }

    private void Update()
    {
        if (bonusDuration <= 0.0f)
        {
            bonusState = false;
        }
        else
            bonusDuration -= Time.deltaTime;
    }

    public override void OnEpisodeBegin()
    {
        //manager.NewGame();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Player position
        sensor.AddObservation(this.transform.position);
        // Player direction
        sensor.AddObservation(movement.direction);

        // Ghosts positions and distance to them
        foreach (GameObject ghost in ghosts)
        {
            sensor.AddObservation(ghost.transform.position);
        }

        // Pellets positions
        foreach (PelletStruct pellet in pellets)
        {
            sensor.AddObservation(pellet.trans.transform.position);
            sensor.AddObservation(pellet.state);
        }

        foreach (PelletStruct powerPellet in powerPellets)
        {
            sensor.AddObservation(powerPellet.trans.transform.position);
            sensor.AddObservation(powerPellet.state);
        }

        // Walls positions
        foreach (Transform wall in walls)
        {
            sensor.AddObservation(wall.transform.position);
        }
        // Powerup duration
        sensor.AddObservation(bonusDuration);
        sensor.AddObservation(bonusState);

        // Score
        sensor.AddObservation(manager.score);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Actions, size = 2
        var vectorAction = actions.ContinuousActions;
        Vector2 controlSignal = Vector2.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.y = vectorAction[1];

        if (controlSignal.x <= 0) {
            // Left
            movement.SetDirection(Vector2.left);
        }
        else if (controlSignal.y <= 0) {
            // Up
            movement.SetDirection(Vector2.up);
        }
        else if (controlSignal.x > 0) {
            // Right
            movement.SetDirection(Vector2.right);
        } 
        else if (controlSignal.y > 0) {
            // Down
            movement.SetDirection(Vector2.down);
        }

        float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }


    public void addPellet(GameObject pellet)
    {
        PelletStruct tmp;
        tmp.trans = pellet.transform;
        tmp.id = pellet.GetInstanceID();
        tmp.state = true;

        pellets[pid] = tmp;
        ++pid;
    }

    public void deletePellet(GameObject pellet)
    {
        int value = pellet.GetInstanceID();
        ChangePelletState(pellets, value, false);
    }

    public void addPowerPellet(GameObject powerPellet)
    {
        PelletStruct tmp;
        tmp.trans = powerPellet.transform;
        tmp.id = powerPellet.GetInstanceID();
        tmp.state = true;

        powerPellets[ppid] = tmp;
        ++ppid;
    }
    
    public void deletePowerPellet(GameObject powerPellet)
    {
        int value = powerPellet.GetInstanceID();
        ChangePelletState(powerPellets, value, false);
    }

    public void eatenPowerPellet(float duration)
    {
        bonusDuration = duration;
        bonusState = true;
    }

    public void AgentAddReward(float reward)
    {
        AddReward(reward);
    }

    void ChangePelletState(PelletStruct[] pels, int id, bool force)
    {
        for (int i = 0; i < pels.Length; i++)
        {
            if (force)
            {
                pels[i].state = !pels[i].state;
                continue;
            }

            if (pels[i].id == id)
            {
                pels[i].state = !pels[i].state;
                break;
            }
        }
    }

    public void PacmanDead()
    {
        ChangePelletState(pellets, 0, true);
        ChangePelletState(powerPellets, 0, true);
        AddReward(-1f);
        EndEpisode();
    }
}
