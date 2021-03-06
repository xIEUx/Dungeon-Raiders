using UnityEngine;

public class Bats : MonoBehaviour
{
    public BatScript[] prefabs;
    
    public int rows = 5; //number of bats going up

    public int columns = 11; //number of bats across
    
    public AnimationCurve speed; //speed

    private Vector3 _direction = Vector2.right; //moves bats right

    public int amountKilled { get; private set; } //counts amount killed

    public int amountAlive => this.totalBats - this.amountKilled;

    public int totalBats => this.rows * this.columns; //calculates percentage of bats

    public float percentKilled => (float)this.amountKilled / (float)this.totalBats; //calculates total bats killed

    public float poisonAttackRate = 1.0f;

    public Projectile poisonPrefab;

    private void Awake()
    {
        for (int row = 0; row < this.rows; row++)
        {
            //creates a grids for the bats to be placed into
            float width = 8.0f * (this.columns - 1);
            float height = 8.0f * (this.rows - 1);
            Vector2 centering = new Vector2(-width / 2, -height /2);
            Vector3 rowPosition = new Vector3(centering.x, centering.y + (row * 8.0f),  0.0f);

            for (int col = 0; col < this.columns; col++)
            {
                //makes an array for the bat prefabs to be placed into a grid, can edit the number of bats spawned in grid through unity inspector
                BatScript bat = Instantiate(this.prefabs[row], this.transform);
                bat.killed += BatKilled;
                Vector3 position = rowPosition;
                position.x += col * 8.0f;
                bat.transform.localPosition = position; 
            }
        }

    }

    private void Start()
    {
        InvokeRepeating(nameof(PoisonAttack), this.poisonAttackRate, this.poisonAttackRate);
    }

    //used to make the Bats move, stops frame rate from having an affect on them

    private void Update()
    {
        this.transform.position += _direction * this.speed.Evaluate(this.percentKilled) * Time.deltaTime;

        //when bats touch edge of main camera...
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        //... go down 1.0f
        foreach (Transform bat in this.transform)
        {
            if (!bat.gameObject.activeInHierarchy) {
                continue;
            }
            //1.0f shows how far down bats move
            if (_direction == Vector3.right && bat.position.x >= (rightEdge.x - 1.0f))
            {
                AdvanceRow();
            }
            //1.0f shows how far down bats move
            else if (_direction == Vector3.left && bat.position.x <= (leftEdge.x + 1.0f)) {
                AdvanceRow();
            }
        }
    }

     private void AdvanceRow()
    {
        _direction.x *= -1.0f;

        Vector3 position = this.transform.position;
        position.y -= 1.0f;
        this.transform.position = position;
    }

    private void PoisonAttack()
    {
        foreach (Transform bat in this.transform)
        {
            if (!bat.gameObject.activeInHierarchy) {
                continue;
            }

            if (Random.value < (1.0f / (float)this.amountAlive))
        {
                Instantiate(this.poisonPrefab, bat.position, Quaternion.identity);
                break;
            }
        }
    }
    private void BatKilled()
    {
        this.amountKilled++;
    }

}
