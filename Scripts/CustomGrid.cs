using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrid : MonoBehaviour
{
    public GameObject target;
    public GameObject structure;
    private GameObject hint;

    private Vector3 truePos;
    private Quaternion trueRotate;

    public float gridSize;
    public float yMinRotate;
    public float startY = 0f;
    public float minY = 1f;
    [Space]
    public Vector3 startPos;
    [Space]
    public AudioClip build;
    public AudioClip end;
    [Space]
    public GameObject win;


    private int nowStage = 0;
    private AudioSource audio;

    public BrickComponent[] bricks;

    [System.Serializable]
    public class BrickComponent
    {
        public GameObject hint;
        public GameObject brick;
        public GameObject setBrick;
        [Space]
        public Quaternion rotation;
        public Vector3 needPosition;
    }

    [Space]
    public hints[] allHints;

    [System.Serializable]
    public class hints
    {
        public bool hintEnable;
        [Space]
        public GameObject hintObj;
    }

    public void ChangeStage()
    {

        //structure.transform.position = startPos;

        hint = Instantiate(bricks[nowStage].hint, bricks[nowStage].needPosition, bricks[nowStage].rotation);

        hint.transform.name = "hint";

        target.transform.position = startPos;
        structure = Instantiate(bricks[nowStage].brick, startPos, Quaternion.identity);

        if (allHints[nowStage].hintEnable == true)
        {
            if (nowStage > 0)
            {
                allHints[nowStage - 1].hintObj.SetActive(false);
            }
            allHints[nowStage].hintObj.SetActive(true);
        }
        else
        {
            allHints[nowStage - 1].hintObj.SetActive(false);
        }
    }

    void Start()
    {
        ChangeStage();
        audio = GetComponent<AudioSource>();
    }

    void LateUpdate()
    {
        truePos.x = Mathf.Floor(target.transform.position.x / gridSize) * gridSize;
        truePos.y = startY;
        truePos.z = Mathf.Floor(target.transform.position.z / gridSize) * gridSize;

        structure.transform.position = truePos;

        ///////

        if (target.transform.rotation.y > 0.6f)
        {
            structure.transform.rotation = Quaternion.Euler(0f, Mathf.Ceil(target.transform.rotation.y) * yMinRotate, 0f);
        }
        if (target.transform.rotation.y < 0.6f)
        {
            structure.transform.rotation = Quaternion.Euler(0f, -Mathf.Floor(target.transform.rotation.y) * yMinRotate, 0f);
        }

        Vector3 origin = new Vector3(structure.transform.position.x - 0.1f, structure.transform.position.y + 0.1f, structure.transform.position.z - 0.1f);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = 0.2f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            if (hit.collider.gameObject.name == "raycast")
            {
                startY += 1f;
            }
            if (hit.collider.gameObject.name == "plate")
            {
                startY += minY;
            }
            if (hit.collider.gameObject.name == "floor")
            {
                startY = 0f;
            }
            if (hit.collider.gameObject.name == "roof")
            {

            }
        }
        else
        {
            startY -= minY;
            Debug.DrawRay(origin, direction * distance, Color.red);
        }

        print(Mathf.Abs(structure.transform.rotation.y) == hint.transform.rotation.y);

        if(hint.transform.position == structure.transform.position && hint.transform.rotation.y == Mathf.Abs(structure.transform.rotation.y))
        {
            Destroy(hint);
            Destroy(structure);
            Instantiate(bricks[nowStage].setBrick, bricks[nowStage].needPosition, bricks[nowStage].rotation);

            audio.PlayOneShot(build, 1f);

            nowStage += 1;

            if (nowStage != bricks.Length)
            {

                ChangeStage();
                
            }
            
            //structure.SetActive(false);
        }

        //target.transform.position = structure.transform.position;
        
        //print(nowStage);
        if (nowStage == bricks.Length)
        {
            win.SetActive(true);
            audio.PlayOneShot(end, 1f);
        }
    }
}
