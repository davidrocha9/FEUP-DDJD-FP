using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    
    [SerializeField] Transform recoilFollowPos;
    [SerializeField] float kickbackAmount = -1f;
    [SerializeField] float kickbackSpeed = 10f;
    [SerializeField] float returnSpeed = 20f;

    float currentRecoilPosition = 0f;
    float finalRecoilPosition = 0f;

    void Start()
    {
        kickbackAmount = -1f;
        kickbackSpeed = 10f;
        returnSpeed = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        currentRecoilPosition = Mathf.Lerp(currentRecoilPosition, 0, returnSpeed * Time.deltaTime);
        finalRecoilPosition = Mathf.Lerp(finalRecoilPosition, currentRecoilPosition, kickbackSpeed * Time.deltaTime);
        recoilFollowPos.localPosition = new Vector3(0, 0, finalRecoilPosition);
    }

    public void TriggerRecoil()
    {
        currentRecoilPosition = currentRecoilPosition + kickbackAmount;
    }

}
