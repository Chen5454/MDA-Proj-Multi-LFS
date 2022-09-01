using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * <summary>Base class for all traffic lights</summary>
 */
public class BaseTrafficLight : MonoBehaviour
{
    Animator animator;

    const int MODE_OFF = 0;
    const int MODE_SYNC = 1;
    const int MODE_SYNC2 = 2;
    const int MODE_IDLE = 3;

    public BaseTrafficLight[] syncTrafficLights;
    public BaseTrafficLight[] shiftedSyncTrafficLights;
    public void StartSlavesWorking()
    {
        if (syncTrafficLights.Length > 0)
            foreach (var oneLight in syncTrafficLights)
                oneLight.StartSyncMode();

        if (shiftedSyncTrafficLights.Length > 0)
            foreach (var oneLight in shiftedSyncTrafficLights)
                oneLight.StartSyncMode2();
    }

    /*
     * @return true
     */

  /**
   * <summary>Check if traffic light is the master</summary>
   * <returns><c>true</c> if this traffic light controls any other traffic lights. Otherwise <c>false</c></returns>
   */
    public bool IsMaster()
    {
        if ((syncTrafficLights != null && syncTrafficLights.Length > 0) ||
            (shiftedSyncTrafficLights  != null && shiftedSyncTrafficLights.Length > 0))
            return true;
        else
            return false;
    }

    /**
     * <summary>Set all slaves traffic lights into idle mode</summary>
     */
    public void StartSlavesIdle()
    {
        if (syncTrafficLights.Length > 0)
            foreach (var oneLight in syncTrafficLights)
                oneLight.StartIdle();

        if (shiftedSyncTrafficLights.Length > 0)
            foreach (var oneLight in shiftedSyncTrafficLights)
                oneLight.StartIdle();
    }

    /**
     * <summary>Turn off all slaves traffic lights</summary>
     */
    public void SlavesOff()
    {
        if (syncTrafficLights.Length > 0)
            foreach (var oneLight in syncTrafficLights)
                oneLight.SetOff();

        if (shiftedSyncTrafficLights.Length > 0)
            foreach (var oneLight in shiftedSyncTrafficLights)
                oneLight.SetOff();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /**
     * <summary>Set traffic light to work in sync mode</summary>
     */
    public void StartSyncMode()
    {
        if (animator != null)
            animator.SetInteger("mode", MODE_SYNC);
    }

    /**
     * <summary>Set traffic light to work in shifted sync mode</summary>
     */
    public void StartSyncMode2()
    {
        if (animator != null)
            animator.SetInteger("mode", MODE_SYNC2);
    }

    /**
     * <summary>Set traffic light to idle mode</summary>
     */
    public void StartIdle()
    {
        if (animator != null)
            animator.SetInteger("mode", MODE_IDLE);
    }

    /**
     * <summary>Turn off traffic light</summary>
     */
    public void SetOff()
    {
        if (animator != null)
            animator.SetInteger("mode", MODE_OFF);
    }
}
