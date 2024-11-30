using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoadingScene : MonoBehaviourPun
{
    // 로비씬에 넣어놓고 로비에 있는 정보를 가지고 게임으로 들어가기 
    // 씬에 입장하면 랜덤 스폰기능 
    // 투표씬 전환
    [SerializeField] Transform[] SpawnPoints;

    private GameObject player;
    public static GameObject MyPlayer { get { return Instance.player; } }
    private Color color;

    private bool isOnGame = false;

    public static GameLoadingScene Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 기존 인스턴스가 있으면 새로 생성된 객체를 제거
        }
        SpawnPoints = new Transform[6];
    }
    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))  // 체크용으로 만들어 둠 지워도 됨 
        {
            GameOverKill();
            GameOverMission();
        }
    }

    public void GameStart()
    {
        SceneChanger.LoadLevel(1);
        isOnGame = true;
        StartCoroutine(Delaying());
    }



    IEnumerator Delaying()
    {
        yield return 2f.GetDelay();

        RandomSpawner(); // 스폰 지정 및 소환 
        yield return null;

        PlayerDataContainer.Instance.RandomSetjob(); // 랜덤 직업 설정 
        photonView.RPC(nameof(RpcSyncPlayerData), RpcTarget.AllBuffered);
    }


    // 게임 종료 조건 
    // 오리승리 : 거위 모두 사망
    // 거위승리 : 오리 모두 사망 , 미션 게이지 달성
    // => 투표 종료 , 플레이어 사망시마다 조건 확인    미션할때마다 게이지 확인 
    // 확인해서 조건 맞으면 게임 종료하기
    // 게임 종료 조건이 되면 모두 결과창 보여주고 다시 같이 로비로
    // 조건 체크를 방장(PhotonNetwork.MasterClient)만 해야하나? 아님 씬 이동이 rpc니 상관없나 
    private int GooseNotDead = 0; // 생존한 오리 , 거위
    private int DuckNotDead = 0;
    public void GameOverKill() // 투표종료 및 살인시마다 호출 
    {   
        
        PlayerDataContainer.Instance.SetPlayerTypeCounts();
        GooseNotDead = PlayerDataContainer.Instance.GooseCount;
        DuckNotDead = PlayerDataContainer.Instance.DuckCount;
        Debug.Log($"생존 : 거위 {GooseNotDead} 오리 {DuckNotDead}");

        if (GooseNotDead < DuckNotDead)// 거위의 수가 오리보다 적으면 오리 승리 , 투표가도 못이기니까   or 남은 거위가 없으면
        {
            // 오리승리로 게임 결과 표시 후 로비로 이동
        }
        else if (DuckNotDead == 0)  // 오리가 다 죽으면  거위 승리 
        {
            // 거위승리로 게임 결과 표시 후 로비로 이동
        }
    }
    public void GameOverMission() // 미션완료시마다 호출 
    {
        
        if (GameManager.Instance._missionScoreSlider.value == 1f) 
        {
            // 미션완료승리로 게임 결과 표시 후 로비로 이동 
        }
        
    }




    private void RandomSpawner()
    {
        photonView.RPC("RpcRandomSpawner", RpcTarget.All);
    }

    private void spawnPlayer(Vector3 Pos)
    {
        player = PhotonNetwork.Instantiate("LJH_Player", Pos, Quaternion.identity);
        color = PlayerDataContainer.Instance.GetPlayerData(PhotonNetwork.LocalPlayer.GetPlayerNumber()).PlayerColor;
        GameObject panel = PhotonNetwork.Instantiate("NamePanel", Pos, Quaternion.identity);
        panel.GetComponent<UiFollowingPlayer>().setTarget(player);

    }

    [PunRPC]
    private void RpcRandomSpawner()
    {
        GameObject obj = GameObject.Find("SpawnPoint");

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            SpawnPoints[i] = obj.transform.GetChild(i);
        }
        int x = Random.Range(0, obj.transform.childCount);

        spawnPlayer(SpawnPoints[x].position);
    }

    [PunRPC]
    private void RpcSyncPlayerData()
    {
        player.GetComponent<PlayerController>().SettingColor(color.r, color.g, color.b);  // 일단 보류 색 보존이 안됨 
        player.GetComponent<PlayerController>().SetJobs();
        PlayerDataContainer.Instance.SetPlayerTypeCounts();
    }
}
