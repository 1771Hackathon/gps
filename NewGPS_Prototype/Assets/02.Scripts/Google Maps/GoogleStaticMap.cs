using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoogleStaticMap : MonoBehaviour
{
	public RawImage rawImage;
	[Range(0f, 1f)]
	public float transparency = 1f;
	public float mapCenterLatitude = 37.5665350f;
	public float mapCenterLongtitude = 126.9779690f;
	[Range(1, 20)]
	public int mapZoom = 15;
	public int mapWidth = 1440;
	public int mapHeight = 1440;
	public enum MapType
	{
		roadmap, satellite, terrain, hybrid,
	}
	public MapType mapType = MapType.roadmap;
	[Range(1, 4)]
	public int scale = 1;
	public float markerLatitude = 37.5665350f;
	public float markerLongtitude = 126.9779690f;
	public enum MarkerSize
	{
		tiny, mid, small,
	}
	public MarkerSize markerSize = MarkerSize.mid;
	public enum MarkerColor
	{
		black, brown, green, purple, yellow, blue, gray, orange, red, white,
	}
	public MarkerColor markerColor = MarkerColor.blue;
	public char label = 'C';
	public string apiKey;

	private string url;
	private Color rawImageColor = Color.white;

    public Text coordinates;

    double detailed_num = 1.0;

	IEnumerator Map()
	{
        coordinates.text = "Lat : " + mapCenterLatitude * detailed_num + "\nLon : " + mapCenterLongtitude * detailed_num + "\nMap is Updated!";
        rawImageColor.a = transparency;
		rawImage.color = rawImageColor;
        markerLatitude = mapCenterLatitude;
        markerLongtitude = mapCenterLongtitude;

        label = Char.ToUpper(label);

		url = "https://maps.googleapis.com/maps/api/staticmap"
			+ "?center=" + mapCenterLatitude + "," + mapCenterLongtitude
			+ "&zoom=" + mapZoom
			+ "&size=" + mapWidth + "x" + mapHeight
			+ "&scale=" + scale
			+ "&maptype=" + mapType
			+ "&markers=size:" + markerSize + "%7Ccolor:" + markerColor + "%7Clabel:" + label + "%7C" + markerLatitude + "," + markerLongtitude
			+ "&key=" + apiKey;
		WWW www = new WWW(url);
		yield return www;
		rawImage.texture = www.texture;
		//rawImage.SetNativeSize();

        coordinates.text = "Lat : " + mapCenterLatitude * detailed_num + "\nLon : " + mapCenterLongtitude * detailed_num;

        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(Map());
	}

	public void RefreshMap()
	{
        
        if (gameObject.activeInHierarchy)
		{
			StartCoroutine(Map());
		}
	}

	private void Reset()
	{
		rawImage = gameObject.GetComponentInChildren<RawImage>();
		RefreshMap();
	}

	private void Start()
	{
		Reset();
        StartCoroutine(StartLocationService());
    }

#if UNITY_EDITOR
	private void OnValidate()
	{
		RefreshMap();
	}
#endif

    private IEnumerator StartLocationService()
    {
        
            if (!Input.location.isEnabledByUser)
            {
                Debug.Log("User has not enabled GPS");
                yield return new WaitForSeconds(1f);
            }

            Input.location.Start();
            int maxWait = 20;
        
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1f);
                maxWait--;
            }

            if (maxWait <= 0)
            {
                Debug.Log("Timed out");
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.Log("Unable to determine device location");
                yield break;
            }
                mapCenterLatitude = Input.location.lastData.latitude;
                mapCenterLongtitude = Input.location.lastData.longitude;
                coordinates.text = "Lat : " + mapCenterLatitude * detailed_num + "\nLon : " + mapCenterLongtitude * detailed_num
                    + "\nGPS is updated!";

        /*
            while (Input.location.isEnabledByUser)
            {
                yield return new WaitForSeconds(1f);
            }
            */
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(StartLocationService());
        
    }
}
