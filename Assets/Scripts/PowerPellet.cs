using Unity.MLAgents;
using UnityEngine;

public class PowerPellet : Pellet
{
    public float duration = 8f;
    private PacmanAgent pagent;

    private void Start()
    {
        pagent = FindObjectOfType<PacmanAgent>();
        pagent.addPowerPellet(this.gameObject);
    }

    protected override void Eat()
    {
        if (this && this.gameObject)
            pagent.deletePowerPellet(this.gameObject);
        FindObjectOfType<GameManager>().PowerPelletEaten(this);
    }

}
