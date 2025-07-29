using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.Networking;

public class TimeManager : MonoSingleton<TimeManager>
{
    private TimeSpan serverOffset = TimeSpan.Zero;


    protected override void Awake()
    {
        base.Awake();
        _ = InitializeTime();
      
    }

    public async Task InitializeTime()
    {
        DateTime? serverTime = await GetWorldTimeAPI();

        if (serverTime.HasValue)
        {
            serverOffset = serverTime.Value - DateTime.UtcNow;
            Debug.Log($"[TimeManager] 서버 시간 초기화 성공 (Offset: {serverOffset.TotalSeconds}초)");
        }
        else
        {
            serverOffset = TimeSpan.Zero;
            Debug.LogError("[TimeManager] 서버 시간 초기화 실패 - 로컬 시간 사용");
        }
    }



    public DateTime Now()
    {
        return DateTime.UtcNow + serverOffset + TimeSpan.FromHours(9); //한국 시간 
    }

    DateTime lastClearDate;

   


    public bool HasOneDayPassed(DateTime lastDate) //다른것도 사용가능
    {
        return lastDate.Date < Now().Date; // 하루가 지났는지 판단
    }




    private async Task<DateTime?> GetWorldTimeAPI()
    {
        const string API_URL = "https://timeapi.io/api/Time/current/zone?timeZone=UTC";

        using (var webRequest = UnityWebRequest.Get(API_URL))
        {
            // SSL 인증서 검증 무시
            webRequest.certificateHandler = new BypassCertificate();


            var operation = webRequest.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            // [수정됨] 요청 완료 후 result 확인 (기존에는 완료 전 바로 체크해서 항상 실패했음)
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string json = webRequest.downloadHandler.text;
                Debug.Log($"[TimeManager] WorldTimeAPI 응답: {json}");

                // JSON 파싱 (timeapi.io 응답 구조)
                var data = JsonUtility.FromJson<TimeApiResponse>(json);
                if (string.IsNullOrEmpty(data.dateTime))
                {
                    Debug.LogError("[TimeManager] dateTime 필드가 비어 있음");
                    return null;
                }

                return DateTime.Parse(data.dateTime).ToUniversalTime();
            }
            else
            {
                // [수정됨] 실패 로그
                Debug.LogError($"[GetWorldTimeAPI] 실패: {webRequest.error}");
                return null;
            }
        }
    }
}


public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData) => true;
}

[Serializable]
public class TimeApiResponse
{
    public string dateTime;
}



/*
 * 
 * 
 *    private async Task<DateTime?> GetGoogleNTPTime()
    {
            return await Task.Run<DateTime?>(() =>
            {
                try
                {
                    string ntpServer = "kr.pool.ntp.org";
                    byte[] ntpData = new byte[48]; //통신 규약?? 암튼 지켜야함

                    ntpData[0] = 0x18; //통신규약 서버프로토콜 보내서 시간요청

                    var addresses = Dns.GetHostEntry(ntpServer).AddressList //NTP -> Ip주소로 변환하고
                                                               .Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToArray(); //IPv4로만 받기 
                    if (addresses.Length == 0)
                    {
                        Debug.LogError("IPv4 주소를 찾지 못했습니다.");
                        return null;
                    }

                    var ipEndPoint = new IPEndPoint(addresses[0], 123);




                    using (var socket = new UdpClient())
                    {
                        socket.Client.ReceiveTimeout = 5000;
                        socket.Connect(ipEndPoint);
                        Debug.Log("NTP 요청 보냄");
                        socket.Send(ntpData, ntpData.Length);
                        Debug.Log("NTP 응답 대기중...");
                        var response = socket.Receive(ref ipEndPoint); //다시 지점에 보내야함
                        //여기서 예외가 생긴다. 
                        //뭔가 방화벽문제인가? 아니면 IPv4가 아닌가?

                        Debug.Log($"NTP 응답 길이: {response.Length}");
                        response.CopyTo(ntpData, 0); //값복사 해서 사용하기 
                    }
                    const byte serverReplyTime = 40; //서버 통신 얼마나 걸리는지 측정
                    ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime); //48바이트에서 40~47번째 바이트가 서버 시간임
                    ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

                    var milliseconds = (intPart * 1000 + (fractPart * 1000) / 0x100000000L); //이게 뭐고

                    return (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds).ToUniversalTime();

                }
                catch (Exception ex)
                {
                    Debug.LogError($"[GetGoogleNTPTime] 예외 발생: {ex}");
                    return null;
                }

            });
    }


   private static uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
                      ((x & 0x0000ff00) << 8) +
                      ((x & 0x00ff0000) >> 8) +
                      ((x & 0xff000000) >> 24));
    }


    //네트워크에서 빅라디안으로 오면 그걸 CPU 가 이해하기 위해 리틀 라디안으로 바꿔야한다. 
 * 
 */