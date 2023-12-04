using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Pellet : MonoBehaviour
{
    public int points = 10;
    private PacmanAgent agent;

    private void Start()
    {
        agent = FindObjectOfType<PacmanAgent>();
        agent.addPellet(this.gameObject);
    }

    protected virtual void Eat()
    {
        FindObjectOfType<GameManager>().PelletEaten(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pacman")) {
            if (this && this.gameObject)
                agent.deletePellet(this.gameObject);
            Eat();
        }
    }

}
