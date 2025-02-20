﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BombGage : MonoBehaviour
{
    public static BombGage Instance
    {
        get
        {
            if (instance != null)
                return instance;
            instance = FindObjectOfType<BombGage>();
            return instance;
        }
    }

    private static BombGage instance;

    public Slider slider;
	public Image sliderBackgroundImage;
	public RectTransform[] handle;
	public RectTransform gageHandler;
	public Animator stageAnimator;
	public TextMeshProUGUI BombInstallText;
	float number;

	float currentGage;
	bool isStart = false;
	int randomNumIndex = 0;

	float timeCount;

	bool printMessage = true;
	float addSpeed = 0.5f;

	bool isCoroutineStarted;

	float[] randomNum = new float[3];
	public IEnumerator BombGageCoroutine;

    public Animator playerAnim;
    public GameObject ScifiBomb;

	void Awake()
	{
		BombGageCoroutine = BombInstall();
		installedBombCount = 0;
		BombInstallText.text = (4 - installedBombCount).ToString();
	}

	void Start()
	{
		GenerateRandomPos();
	}

	public bool canInstall;
	public bool isInstalling;
	[HideInInspector] public float correctCount = 0;
	public IEnumerator BombInstall()
	{
		isCoroutineStarted = false;
		if (!isCoroutineStarted)
        {
            isCoroutineStarted = true;
            correctCount = 0;
            //PlayerManager.isHit = false;

            slider.GetComponent<Animator>().SetBool("BombGageFade", true);
            yield return new WaitForSeconds(.5f);

			// install sound play
			while (currentGage <= 4)
            {
				BombInstallInstructor.Instance.CorrectBombInstruction();

				//if (PlayerManager.isHit) break;

                addSpeed += Time.deltaTime;
                currentGage = Mathf.Clamp(currentGage += Time.deltaTime * 1.5f * addSpeed, 0, 4);
                slider.value = Mathf.Floor(currentGage * 100f) * 0.25f;
                if (BombInstallInstructor.Instance.isOutOfBound)
                {
                    slider.GetComponent<Animator>().SetBool("BombGageFade", false);
                    break;
                }
                if (slider.value > randomNum[randomNumIndex] + 5 && correctCount < 3)
                {
                    //Debug.Log("FIRST");
                    slider.GetComponent<Animator>().SetBool("BombGageFade", false);
                    break;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    //Debug.Log("Click B");
                    if (randomNum[randomNumIndex] - 5 < slider.value && slider.value < randomNum[randomNumIndex] + 5)
                    {
                        // change color
                        handle[randomNumIndex].GetComponent<Image>().color = Color.red;

                        // correct sound play
                        randomNumIndex++;
                        if (randomNumIndex >= 3)
                            randomNumIndex = 0;
                        correctCount++;
                    }
                    else
                    {
                        //Debug.Log("SECOND");
                        slider.GetComponent<Animator>().SetBool("BombGageFade", false);
                        break;
                    }
                }

                if (correctCount >= 3f && slider.value.Equals(100))
                {
                    //Debug.Log("CLEAR");
                    BombIsInstalled();
                    //playerAnim.SetTrigger("SuccesBombSet");
                    //playerAnim.SetBool("ReadyToBombSet", false);
                    playerAnim.SetTrigger("endBomb");
                    //myWeapon.myWeapnType = myWeapon.prevWeaponType;
                    Instantiate(ScifiBomb, playerAnim.transform.position + new Vector3(0f, 0.5f, 0f), transform.rotation, null);
                    break;
                }
                yield return null;
            }
			//Debug.Log("THIRD");
			
			slider.GetComponent<Animator>().SetBool("BombGageFade", false);
			isInstalling = false;
			WeaponCtrl.Instance.isHoldingBomb = false;

			if (correctCount < 3) BombInstallInstructor.Instance.InstallBombInstruction();

			yield return new WaitForSeconds(.5f);
            for (int i = 0; i < randomNum.Length; i++)
            {
                handle[i].GetComponent<Image>().color = Color.white;
            }

            //playerAnim.ResetTrigger("SuccesBombSet");
            randomNumIndex = 0;
            slider.value = 0;
            addSpeed = 0;
            currentGage = 0;
			correctCount = 0;
			GenerateRandomPos();
        }
		isCoroutineStarted = false;
		//StopCoroutine(BombGageCoroutine);
	}

	void GenerateRandomPos()
	{
		for (int i = 0; i < randomNum.Length; i++)
		{
			randomNum[i] = Random.Range(5 + (i * 25), 35 + (i * 25));
			slider.handleRect = handle[i];
			handle[i].anchoredPosition = new Vector2(0, 0);
			handle[i].sizeDelta = new Vector2(20, 0);
			handle[i].localScale = new Vector3(1, 1, 1);
			slider.value = randomNum[i];
		}

		slider.handleRect = gageHandler;
		slider.value = 0;
	}

	public int installedBombCount = 0;
	public ZoneTriggers[] zoneTriggers;

	void BombIsInstalled()
	{
		if (installedBombCount < 4)
		{
			installedBombCount++;
			BombInstallText.text = (4 - installedBombCount).ToString();
		}

		for(int i = 0; i < zoneTriggers.Length; i++)
		{
			if (zoneTriggers[i].isColliding)
			{
				zoneTriggers[i].changeZoneCircleRange();
				break;
			}
		}
	}

	public GameObject[] bombPlace;
	public void SetBombPlace()
	{
		stageAnimator.SetBool("FadeIn", true);

		bombPlace[0].transform.position = new Vector3(10 + Random.Range(-5f, 5f), bombPlace[0].transform.position.y, 10 + Random.Range(-5f, 5f));
		bombPlace[1].transform.position = new Vector3(-10 + Random.Range(-5f, 5f), bombPlace[0].transform.position.y, 10 + Random.Range(-5f, 5f));
		bombPlace[2].transform.position = new Vector3(-10 + Random.Range(-5f, 5f), bombPlace[0].transform.position.y, -10 + Random.Range(-5f, 5f));
		bombPlace[3].transform.position = new Vector3(10 + Random.Range(-5f, 5f), bombPlace[0].transform.position.y, -10 + Random.Range(-5f, 5f));

		for (int i = 0; i < bombPlace.Length; i++)
		{
			bombPlace[i].SetActive(true);
		}
	}
}
