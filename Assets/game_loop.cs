using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class game_loop : MonoBehaviour {
    uint p1_points = 0, p2_points = 0;

    public GameObject ball, p1_paddle, p2_paddle;
    public Text p1_text, p2_text, win_text;
    public Text info1, info2;
    Vector3 ball_speed;
    const int SCREEN_HEIGHT = 200;
    const int SCREEN_WIDTH = (int)(SCREEN_HEIGHT * 1.8);
    const float BALL_SIZE = 10.0f;
    const int PADDLE_LENGTH = 50;
    const int PADDLE_THICKNESS = 10;
    const int PADDLE_OFFSET = 10;
    const float PADDLE_SPEED = 3.0f;
    const float INITIAL_BALL_SPEED = 4.0f;


    enum State { START, SERVE, PLAYING, GAME_END };
    State state = State.START;
	// Use this for initialization
	void Start () {
        Screen.SetResolution(360, 200, false);
        win_text.gameObject.SetActive(false);

    }

    bool checkCollision(GameObject paddle)
    {
        if (ball.transform.position.x + BALL_SIZE / 2 < paddle.transform.position.x - PADDLE_THICKNESS / 2)
            return false;
        if (ball.transform.position.x - BALL_SIZE / 2 > paddle.transform.position.x + PADDLE_THICKNESS / 2)
            return false;
        if (ball.transform.position.y + BALL_SIZE / 2 < paddle.transform.position.y - PADDLE_LENGTH / 2)
            return false;
        if (ball.transform.position.y - BALL_SIZE / 2 > paddle.transform.position.y + PADDLE_LENGTH / 2)
            return false;
        return true;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        

        p1_paddle.transform.Translate(0, Input.GetAxis("P1 Vertical")* PADDLE_SPEED, 0);
        p2_paddle.transform.Translate(0, Input.GetAxis("P2 Vertical")* PADDLE_SPEED, 0);
        if(state == State.START && (Input.GetAxis("P1 Serve") > 0 || Input.GetAxis("P1 Serve") > 0))
        {
            state = State.SERVE;
            info1.gameObject.SetActive(false);
            info2.gameObject.SetActive(false);
            ball.gameObject.SetActive(true);
            p1_text.gameObject.SetActive(true);
            p2_text.gameObject.SetActive(true);
        }

        if (state == State.SERVE)
        {
            uint player_to_serve = ((p1_points + p2_points) / 2) % 2 + 1;
            if (player_to_serve == 1)
            {
                ball.transform.position = p1_paddle.transform.position + new Vector3(15, 0);
                if (Input.GetAxis("P1 Serve") > 0)
                {
                    ball_speed.x = 2;
                    ball_speed.y = -1;
                    ball_speed = ball_speed.normalized * INITIAL_BALL_SPEED;
                    state = State.PLAYING;
                }
            }
            else {
                ball.transform.position = p2_paddle.transform.position - new Vector3(15, 0);
                if (Input.GetAxis("P2 Serve") > 0)
                {
                    ball_speed.x = -2;
                    ball_speed.y = 1;
                    ball_speed = ball_speed.normalized * INITIAL_BALL_SPEED;
                    state = State.PLAYING;
                }
            }

        }
        else if (state == State.PLAYING)
        {
            ball.transform.Translate(ball_speed);
            if (ball_speed.y < 0 && ball.transform.position.y < 5)
                ball_speed.y = -ball_speed.y;
            if (ball_speed.y > 0 && ball.transform.position.y > SCREEN_HEIGHT - 5)
                ball_speed.y = -ball_speed.y;
            if (ball_speed.x > 0 && checkCollision(p2_paddle))
            {
                float y_diff = -ball.transform.position.y + p2_paddle.transform.position.y;
                float angle = Mathf.Atan(y_diff / (PADDLE_LENGTH / 2));
                float speed = ball_speed.magnitude * 1.05f;
                ball_speed = new Vector3(speed * Mathf.Cos(angle + Mathf.PI), speed * Mathf.Sin(angle + Mathf.PI));
            }
            if (ball_speed.x < 0 && checkCollision(p1_paddle))
            {
                float y_diff = ball.transform.position.y - p1_paddle.transform.position.y;
                float angle = Mathf.Atan(y_diff / (PADDLE_LENGTH / 2));
                float speed = ball_speed.magnitude * 1.05f;
                ball_speed = new Vector3(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));
            }
            if (ball.transform.position.x < -BALL_SIZE)
            {
                p2_points++;
                state = State.SERVE;
                //p2_score_text.setString(std::to_string(p2_points));
                p2_text.text = p2_points.ToString();
            }
            if (ball.transform.position.x > SCREEN_WIDTH + BALL_SIZE)
            {
                p1_points++;
                state = State.SERVE;
                //p1_score_text.setString(std::to_string(p1_points));
                p1_text.text = p1_points.ToString();
            }
            if (System.Math.Abs(p1_points - p2_points) >= 2 && (p1_points >= 11 || p2_points >= 11))
            {
                win_text.gameObject.SetActive(true);
                if (p1_points > p2_points)
                {
                    win_text.text = "Player 1 wins!";
                }
                else {
                    win_text.text="Player 2 wins!";
                }
                state = State.GAME_END;
            }
        }
        if(state == State.GAME_END)
        {
           if( Input.GetAxis("P1 Serve") > 0 || Input.GetAxis("P1 Serve") > 0)
            {
                p1_points = p2_points = 0;
                state = State.SERVE;
                p1_text.text = p1_points.ToString();
                p2_text.text = p1_points.ToString();
                win_text.gameObject.SetActive(false);
            }

        }

    }
}
