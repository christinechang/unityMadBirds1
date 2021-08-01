using UnityEngine;

public class Fish : MonoBehaviour
{
    public GameObject FishOriginal;
    public GameObject FishContainer;
    private float _lifetime;
    private Vector3 _scaleIncrease = new Vector3(0.1f, 0.1f, 1f);

    // SWIMMING
    private Vector2 _fishDir;
    private float _fishAngle;
    [SerializeField] private float SpeedMult = 3;

    bool FISH_COLLISION = false;
    bool WALL_COLLISION = true;

    // WALL COLLISIONS
    private int _numWallCollisions = 0;
    [SerializeField] private int MaxWallColls = 20;
    private bool isTouchingWall = false;
    private float maxDistance = 1f;

    // FISH COLLISIONS
    // private int _numCollisions = 0;
    private float _timeStalled = 0;
    private float _fishPauseTime = 1;
    private bool _fishCollisionPause;
    private int _numFishCollisions = 0;
    [SerializeField] private float lerpSpeed;

    // HAVING BABIES
    // public Vector2 _oldPosition;
    private float _babyTime, _babyTimer;
    private bool _babyOK = false;
    [SerializeField] private int _minimumSex = 3; // how many fishcollisions to have baby?
    [SerializeField] private int MaxFish = 10;

    private int _initPositionX = -10;
    private int[] _initPositionY = { 4, -1, -2, -5 }; // fish placed in different positions

    // FISH BUMP ROTATION
    private float _rotAngle;
    private float startAngle;
    private float endAngle;
    private float ratio;
    private bool _fishRotating = false;
    // Start is called before the first frame update
    void Start()
    {
        //  _oldPosition = GetComponent<Rigidbody2D>().position;
        _lifetime = 300 + Random.Range(-50f, 50f);
        _babyTime = 60.0f;
        _babyTimer = 0.0f;
        lerpSpeed = Random.Range(0.5f, 2f);
        Destroy(gameObject, _lifetime);
        //transform.localScale = new Vector3(3f, 3f, 1f); // start little fish
        transform.localScale = new Vector3(12f, 12f, 1f); // start big fish
        Debug.Log("WALL COLLISION!!!!!!!!!!!!!!!!!");
        setFishAngle(WALL_COLLISION);
        transform.rotation = Quaternion.Euler(0, 0, _fishAngle);
    }

    // Update is called once per frame
    void Update()
    {
        // get angle of direction/force, calc rotation and apply
        Vector2 curPosition = GetComponent<Transform>().position;
        _babyTimer += Time.deltaTime;
        if (_fishCollisionPause)
        { // pause first
            _timeStalled += Time.deltaTime;

            if (_timeStalled > _fishPauseTime)
            {
                _fishCollisionPause = false;
                _timeStalled = 0;
            }
        }
        else if (_fishRotating) // fish turns
        {
            fishLerpRot();
            GetComponent<Transform>().position = curPosition + (_fishDir * Time.deltaTime * SpeedMult);
        }

        else // fish moves forward
        {
            GetComponent<Transform>().position = curPosition + (_fishDir * Time.deltaTime * SpeedMult);

            // test to see if FishAngle and actual direction are different - could be stuck on the wall - reset
            float curAngle = transform.localEulerAngles.z;
            if (Mathf.Abs(curAngle) - Mathf.Abs(_fishAngle) > 10)
            {
                Debug.Log(name + "!!! ANGLES differ " + curAngle + "   " + _fishAngle);
                setFishAngle(FISH_COLLISION); // new rotate fish
                transform.rotation = Quaternion.Euler(0, 0, _fishAngle);
            //    Debug.Log(name + "!!! NEW ANGLE " + _fishAngle);
            }

            // fish light sword (shows FishDir)
            Vector2 newCurPosition = GetComponent<Transform>().position;
            Vector3[] linePoints = new Vector3[2];
            linePoints[0] = new Vector3(newCurPosition.x, newCurPosition.y, 0);
            linePoints[1] = linePoints[0] + new Vector3(4 * _fishDir.x, 4 * _fishDir.y, 0);
            GetComponent<LineRenderer>().SetPositions(linePoints);
        }
        /// Good Fish Growing ////////////////////////////////////////////////////// change to everytime it eats
        if (transform.localScale.x < 12)
        { // fish grows
            transform.localScale += _scaleIncrease * Time.deltaTime;
        }

        if (_babyTimer > _babyTime)
        {
            _babyTimer -= _babyTime; // reset _babyTimer
            //have baby
            _babyOK = true;
        }
    }
    void InitLerpRot()
    {
        startAngle = transform.localEulerAngles.z;
        Debug.Log("FISH COLLISION!!!");
        setFishAngle(FISH_COLLISION);
        ratio = 0f;
        _fishRotating = true;
        _fishCollisionPause = true;
    }

    void fishLerpRot()
    {
        if (_fishRotating)
        {
            ratio += lerpSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, startAngle + (ratio * _rotAngle));
            if (ratio >= 1f)
            {
                _fishRotating = false;
            }
        }
    }
    void setFishAngle(bool wall)
    {
        Debug.Log("wall collision? " + wall);
        if (wall)
        {
            //make new vector then get angle - go towards center with a bit of randomness added
            Vector2 curPosition = GetComponent<Transform>().position;
            _fishDir.x = -curPosition.x + (Random.Range(-5f, 5f));
            _fishDir.y = -curPosition.y + (Random.Range(-5f, 5f));
            _fishDir.Normalize();
            SpeedMult = Random.Range(2.1f, 3.3f);
            _fishAngle = Vector2.SignedAngle(Vector2.up, _fishDir);
            Debug.Log(name + "-------------WALL - fish ANGLE SET----------------------------");
        }
        else //fish collision or other
        {
            // get angle and THEN get vector
            _rotAngle = Random.Range(60f, 120f) * Mathf.Sign(Random.Range(-1f, 1f));
            _fishAngle += _rotAngle;
            // get new angle then reduce to -180 : 180
            _fishAngle %= 360;
            _fishAngle = Mathf.Abs(_fishAngle) > 180 ? _fishAngle - (Mathf.Sign(_fishAngle) * 360) : _fishAngle;

            _fishDir.x = -Mathf.Sin(Mathf.Deg2Rad * _fishAngle);
            _fishDir.y = Mathf.Cos(Mathf.Deg2Rad * _fishAngle);
            Debug.Log(name + "-------------FISH- new Angle set---------------------------");
        }
        Debug.Log("FISHANGLE: " + _fishAngle + ", _fishDir:  " + _fishDir);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_fishRotating) return; // if fish in middle of rotating - do nothing here

        Vector2 curPosition = GetComponent<Transform>().position;
        GetComponent<Transform>().position = curPosition - (3 * _fishDir * Time.deltaTime * SpeedMult); // all jump back from collision

        Fish fishColl = collision.collider.GetComponent<Fish>();
        if (fishColl != null) // if  fish collision
        {
            InitLerpRot();

            _numFishCollisions++;
            var numFish = GameObject.FindGameObjectsWithTag("Fish").Length;
            if (numFish < MaxFish && _numFishCollisions > _minimumSex && _babyOK)
            {
                _numFishCollisions = 0;
                _babyOK = false;
                HaveBaby(numFish);
            }
        }
        else // only one collision at a time
        {
            BoxCollider2D wallColl = collision.collider.GetComponent<BoxCollider2D>();
            if (wallColl != null) // if wall collision
            {
                _numWallCollisions++;
                if (_numWallCollisions > MaxWallColls && _lifetime > _lifetime - 10)
                {
                    Debug.Log("fish died off screen");
                    Destroy(gameObject);
                    return;
                }
                setFishAngle(FISH_COLLISION); // new fish angle
                transform.rotation = Quaternion.Euler(0, 0, _fishAngle);
            }
        }
    }
    private void HaveBaby(int numFish)
    {
        Vector2 NewFishPosition = new Vector2(_initPositionX, _initPositionY[numFish % 4]);
        Debug.Log("----------------NewFishposition:" + NewFishPosition);
        GameObject FishClone = Instantiate(FishOriginal, NewFishPosition, Quaternion.identity);
        FishClone.transform.parent = FishContainer.transform;
        FishClone.name = "FishClone" + numFish;
    }
    void OnMouseDown()
    {
        Debug.Log(name + "FishAngle: " + _fishAngle + "FishDir:" + _fishDir);
        return;
    }
    // attracted to a point
    // attracted to a moving point
    // speed affected by sine wave starting at random point in wave and multiplied by random length
    // direction changes slowly over time
}
/* FISH SIZE - grows with time OR grows with amound of food
 * 
 * FOOD - fish needs food at rate 10/60 seconds - or dies
 * FOOD - with each 10 food, increase in size
 * BIRTH - number of collisions with another fish - spawns 1 fish
 * FOOD appears with current - particle system
 * ANIMATIONS - fish forward, curve, pause
 * FOOD GRAVITY -- food attracts fish
 * LEADER FISH -- one fish attracts others
 * 
 * 
 * */
//transform.rotation = Quaternion.Euler(0, 0, _fishAngle);
////transform.localEulerAngles = new Vector3(0,0,_fishAngle);
///        //float angle = 0.0f;
//Vector3 axis = Vector3.zero;
//transform.rotation = Quaternion.Euler(0, 0, angle);

//transform.rotation.ToAngleAxis(out angle, out axis); //rotate to angle?
// private is the default
// int[] myIntArray = {1,2,3,45}; // note curly braces
// or 
// int[] myIntArray = new int [5];  // 5 elements in array wit square brackets
// allFish = GameObject.FindGameObjectWithTag("Fish");