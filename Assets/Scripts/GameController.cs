using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject snakeHeadPrefab;
    public GameObject snakeSegmentPrefab;
    public GameObject foodPrefab;
    public GameObject wallPrefab;
    public float moveInterval = 0.4f; // 调整移动速度

    private Transform snakeHead;
    private List<Transform> snakeSegments = new List<Transform>();
    private Vector2Int direction = Vector2Int.right;
    private float timer;

    void Start()
    {
        Debug.Log("Game Started");
        // 初始化蛇头
        snakeHead = Instantiate(snakeHeadPrefab, Vector3.zero, Quaternion.identity).transform;
        snakeSegments.Add(snakeHead);
        SpawnWalls();
        SpawnFood();
    }

    void Update()
    {
        // 根据输入更改方向
        if (Input.GetKeyDown(KeyCode.W) && direction != Vector2Int.down) direction = Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.S) && direction != Vector2Int.up) direction = Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.A) && direction != Vector2Int.right) direction = Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.D) && direction != Vector2Int.left) direction = Vector2Int.right;

        // 按设定的时间间隔移动蛇
        timer += Time.deltaTime;
        if (timer >= moveInterval)
        {
            Debug.Log("Moving Snake");
            MoveSnake();
            timer = 0f;
        }
    }

    void MoveSnake()
    {
        Vector2Int nextPosition = Vector2Int.RoundToInt(snakeHead.position) + direction;
        Vector3 nextPosition3D = new Vector3(nextPosition.x, nextPosition.y, 0);

        // 检查与墙壁或自身的碰撞
        if (nextPosition.x < -Camera.main.orthographicSize * Camera.main.aspect ||
            nextPosition.x >= Camera.main.orthographicSize * Camera.main.aspect ||
            nextPosition.y < -Camera.main.orthographicSize ||
            nextPosition.y >= Camera.main.orthographicSize ||
            snakeSegments.Exists(segment => segment.position == nextPosition3D))
        {
            // 游戏结束
            Debug.Log("Game Over");
            return;
        }

        // 移动蛇
        for (int i = snakeSegments.Count - 1; i > 0; i--)
        {
            snakeSegments[i].position = snakeSegments[i - 1].position;
        }
        snakeHead.position = nextPosition3D;

        // 检查食物碰撞
        if (snakeHead.position == GameObject.FindWithTag("Food").transform.position)
        {
            GrowSnake();
            SpawnFood();
        }
    }

    void GrowSnake()
    {
        Vector3 newSegmentPosition = snakeSegments[snakeSegments.Count - 1].position;
        snakeSegments.Add(Instantiate(snakeSegmentPrefab, newSegmentPosition, Quaternion.identity).transform);
    }

    void SpawnFood()
    {
        // 删除场景中现有的食物
        GameObject existingFood = GameObject.FindWithTag("Food");
        if (existingFood != null)
        {
            Destroy(existingFood);
        }

        // 获取相机的可视范围
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        // 生成新的食物位置
        Vector2Int foodPosition;
        do
        {
            foodPosition = new Vector2Int(
                Random.Range(Mathf.FloorToInt(-camWidth / 2), Mathf.CeilToInt(camWidth / 2)),
                Random.Range(Mathf.FloorToInt(-camHeight / 2), Mathf.CeilToInt(camHeight / 2))
            );
            Debug.Log("Trying position: " + foodPosition);
        } while (snakeSegments.Exists(segment => segment.position == new Vector3(foodPosition.x, foodPosition.y, 0)) || foodPosition == Vector2Int.zero);

        // 实例化新的食物
        Instantiate(foodPrefab, new Vector3(foodPosition.x, foodPosition.y, 0), Quaternion.identity);
        Debug.Log("Spawned Food at: " + foodPosition);
    }

    void SpawnWalls()
    {
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        // 水平方向墙壁
        for (int x = Mathf.FloorToInt(-camWidth / 2); x <= Mathf.CeilToInt(camWidth / 2); x++)
        {
            Instantiate(wallPrefab, new Vector3(x, Mathf.FloorToInt(-camHeight / 2), 0), Quaternion.identity);
            Instantiate(wallPrefab, new Vector3(x, Mathf.CeilToInt(camHeight / 2), 0), Quaternion.identity);
        }

        // 垂直方向墙壁
        for (int y = Mathf.FloorToInt(-camHeight / 2); y <= Mathf.CeilToInt(camHeight / 2); y++)
        {
            Instantiate(wallPrefab, new Vector3(Mathf.FloorToInt(-camWidth / 2), y, 0), Quaternion.identity);
            Instantiate(wallPrefab, new Vector3(Mathf.CeilToInt(camWidth / 2), y, 0), Quaternion.identity);
        }
    }
}
