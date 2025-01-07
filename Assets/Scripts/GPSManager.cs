using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class GPSManager : MonoBehaviour
{
    public Text text_ui;             // UI �ؽ�Ʈ ��Ҹ� ������ ����
    public GameObject welcome_popUp; // ȯ�� �˾� â�� ������ ����
    public bool isFirst = false;     // ���� �˾��� �� ���� ǥ���ϱ� ���� �÷���

    public double[] lats;            // ��ǥ������ ���� �迭
    public double[] longs;           // ��ǥ������ �浵 �迭

    IEnumerator Start()
    {
        // ��ġ ������ �ο����� �ʾ��� ��� ���� ��û
        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            yield return null;
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        // ��ġ ���񽺰� ����ڿ� ���� Ȱ��ȭ�Ǿ� �ִ��� Ȯ��
        if (!Input.location.isEnabledByUser)
            yield break;

        // ��ġ ���� ���� (��Ȯ�� 10m, ���� �ֱ� 1��)
        Input.location.Start(10, 1);

        int maxWait = 20;
        // ��ġ ���� �ʱ�ȭ ���
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // ��ġ ���� �ʱ�ȭ Ÿ�Ӿƿ�
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // ��ġ ���� �ʱ�ȭ ����
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            // ��ġ ���� ǥ��
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

            // ��ġ ������ UI�� ������Ʈ�ϴ� ���� ����
            while (true)
            {
                yield return null;
                text_ui.text = Input.location.lastData.latitude + " / " + Input.location.lastData.longitude;
            }
        }

        // Input.location.Stop(); // ��ġ ���� ���� (�ʿ�� �ּ� ����)
    }

    void Update()
    {
        // ��ġ ���񽺰� ���� ���̰�, ��ȿ�� ��ġ �����Ͱ� �ִ� ���
        if (Input.location.status == LocationServiceStatus.Running)
        {
            double myLat = Input.location.lastData.latitude;
            double myLong = Input.location.lastData.longitude;

            // ���� ��ġ�� ��ǥ ��ġ ���� �Ÿ� ���
            double remainDistance = distance(myLat, myLong, lats[0], longs[0]);

            // ��ǥ ��ġ�� �����ϸ�
            if (remainDistance <= 10f)  // ������ 10m�� ����
            {
                // ���� �˾��� �� ���� ǥ��
                if (!isFirst)
                {
                    isFirst = true;
                    welcome_popUp.SetActive(true); // ȯ�� �˾� Ȱ��ȭ
                }
            }
        }
    }

    // �� ���� ���� �Ÿ��� ����ϴ� �Լ� (���� ����)
    private double distance(double lat1, double lon1, double lat2, double lon2)
    {
        // �� ���� ���� ���� ���� ���
        double theta = lon1 - lon2;

        // ���� �� �浵�� ���� ������ ��ȯ�Ͽ� �Ÿ� ���
        double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) +
                      Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));

        // �Ÿ��� ������ ��ȯ
        dist = Math.Acos(dist);

        // ���� ���� ������ ��ȯ
        dist = Rad2Deg(dist);

        // �ػ� ����(m)�� ��ȯ
        dist = dist * 60 * 1.1515;

        // ���ͷ� ��ȯ
        dist = dist * 1609.344;

        return dist;
    }

    // ������ �������� ��ȯ�ϴ� �Լ�
    private double Deg2Rad(double deg)
    {
        return (deg * Mathf.PI / 180.0f);
    }

    // ������ ������ ��ȯ�ϴ� �Լ�
    private double Rad2Deg(double rad)
    {
        return (rad * 180.0f / Mathf.PI);
    }
}
